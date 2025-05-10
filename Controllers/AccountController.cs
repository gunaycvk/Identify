using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{

    public class AccountController : Controller
    {
        private UserManager<AppUser> _usermanager;
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _usermanager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _usermanager.ResetAccessFailedCountAsync(user);
                        await _usermanager.SetLockoutEndDateAsync(user, null);

                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutDate = await _usermanager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutDate.Value - DateTime.UtcNow;
                        ModelState.AddModelError("", $"Hesabınız kilitlendi, lütfen {timeLeft.Minutes} dakika sonra tekrar deneyiniz.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hatalı e-posta adresi veya parola.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı e-posta adresi veya parola.");
                }
            }

            return View();
        }

    }
}