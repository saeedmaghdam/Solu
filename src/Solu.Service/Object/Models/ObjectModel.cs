using Solu.Framework.Services.Object;

namespace Solu.Service.Object.Models
{
    public class ObjectModel : Record, IObject
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
