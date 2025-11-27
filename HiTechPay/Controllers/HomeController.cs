using Microsoft.AspNetCore.Mvc;

namespace HiTechPay.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
