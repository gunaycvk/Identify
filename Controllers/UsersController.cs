using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityApp.Controllers
{

    public class UsersController : Controller
    {

        private UserManager<AppUser> _usermanager;
        private RoleManager<AppRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _usermanager = userManager;
            _roleManager = roleManager;
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
                    UserName = "user" + new Random().Next(1, 999999),
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

                ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();

                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    SelectedRoles = await _usermanager.GetRolesAsync(user)

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

                        await _usermanager.RemoveFromRolesAsync(user, await _usermanager.GetRolesAsync(user));
                        if (model.SelectedRoles != null)
                        {
                            await _usermanager.AddToRolesAsync(user, model.SelectedRoles);

                        }
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