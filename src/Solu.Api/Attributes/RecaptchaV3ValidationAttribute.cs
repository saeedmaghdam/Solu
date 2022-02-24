using System;
using System.Net.Http;
using System.Threading.Tasks;
using Solu.Framework;
using Solu.Framework.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Solu.Api.Attributes
{
    public class RecaptchaV3ValidationAttribute : ActionFilterAttribute
    {
        private class CaptchaVerificationResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }
            [JsonProperty("challenge_ts")]
            public DateTime ChallengeDateTime { get; set; }
            [JsonProperty("hostname")]
            public string Hostname { get; set; }
            [JsonProperty("score")]
            public double Score { get; set; }
            [JsonProperty("action")]
            public string Action { get; set; }
        }

        private readonly IOptionsMonitor<ApplicationOptions> _optionsMonitor;

        public RecaptchaV3ValidationAttribute(IOptionsMonitor<ApplicationOptions> optionsMonitor) => _optionsMonitor = optionsMonitor;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next.Invoke();
            // var recaptchaToken = context.HttpContext.Request.Headers["RecaptchaToken"];
            // if (string.IsNullOrEmpty(recaptchaToken))
            //     throw new ValidationException("100", "Recatpcha token not found.");

            // var isRecaptchaValid = await IsCaptchaValid(recaptchaToken.ToString());

            // if (!isRecaptchaValid)
            //     throw new ValidationException("100", "ReCaptcha is not valid.");

            // await next.Invoke();
        }

        public async Task<bool> IsCaptchaValid(string token)
        {
            var result = false;

            var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

            try
            {
                using var client = new HttpClient();

                var response = await client.PostAsync($"{googleVerificationUrl}?secret={_optionsMonitor.CurrentValue.ReCaptchaSecretKey}&response={token}", null);
                var jsonString = await response.Content.ReadAsStringAsync();
                var captchaVerfication = JsonConvert.DeserializeObject<CaptchaVerificationResponse>(jsonString);

                result = captchaVerfication.Success;
            }
            catch (Exception e)
            {
                throw new ValidationException("100", e.Message);
            }

            return result;
        }
    }
}
