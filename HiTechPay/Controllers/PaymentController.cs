using Microsoft.AspNetCore.Mvc;

namespace HiTechPay.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
