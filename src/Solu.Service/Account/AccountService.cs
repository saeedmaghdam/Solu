using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Solu.Domain;
using Solu.Framework;
using Solu.Framework.Constants;
using Solu.Framework.Exceptions;
using Solu.Framework.Services.Account;
using Solu.Framework.Shared;
using Solu.Service.Account.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Solu.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly IMongoCollection<Domain.Account> _accountCollection;
        private readonly IMongoCollection<Session> _sessionCollection;
        private readonly ISecurity _security;
        private readonly IJwtManager _jwtManager;
        private readonly INotificationHandler _notificationHandler;

        public AccountService(IOptionsMonitor<ApplicationOptions> options, ISecurity security, IJwtManager jwtManager, INotificationHandler notificationHandler)
        {
            _security = security;
            _jwtManager = jwtManager;

            var mongoClient = new MongoClient(
            options.CurrentValue.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                options.CurrentValue.DatabaseName);

            _accountCollection = mongoDatabase.GetCollection<Domain.Account>("Account");
            _sessionCollection = mongoDatabase.GetCollection<Session>("Session");
            _notificationHandler = notificationHandler;
        }

        public async Task<IAccount> GetAccountByToken(string token, CancellationToken cancellationToken)
        {
            var session = await _sessionCollection.Find(x => x.Token == token && x.IsValid).FirstOrDefaultAsync(cancellationToken);
            if (session == null)
                throw new ValidationException("100", "Token is invalid.");

            var account = await _accountCollection.Find(x => x.Id == session.AccountId).FirstOrDefaultAsync(cancellationToken);
            if (account == null)
                throw new ValidationException("100", "Account already exists.");

            return ToModel(new[] { account }).First();
        }

        public async Task<string> CreateAsync(string accountId, string username, string password, string name, string family, CancellationToken cancellationToken)
        {
            var account = await _accountCollection.Find(x => x.Id == accountId).FirstOrDefaultAsync(cancellationToken);
            if (account == null)
                throw new ValidationException("100", "Account not found.");

            var currentAccount = await _accountCollection.Find(x => x.Username.ToLower() == username.Trim().ToLower()).FirstOrDefaultAsync(cancellationToken);
            if (currentAccount != null)
                throw new ValidationException("100", "Account already exists.");

            account = new Domain.Account()
            {
                RecordInsertDate = DateTime.Now,
                RecordLastEditDate = DateTime.Now,
                RecordStatus = RecordStatus.Inserted,
                Family = family,
                IsAdmin = true,
                Name = name,
                Username = username,
                Password = _security.HashPassword(password)
            };

            var accounts = await _accountCollection.AsQueryable().ToListAsync(cancellationToken);
            if (accounts.Count > 0)
            {
                currentAccount = await _accountCollection.Find(x => x.Id == accountId).FirstOrDefaultAsync(cancellationToken);
                if (currentAccount == null)
                    throw new ValidationException("100", "Parent account not found.");

                if (!currentAccount.IsAdmin)
                    throw new ValidationException("100", "Action not allowed.");

                account.IsAdmin = false;
            }

            await _accountCollection.InsertOneAsync(account, new InsertOneOptions(), cancellationToken);

            return account.Id;
        }

        public async Task<bool> IsAuthenticated(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(token.Trim()))
                throw new ValidationException("100", "Token is required.");

            return await IMongoCollectionExtensions.Find<Session>(_sessionCollection, x => x.Token == token && x.IsValid).AnyAsync(cancellationToken);
        }

        public async Task<ILogin> LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(username.Trim()))
                throw new ValidationException("100", "Username is required.");

            if (string.IsNullOrEmpty(password))
                throw new ValidationException("100", "Password is required.");

            var account = _accountCollection.Find(x => x.Username.ToLower() == username.Trim().ToLower()).FirstOrDefault();
            if (account == null)
                throw new ValidationException("100", "Account not found.");

            if (!_security.VerifyHashedPassword(account.Password, password))
                throw new ValidationException("100", "Username or password is not valid.");

            var sessionId = Guid.NewGuid().ToString();
            var expirationDate = DateTime.Today.AddDays(365);

            var token = _jwtManager.GenerateToken(sessionId, account.Id, account.Name, account.Family, expirationDate);

            var session = new Session()
            {
                RecordInsertDate = DateTime.Now,
                RecordLastEditDate = DateTime.Now,
                RecordStatus = RecordStatus.Inserted,
                AccountId = account.Id,
                ExpirationDate = expirationDate,
                IsValid = true,
                Token = token
            };

            await _sessionCollection.InsertOneAsync(session, new InsertOneOptions(), cancellationToken);

            return new LoginModel()
            {
                ExpirationDate = session.ExpirationDate,
                IsSuccessful = true,
                Token = session.Token
            };
        }

        public async Task LogoutAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(token.Trim()))
                throw new ValidationException("100", "Token is required.");

            var filter = Builders<Session>.Filter.Where(x => x.Token == token && x.IsValid);
            var session = _sessionCollection.Find(filter).FirstOrDefault();
            if (session == null)
                throw new ValidationException("100", "Session not found.");

            session.IsValid = false;
            session.RecordStatus = RecordStatus.Updated;
            session.RecordLastEditDate = DateTime.Now;

            await _sessionCollection.ReplaceOneAsync(filter, session, new ReplaceOptions(), cancellationToken);
        }

        public async Task RegisterVerificationCodeAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            var currentAccount = await _accountCollection.Find(x => x.Username == mobileNumber.Trim().ToLower()).FirstOrDefaultAsync(cancellationToken);
            if (currentAccount != null && currentAccount.IsVerified)
                throw new ValidationException("100", $"A verified account with mobile number {mobileNumber} already exists.");

            if (currentAccount == null)
            {
                currentAccount = new Domain.Account()
                {
                    IsAdmin = false,
                    IsVerified = false,
                    RecordInsertDate = DateTime.Now,
                    RecordLastEditDate = DateTime.Now,
                    RecordStatus = RecordStatus.Inserted,
                    Username = mobileNumber.Trim().ToLower()
                };

                await _accountCollection.InsertOneAsync(currentAccount, new InsertOneOptions(), cancellationToken);
            }

            var verificationCode = new Random().Next(1000, 9999).ToString();
            currentAccount.VerificationCode = verificationCode;

            await _notificationHandler.SendVerificationCodeAsync(mobileNumber, verificationCode, cancellationToken);

            await _accountCollection.ReplaceOneAsync(x => x.Username == currentAccount.Username, currentAccount, new ReplaceOptions(), cancellationToken);
        }

        public async Task RegisterAsync(string mobileNumber, string password, string name, string family, string verificationCode, CancellationToken cancellationToken)
        {
            var currentAccount = await _accountCollection.Find(x => x.Username == mobileNumber.Trim().ToLower()).FirstOrDefaultAsync(cancellationToken);

            if (currentAccount == null)
                throw new ValidationException("100", "Account not found.");

            if (currentAccount != null && currentAccount.IsVerified)
                throw new ValidationException("100", $"A verified account with mobile number {mobileNumber} already exists.");

            if (currentAccount.VerificationCode != verificationCode)
                throw new ValidationException("100", "Verification code is not working.");

            currentAccount.Name = name;
            currentAccount.Family = family;
            currentAccount.Password = _security.HashPassword(password);
            currentAccount.IsVerified = true;
            currentAccount.RecordLastEditDate = DateTime.Now;
            currentAccount.RecordStatus = RecordStatus.Updated;
            currentAccount.VerificationCode = string.Empty;

            await _accountCollection.ReplaceOneAsync(x=> x.Username == mobileNumber.Trim().ToLower(), currentAccount, new ReplaceOptions(), cancellationToken);
        }

        public async Task ResetPasswordVerificationCodeAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            var currentAccount = await _accountCollection.Find(x => x.Username == mobileNumber.Trim().ToLower()).FirstOrDefaultAsync(cancellationToken);
            if (currentAccount == null)
                throw new ValidationException("100", $"Account not found.");

            var verificationCode = new Random().Next(1000, 9999).ToString();
            currentAccount.VerificationCode = verificationCode;
            currentAccount.RecordLastEditDate = DateTime.Now;
            currentAccount.RecordStatus = RecordStatus.Updated;

            await _notificationHandler.SendVerificationCodeAsync(mobileNumber, verificationCode, cancellationToken);

            await _accountCollection.ReplaceOneAsync(x=> x.Username == mobileNumber.Trim().ToLower(), currentAccount, new ReplaceOptions(), cancellationToken);
        }

        public async Task ResetPasswordAsync(string mobileNumber, string password, string verificationCode, CancellationToken cancellationToken)
        {
            var currentAccount = await _accountCollection.Find(x => x.Username.ToLower() == mobileNumber.Trim().ToLower()).FirstOrDefaultAsync(cancellationToken);
            if (currentAccount == null)
                throw new ValidationException("100", "Account not found.");

            if (currentAccount.VerificationCode != verificationCode)
                throw new ValidationException("100", "Verification code is not working.");

            currentAccount.IsVerified = true;
            currentAccount.Password = _security.HashPassword(password);
            currentAccount.RecordLastEditDate = DateTime.Now;
            currentAccount.RecordStatus = RecordStatus.Updated;
            currentAccount.VerificationCode = string.Empty;

            await _accountCollection.ReplaceOneAsync(x => x.Username.ToLower() == mobileNumber.Trim().ToLower(), currentAccount, new ReplaceOptions(), cancellationToken);
        }

        public static IEnumerable<IAccount> ToModel(IEnumerable<Domain.Account> accounts)
        {
            return accounts.Select(x => new AccountModel()
            {
                Id = x.Id,
                Family = x.Family,
                IsAdmin = x.IsAdmin,
                Name = x.Name,
                RecordInsertDate = x.RecordInsertDate,
                RecordLastEditDate = x.RecordLastEditDate,
                RecordStatus = x.RecordStatus,
                Username = x.Username
            });
        }
    }
}
