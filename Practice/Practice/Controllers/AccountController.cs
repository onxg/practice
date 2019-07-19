using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Practice.Core.ViewModels;
using Practice.DAL.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Practice.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser, string> _userManager;
        private readonly SignInManager<ApplicationUser, string> _signInManager;

        public AccountController(UserManager<ApplicationUser, string> userManager, SignInManager<ApplicationUser, string> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Core.ViewModels.Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindAsync(model.Email, model.Password);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
            TempData["LoggedIn"] = "Successfully logged in.";

            return RedirectToAction("Index","Home");
        }

        public ActionResult LogOff()
        {
            _signInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            TempData["LoggedIn"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Core.ViewModels.Register model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddModelErrors(result);

                return View();
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.RouteUrl("ActivateAccount", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
            await _userManager.SendEmailAsync(user.Id, "Activate your account", "Please activate your account by clicking here: " + callbackUrl + "");

            TempData["Toastr"] = new Toastr { Type = "success", Title = "Success", Message = "Your account has been created. You have to activate your account before you could log in." };

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> ActivateAccount(string userId, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(userId, code);

            if(!result.Succeeded)
                TempData["Toastr"] = new Toastr { Type = "error", Title = "Error", Message = result.Errors.First() };

            TempData["Toastr"] = new Toastr { Type = "success", Title = "Success", Message = "Your account is activated. You can log in now." };

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword() => View();

        private void AddModelErrors(IdentityResult result) => result.Errors.ToList().ForEach(e => ModelState.AddModelError("", e));
    }
}