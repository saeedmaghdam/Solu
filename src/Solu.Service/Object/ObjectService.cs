using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Solu.Framework;
using Solu.Framework.Constants;
using Solu.Framework.Exceptions;
using Solu.Framework.Services.Object;
using Solu.Service.Object.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Solu.Service.Object
{
    public class ObjectService : IObjectService
    {
        private readonly IMongoCollection<Domain.Object> _objectCollection;
        private readonly IMongoCollection<Domain.Account> _accountCollection;

        public ObjectService(IOptionsMonitor<ApplicationOptions> options)
        {
            var mongoClient = new MongoClient(
            options.CurrentValue.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                options.CurrentValue.DatabaseName);

            _objectCollection = mongoDatabase.GetCollection<Domain.Object>("Object");
            _accountCollection = mongoDatabase.GetCollection<Domain.Account>("Account");
        }

        public async Task<IObject> GetByIdAsync(string accountId, string id, CancellationToken cancellationToken)
        {
            var account = await _accountCollection.Find(x => x.Id == accountId).FirstOrDefaultAsync(cancellationToken);
            if (account == null)
                throw new ValidationException("100", "Account not found.");

            var @object = await _objectCollection.Find(x => x.CreatorAccountId == accountId && x.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (@object == null)
                throw new ValidationException("100", "Object not found.");

            return ToModel(new[] { @object }).First();
        }

        public async Task<string> CreateAsync(string accountId, byte[] content, string contentType, string hash, CancellationToken cancellationToken)
        {
            var account = await _accountCollection.Find(x => x.Id == accountId).FirstOrDefaultAsync(cancellationToken);
            if (account == null)
                throw new ValidationException("100", "Account not found.");

            var @object = await _objectCollection.Find(x => x.Hash == hash).FirstOrDefaultAsync(cancellationToken);
            if (@object != null)
                return @object.Id;

            var newObject = new Domain.Object()
            {
                RecordInsertDate = DateTime.Now,
                RecordLastEditDate = DateTime.Now,
                RecordStatus = RecordStatus.Inserted,
                Content = content,
                CreatorAccountId = accountId,
                Hash = hash,
                ContentType = contentType
            };

            await _objectCollection.InsertOneAsync(newObject, new InsertOneOptions(), cancellationToken);

            return newObject.Id;
        }

        public IEnumerable<IObject> ToModel(IEnumerable<Domain.Object> objects)
        {
            return objects.Select(x => new ObjectModel()
            {
                RecordStatus = x.RecordStatus,
                Content = x.Content,
                ContentType = x.ContentType,
                CreatorAccountId = x.CreatorAccountId,
                Hash = x.Hash,
                Id = x.Id,
                RecordInsertDate = x.RecordInsertDate,
                RecordLastEditDate = x.RecordLastEditDate
            });
        }
    }
}
