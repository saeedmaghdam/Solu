using System.Threading;
using System.Threading.Tasks;

namespace Solu.Framework.Services.Object
{
    public interface IObjectService
    {
        Task<IObject> GetByIdAsync(string accountId, string id, CancellationToken cancellationToken);

        Task<string> CreateAsync(string accountId, byte[] content, string contentType, string hash, CancellationToken cancellationToken);
    }
}
