using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Practice.DAL.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Practice.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser, string> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser, string> signInManager)
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
            return RedirectToAction("Index","Home");
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
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                AddModelErrors(result);

            return View();
        }

        public ActionResult ForgotPassword() => View();

        private void AddModelErrors(IdentityResult result) => result.Errors.ToList().ForEach(e => ModelState.AddModelError("", e));
    }
}