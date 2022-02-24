using System.Threading;
using System.Threading.Tasks;

namespace Solu.Framework.Shared
{
    public interface INotificationHandler
    {
        Task SendVerificationCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken);
    }
}
