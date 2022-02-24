namespace Solu.Api.Controllers.Account.InputModels
{
    public class ResetPasswordInputModel
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

        public string VerificationCode
        {
            get;
            set;
        }
    }
}
