namespace Solu.Domain
{
    public class Account : BaseEntity
    {
        public string Username
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

        public bool IsAdmin
        {
            get;
            set;
        }

        public bool IsVerified
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
