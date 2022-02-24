using System.Threading;
using System.Threading.Tasks;

namespace Solu.Framework.Services.Account
{
    public interface IAccountService
    {
        Task<IAccount> GetAccountByToken(string token, CancellationToken cancellationToken);
        Task<string> CreateAsync(string accountId, string username, string password, string name, string family, CancellationToken cancellationToken);
        Task<ILogin> LoginAsync(string username, string password, CancellationToken cancellationToken);
        Task LogoutAsync(string token, CancellationToken cancellationToken);
        Task<bool> IsAuthenticated(string token, CancellationToken cancellationToken);
        Task RegisterVerificationCodeAsync(string mobileNumber, CancellationToken cancellationToken);
        Task RegisterAsync(string mobileNumber, string password, string name, string family, string verificationCode, CancellationToken cancellationToken);
        Task ResetPasswordVerificationCodeAsync(string mobileNumber, CancellationToken cancellationToken);
        Task ResetPasswordAsync(string mobileNumber, string password, string verificationCode, CancellationToken cancellationToken);
    }
}
