using System;

namespace Solu.Framework.Services.Session
{
    public interface ISession : IRecord
    {
        string AccountId
        {
            get;
            set;
        }

        string Token
        {
            get;
            set;
        }

        string SessionId
        {
            get;
            set;
        }

        DateTime ExpirationDate
        {
            get;
            set;
        }

        bool IsValid
        {
            get;
            set;
        }
    }
}
