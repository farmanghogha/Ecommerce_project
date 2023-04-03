
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Product.Controllers
{
    [Area("Product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public ProductController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
      
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);

            var role =  HttpContext.Session.GetString("Role");

            if(role== RoleType.Dealer.ToString())
            {
                var data = _db.product.Where(x => x.CreatedBy == user.Id).ToList();
                return View(data);
            }
            else if (role == RoleType.Admin.ToString() || role == RoleType.SuparAdmin.ToString())
            {
                return View(_db.product.ToList());
            }
            else
            {
                return View();
            }
            
          
        }

        [HttpGet]
        public IActionResult productPage(int? id)
        {
            if(id== null)
            {
                return View();
            }
            else
            {
                var data = _db.product.Find(id);
              return View(data);

            }
        }


        [HttpPost]
        public async Task<IActionResult> productPage(Productdata productdata)
        {
            
           var data=await _userManager.GetUserAsync(User);

            productdata.CreatedBy = data.Id;

           await _db.product.AddAsync(productdata);
           await _db.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EditProduct(Productdata productdata)
        {
            _db.product.Update(productdata);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteProduct(string id)
        {
            return RedirectToAction("Index");
        }
    }
}
