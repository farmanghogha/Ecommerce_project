
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Product.Controllers
{
    [Area("Product")]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult productPage()
        {
            return View();
        }


        [HttpPost]
        public IActionResult AddProduct(Models.Productdata product)
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EditProduct(string id)
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteProduct(string id)
        {
            return RedirectToAction("Index");
        }
    }
}
