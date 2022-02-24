using Solu.Framework.Services.Account;

namespace Solu.Service.Account.Models
{
    public class AccountModel : Record, IAccount
    {
        public string Username
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Family
        {
            get;
            set;
        }

        public bool IsAdmin
        {
            get;
            set;
        }
    }
}
