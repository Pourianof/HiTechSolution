using System.Web;

using HiTechPay.Sdk.Communication;
using HiTechPay.Services;

using Microsoft.AspNetCore.Mvc;

namespace HiTechPay.Controllers
{
    public class PaymentController(ISignerService signerService, IConfiguration configuration) : Controller
    {
        private IConfiguration _configuration = configuration;
        private ISignerService _signer = signerService;

        public IActionResult Index()
        {
            var specifiedCallbackURL = HttpContext.Request.Query[ConnectionQueryStrings.CallbackUrl];
            var key = HttpContext.Request.Query[ConnectionQueryStrings.Key];

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(specifiedCallbackURL))
            {
                var signedKey = _signer.Sign(key!);
                var callbackUrl = new UriBuilder(specifiedCallbackURL!);

                var query = HttpUtility.ParseQueryString(callbackUrl.Query);

                query[ConnectionQueryStrings.ConfirmKey] = signedKey;
                query[ConnectionQueryStrings.Key] = key;

                callbackUrl.Query = query.ToString();
                string finalCallbackUrl = callbackUrl.ToString();

                ViewBag.CallbackURL = finalCallbackUrl;

            }

            return View();
        }
    }
}
