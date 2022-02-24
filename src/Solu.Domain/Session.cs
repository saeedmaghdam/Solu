using System;

namespace Solu.Domain
{
    public class Session : BaseEntity
    {
        public string AccountId
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public string SessionId
        {
            get;
            set;
        }

        public DateTime ExpirationDate
        {
            get;
            set;
        }

        public bool IsValid
        {
            get;
            set;
        }
    }
}
