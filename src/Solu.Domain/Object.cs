namespace Solu.Domain
{
    public class Object : BaseEntity
    {
        public string CreatorAccountId
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }
        
        public string Hash
        {
            get;
            set;
        }
    }
}
