using Solu.Shared;
using Solu.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Solu.Api.Controllers
{
    public class ApiControllerBase : Controller
    {
        public UserSessionModel UserSession => (UserSessionModel)HttpContext.Items["UserSession"];

        public OkObjectResult OkData<TData>(TData data, object meta = null)
        {
            return Ok(ApiResultViewModel<TData>.FromData(data, meta));
        }
    }
}
