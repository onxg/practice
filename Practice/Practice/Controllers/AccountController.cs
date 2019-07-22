using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Practice.Core.ViewModels;
using Practice.DAL.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                return View(model);


            var user = await _userManager.FindAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                model.Password = string.Empty;

                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            TempData["Toastr"] = new Toastr { Type = "success", Title = "Success", Message = "Successfully logged in." };

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            _signInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            TempData["Toastr"] = new Toastr { Type = "success", Title = "Success", Message = "You have been logged out." };

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

            if (!result.Succeeded)
                TempData["Toastr"] = new Toastr { Type = "error", Title = "Error", Message = result.Errors.First() };

            TempData["Toastr"] = new Toastr { Type = "success", Title = "Success", Message = "Your account is activated. You can log in now." };

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword() => View();
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasssword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return View("ForgotPasswordConfirmation");
                }
                string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                //code = HttpUtility.UrlEncode(code);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your passsword by clicking <a href=\'" + callbackUrl + "\'>here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View();
        }

        public ActionResult ForgotPasswordConfirmation() => View();

        [AllowAnonymous]
        public ActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordComfirmation");
            }
            // model.Code = HttpUtility.UrlDecode(model.Code);
            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordComfirmation");
            }
            AddModelErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordComfirmation()
        {
            return View();
        }

        public ActionResult UserPanel()
        {
            var model = new Practice.Core.ViewModels.UserPanel();

            if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.FindById(User.Identity.GetUserId());
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
            }

            return PartialView(model);
        }

        private void AddModelErrors(IdentityResult result) => result.Errors.ToList().ForEach(e => ModelState.AddModelError("", e));
    }
}