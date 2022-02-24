namespace Solu.Api.Controllers.Account.InputModels
{
    public class RegisterInputModel
    {
        public string MobileNumber
        {
            get;
            set;
        }

        public string Password
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

        public string VerificationCode
        {
            get;
            set;
        }
    }
}
