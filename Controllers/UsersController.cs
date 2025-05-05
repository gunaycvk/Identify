using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityApp.Controllers
{

    public class UsersController : Controller
    {

        private UserManager<AppUser> _usermanager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _usermanager = userManager;
        }

        public IActionResult Index()
        {
            return View(_usermanager.Users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    FullName = model.FullName
                };
                IdentityResult result = await _usermanager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }


            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return RedirectToAction("Index");

            }

            var user = await _usermanager.FindByIdAsync(id);

            if (user != null)
            {

                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,

                });

            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model)
        {

            if (id != @model.Id)
            {
                return RedirectToAction("Index");

            }

            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.FullName = model.FullName;

                    var result = await _usermanager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");

                    }

                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }


            }

            return View(model);

        }

        [HttpPost]

        public async Task<IActionResult> Delete(string id)
        {

            var user = await _usermanager.FindByIdAsync(id);

            if (user != null)
            {

                await _usermanager.DeleteAsync(user);
            }
            return RedirectToAction("Index");

        }
    }
}