using MongoDB.Bson;

namespace Solu.Domain
{
    public class Identify
    {
        private string _id;

        public Identify()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _id = ObjectId.GenerateNewId().ToString();
                else
                    _id = value;
            }
        }
    }
}
