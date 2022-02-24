using System;

namespace Solu.Framework.Shared
{
    public interface IJwtManager
    {
        string GenerateToken(string sessionId, string accountId, string name, string family, DateTime expirationDate);
    }
}
