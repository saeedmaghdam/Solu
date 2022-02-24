using Microsoft.AspNetCore.Http;

namespace Solu.Api.Controllers.Object.InputModels
{
    public class UploadInputModel
    {
        public IFormFile File
        {
            get;
            set;
        }

        public string MetaData
        {
            get;
            set;
        }
    }
}
