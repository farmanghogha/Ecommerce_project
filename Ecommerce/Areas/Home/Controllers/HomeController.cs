using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Home.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
