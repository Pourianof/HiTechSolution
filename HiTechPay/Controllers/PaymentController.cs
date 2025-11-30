using System.Web;

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
            var specifiedCallbackURL = HttpContext.Request.Query["callback"];
            var key = HttpContext.Request.Query["key"];

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(specifiedCallbackURL))
            {
                var signedKey = _signer.Sign(key!);
                var callbackUrl = new UriBuilder(specifiedCallbackURL!);

                var query = HttpUtility.ParseQueryString(callbackUrl.Query);

                query["confirm_key"] = signedKey;

                callbackUrl.Query = query.ToString();
                string finalCallbackUrl = callbackUrl.ToString();

                ViewBag.CallbackURL = finalCallbackUrl;

            }

            return View();
        }
    }
}
