using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Solu.Api.ViewModels
{
    [DataContract]
    public class ErrorViewModel : ActionResult
    {
        [DataMember(Name = "error_code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "error_description")]
        public string ErrorDescription { get; set; }
    }
}
