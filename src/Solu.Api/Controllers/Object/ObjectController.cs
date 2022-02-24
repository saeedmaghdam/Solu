using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Solu.Api.Controllers.Object.InputModels;
using Solu.Framework;
using Solu.Framework.Exceptions;
using Solu.Framework.Services.Object;
using Solu.Shared;
using Solu.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Solu.Api.Controllers.Object
{
    [Authorize]
    [ApiController]
    [Route("api/object")]
    public class ObjectController : ApiControllerBase
    {
        private readonly IObjectService _objectService;
        private readonly IOptionsMonitor<ApplicationOptions> _options;

        private static readonly string[] VALID_FILE_TYPES = { "application/pdf", "image/bmp", "image/png", "image/jpeg" };

        public ObjectController(IObjectService objectService, IOptionsMonitor<ApplicationOptions> options)
        {
            _objectService = objectService;
            _options = options;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResultViewModel<string>>> UploadAsync([FromForm] UploadInputModel model, CancellationToken cancellationToken)
        {
            if (model.File.Length > 0)
            {
                if (model.File.Length > _options.CurrentValue.MaximumUploadSizeInBytes)
                    throw new ValidationException("100", $"Upload maximum size is exceeded. Maximum object size is {_options.CurrentValue.MaximumUploadSizeInBytes / 1024} Kb");

                if (!VALID_FILE_TYPES.Contains(model.File.ContentType.ToLower()))
                    throw new ValidationException("100", "Unsupported file type.");

                using var fileStream = model.File.OpenReadStream();
                byte[] bytes = new byte[model.File.Length];
                fileStream.Read(bytes, 0, (int)model.File.Length);
                fileStream.Seek(0, SeekOrigin.Begin);
                using (var md5 = MD5.Create())
                {
                    var hashBytes = md5.ComputeHash(fileStream);
                    var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                    var result = await _objectService.CreateAsync(UserSession.AccountId, bytes, model.File.ContentType, hash, cancellationToken);

                    return OkData(result);
                }
            }
            else
            {
                throw new ValidationException("100", "File is invalid.");
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 31536000, Location = ResponseCacheLocation.Any)]
        public async Task<FileResult> DownloadAsync([FromRoute] string id, CancellationToken cancellationToken)
        {
            var fileToRetrieve = await _objectService.GetByIdAsync(UserSession.AccountId, id, cancellationToken);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}
