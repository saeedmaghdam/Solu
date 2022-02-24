namespace Solu.Framework.Services.Object
{
    public interface IObject : IRecord
    {
        string CreatorAccountId
        {
            get;
            set;
        }

        byte[] Content
        {
            get;
            set;
        }

        string ContentType
        {
            get;
            set;
        }

        string Hash
        {
            get;
            set;
        }
    }
}
