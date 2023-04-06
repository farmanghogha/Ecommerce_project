
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
        public async Task<IActionResult> Index(string? id)
        {

           
            var user =await _userManager.GetUserAsync(User);

            var role = await _userManager.GetRolesAsync(user);
            var currentrole = role.FirstOrDefault();
            ViewBag.roletype = currentrole;


            if (id == null)
            {
                var userd = await _userManager.GetUserAsync(User);
                ViewBag.UserName = user.Email;
                              
                var data = _db.product.Where(x => x.CreatedBy == userd.Id).ToList();
                return View(data);
                
            }
            else if (id != null)
            {
                if (id.Contains("@"))
                {
                    var userdata = await _userManager.FindByEmailAsync(id);
                    var data = _db.product.Where(x => x.CreatedBy == userdata.Id).ToList();
                    return View(data);
                }
                else
                {
                    var userdata = await _userManager.FindByIdAsync(id);
                    var data = _db.product.Where(x => x.CreatedBy == userdata.Id).ToList();
                    return View(data);
                }
                   
               
               
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

        // Add Product
        [HttpPost]
        public async Task<IActionResult> productPage(Productdata productdata)
        {
            
           var data=await _userManager.GetUserAsync(User);

            productdata.CreatedBy = data.Id;

           await _db.product.AddAsync(productdata);
           await _db.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
        
        //Edit Product
        [HttpPost]
        public IActionResult EditProduct(Productdata productdata)
        {
            _db.product.Update(productdata);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Delete Product
        public IActionResult DeleteProduct(int id)
        {
            var data=_db.product.Find(id);
            _db.product.Remove(data);
             _db.SaveChanges();

            return RedirectToAction("Index");
        }
        //Add Discount
        public IActionResult AddDiscount(int id)
        {
            ViewBag.pid = id;
            return View();
        }
        [HttpPost]
        public IActionResult AddDiscount(Discount discount)
        {
           _db.discount.Add(discount);

            var product = _db.product.Find(discount.ProductId);

            if(discount.DiscountType==DiscountType.Amount)
            {
                product.DiscountAmount = discount.Amount;
            }
            else
            {
                product.DiscountAmount = (product.Price * discount.Amount) / 100 ;

            }
            _db.product.Update(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        // status change Active or Deactive
        public IActionResult StatusUpdate(int id)
        {
            var data = _db.product.Find(id);
            if(id!=0 || id != null)
            {
                if(data.IsActive==true)
                {
                    data.IsActive= false;
                }
                else
                {
                    data.IsActive=true;
                }
            }
            _db.product.Update(data);
            _db.SaveChanges();
           return RedirectToAction("Index", null, new { id = data.CreatedBy});
        }
    }
}
