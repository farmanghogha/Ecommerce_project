
using Ecommerce.Areas.User.Controllers;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Areas.Product.Controllers
{
    [Area("Product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;


        public ProductController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User", new { area = "User" });
            }

            var user = await _userManager.GetUserAsync(User);

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
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User", new { area = "User" });
            }

            if (id == null)
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
        public async Task<IActionResult> productPage(Productdata productdata, IFormFile file)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User", new { area = "User" });
            }



            if (file == null)
            {
                ViewBag.imagevalid = "Please choose image";
                return View();
            }

            string wwwRootPath = _hostEnvironment.WebRootPath;

            string filename = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwwRootPath, @"images\Products");
            var extension = Path.GetExtension(file.FileName);


            using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
            {
                file.CopyTo(fileStreams);
            }
            productdata.ImageUrl = @"\images\products\" + filename + extension;





            var data = await _userManager.GetUserAsync(User);

            productdata.CreatedBy = data.Id;
            productdata.IsActive = true;
            await _db.product.AddAsync(productdata);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //Edit Product
        [HttpPost]
        public IActionResult EditProduct(Productdata productdata, IFormFile? file)
        {
            if (file != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;


                if (productdata.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, productdata.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }


                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\Products");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                productdata.ImageUrl = @"\images\products\" + filename + extension;

            }


            

            _db.product.Update(productdata);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Delete Product
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {

            var data = _db.product.Find(id);

            string wwwRootPath = _hostEnvironment.WebRootPath;


            if (data.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, data.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

            }

            _db.product.Remove(data);
            _db.SaveChanges();

            return View("Index",_db.product.ToList());
        }
        //Add Discount
        public IActionResult AddDiscount(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User", new { area = "User" });
            }
            ViewBag.pid = id;
            return View();
        }
        [HttpPost]
        public IActionResult AddDiscount(Discount discount)
        {

            var product = _db.product.Find(discount.ProductId);

            if (discount.DiscountType == DiscountType.Amount)
            {
                product.DiscountAmount = discount.Amount;
            }
            else
            {
                if (discount.Amount > 100)
                {
                    ViewBag.adddisErroe = "percentage not valid Enter only 1 to 100";
                    @ViewBag.pid = discount.ProductId;
                    return View(discount);
                }
                product.DiscountAmount = (product.Price * discount.Amount) / 100;

            }
            _db.discount.Add(discount);
            _db.product.Update(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // status change Active or Deactive
        public IActionResult StatusUpdate(int? id)
        {
            var data = _db.product.Find(id);
            if (id != 0 || id != null)
            {
                if (data.IsActive == true)
                {
                    data.IsActive = false;
                }
                else
                {
                    data.IsActive = true;
                }
            }
            _db.product.Update(data);
            _db.SaveChanges();
            return RedirectToAction("Index", null, new { id = data.CreatedBy });
        }


        //[HttpGet]
        //public async Task<IActionResult> Search(string? name,int? id)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    var data = _db.product.Where(x => x.CreatedBy == user.Id);
        //    if (name == null)
        //    {

        //        return View("Index",data);
        //    }
        //    else if (id != null)
        //    {
        //        return View("index");
        //    }
        //    else
        //    {
        //      var serchdata=data.Where(x=>x.ProductName.Contains(name)).ToList();  
        //        return View("Index", serchdata);
        //    }

        //}
    }
}
