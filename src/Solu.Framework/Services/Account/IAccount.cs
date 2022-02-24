namespace Solu.Framework.Services.Account
{
    public interface IAccount : IRecord
    {
        string Username
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        string Family
        {
            get;
            set;
        }

        bool IsAdmin
        {
            get;
            set;
        }
    }
}
