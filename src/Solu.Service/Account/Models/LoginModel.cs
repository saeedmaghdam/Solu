using System;
using Solu.Framework.Services.Account;

namespace Solu.Service.Account.Models
{
    public class LoginModel : ILogin
    {
        public string Token
        {
            get;
            set;
        }

        public bool IsSuccessful
        {
            get;
            set;
        }

        public DateTime ExpirationDate
        {
            get;
            set;
        }
    }
}
