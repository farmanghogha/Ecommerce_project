using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Models.DashboardVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace Ecommerce.Areas.User.Controllers
{
    [Area("User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserController(ApplicationDbContext db,
                              UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager= roleManager;
            _signInManager = signInManager;
                
        }



        // Login for any User 


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            var data = await _userManager.FindByEmailAsync(login.Email);

            if (data == null)
            {
                ViewBag.validLogin = "Invalid Email And Password.....";
                return View();
            }
          

            var result = await _signInManager.PasswordSignInAsync(data, login.password, false, false);
            var role = await _userManager.GetRolesAsync(data);
            if(result.Succeeded && role.Any())
            {
                var currentRole=role.FirstOrDefault();
               // HttpContext.Response.Cookies.Append("Role", currentRole);
                HttpContext.Session.SetString("Role", currentRole);
                return RedirectToAction("Dashboard");
            }
           
            return View();
        }




        // signup for dealler only....


        public IActionResult SignUp()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDealler signUpDealler)
        {

            if(signUpDealler.password != signUpDealler.confo_password)
            {
                ViewBag.validSignup = "You have to enter Password and con_password must be match....";
                return View();
            }
            else{
               // var data = await _userManager.FindByEmailAsync(signUpDealler.email);
               var data=_db.dealer.FirstOrDefault(x=>x.Email==signUpDealler.email);

                if (data != null)
                {
                    ViewBag.validSignup = "User Already Exist......";
                    return View();
                }
                else
                {
                    Dealer dealer = new Dealer()
                    {
                        Email = signUpDealler.email,
                        Password = signUpDealler.password,
                        Phone = signUpDealler.phoneNo,
                        UserName = signUpDealler.userName,
                        status = (int)Status.pendding
                    };
                    _db.dealer.Add(dealer);
                    _db.SaveChanges();

                    //IdentityUser user = new IdentityUser()
                    //{
                    //    Email = signUpDealler.email,
                    //    PhoneNumber = signUpDealler.phoneNo,
                    //    UserName = signUpDealler.userName,

                    //};

                    //var result = await _userManager.CreateAsync(user, signUpDealler.password);

                    //if (result.Succeeded)
                    //{
                    //    if (!await _roleManager.RoleExistsAsync(RoleType.Dealer.ToString()))
                    //    {
                    //        await _roleManager.CreateAsync(new IdentityRole(RoleType.Dealer.ToString()));
                    //    }
                    //    await _userManager.AddToRoleAsync(user, RoleType.Dealer.ToString());
                    //}

                    return RedirectToAction("Login");
                    
                }
            }                 
          
        }


        // Dashboard for admin/dealler/superadmin

        [HttpGet]
        public async Task<IActionResult> Dashboard()
         {
            //ViewBag.rolecheck = HttpContext.Request.Cookies["Role"];
            ViewBag.rolecheck = HttpContext.Session.GetString("Role");

            dynamic GlobleModel = new ExpandoObject();
            GlobleModel.dealer = await _db.dealer.ToListAsync();
            GlobleModel.Admin = await _userManager.GetUsersInRoleAsync(RoleType.Admin.ToString());
            
           

            return View(GlobleModel);
          }


       // approve Dealler
        public async Task<IActionResult> Approve(string email)
        {
            var dealer = _db.dealer.FirstOrDefault(x => x.Email == email);

            IdentityUser user = new()
            {
                Email=dealer.Email,
                UserName=dealer.UserName,
                PhoneNumber=dealer.Phone,
                
            };
            var result = await _userManager.CreateAsync(user, dealer.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(RoleType.Dealer.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(RoleType.Dealer.ToString()));
                }
                await _userManager.AddToRoleAsync(user, RoleType.Dealer.ToString());
            }

            dealer.status = Status.Approve;
            _db.SaveChanges();


            SendMail sendMail = new SendMail()
            {
              From="admin@gmail.com",
              To="soumy.oza@technomark.io",
              Subject="Test mail",
              Body= "<h2>Congratulations <h2>"+"<br>"+ " <p> Your Request has been approve successfully......<p>"

            };

            MailController mailController=new MailController();
            mailController.Index(sendMail);
            return RedirectToAction("Dashboard");
        }
        
        
        // Reject Dealler


        [HttpPost]
        public IActionResult Reject(string email,string reason)
        {
            var data = _db.dealer.FirstOrDefault(x => x.Email == email);

            data.status = Status.Reject;
            data.reason = reason;

            _db.dealer.Update(data);
            _db.SaveChanges();
            SendMail sendMail = new SendMail()
            {
                From = "admin@gmail.com",
                To = "soumy.oza@technomark.io",
                Subject = "Test mail",
                Body = "<h2>Soryy.... <h2>" + " <br> " + "<p> Your Request has been reject ......<p>"

            };

            MailController mailController = new MailController();
            mailController.Index(sendMail);

            return RedirectToAction(nameof(Dashboard));
        }




        [HttpGet]
        public IActionResult Popup(string email)
        {
            ViewBag.email = email;
            return PartialView("_Popup");
        }

        public IActionResult Block(string email)
        {


            return RedirectToAction("Dashboard");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
