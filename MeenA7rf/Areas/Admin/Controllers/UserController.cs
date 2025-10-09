using Entities.Models;
using Entities.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utility.DBInitializer;

namespace MeenA7rf.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Admin()
        {
            var users = _userManager.Users.ToList();

            var admins = new List<ApplicationUser>();

            foreach (var admin in users)
            {
                var roles = await _userManager.GetRolesAsync(admin);

                if (roles.Contains(SD.SuperAdmin))
                    admins.Add(admin);
            }

            return View(admins);
        }

        public async Task<IActionResult> Player()
        {
            var users = _userManager.Users.ToList();

            var players = new List<ApplicationUser>();

            foreach (var player in users)
            {
                var roles = await _userManager.GetRolesAsync(player);

                if (roles.Contains(SD.Player))
                    players.Add(player);
            }

            return View(players);
        }

        public IActionResult Create(string returnTo)
        {
            var vm = new ManageUserVM()
            {
                Roles = _roleManager.Roles.Select(e => new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList()
            };

            ViewBag.ReturnTo = returnTo;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ManageUserVM vm, List<string> roles, string returnTo)
        {
            if (!ModelState.IsValid)
            {
                vm.Roles = _roleManager.Roles.Select(e => new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList();
                ViewBag.ReturnTo = returnTo;
                return View(vm);
            }

            var user = new ApplicationUser()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                UserName = vm.UserName,
                EmailConfirmed = vm.IsEmailConfirmed
            };
            var result = await _userManager.CreateAsync(user, "Test123+");
            if (result.Succeeded) 
            {
                await _userManager.AddToRolesAsync(user, roles);
                TempData["success-notification"] = "User created successfully";

                if (string.Equals(returnTo, "Player", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Player");
                else
                    return RedirectToAction("Admin");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            vm.Roles = _roleManager.Roles.Select(e => new SelectListItem()
            {
                Text = e.Name,
                Value = e.Name
            }).ToList();

            ViewBag.ReturnTo = returnTo;
            return View(vm);
        }

        public async Task<IActionResult> Edit(string id, string returnTo)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            var vm = user.Adapt<ManageUserVM>();
            vm.IsEmailConfirmed = user.EmailConfirmed;

            var userRoles = await _userManager.GetRolesAsync(user);
            vm.Roles = _roleManager.Roles.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Name,
                Selected = userRoles.Contains(e.Name!)
            }).ToList();

            ViewBag.ReturnTo = returnTo;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManageUserVM vm, List<string> roles, string returnTo)
        {
            if (!ModelState.IsValid)
            {
                vm.Roles = _roleManager.Roles.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList();

                ViewBag.ReturnTo = returnTo;
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(vm.Id!);
            if (user is null)
                return NotFound();

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.UserName = vm.UserName;
            user.EmailConfirmed = vm.IsEmailConfirmed;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                vm.Roles = _roleManager.Roles.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList();

                ViewBag.ReturnTo = returnTo;
                return View(vm);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, roles);

            TempData["success-notification"] = "User updated successfully";

            return RedirectToAction(returnTo);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {
                await _userManager.DeleteAsync(user);

                TempData["success-notification"] = "User Deleted Successfully";

                return RedirectToAction(nameof(Player));
            }

            return NotFound();
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = DateTime.UtcNow.AddMonths(1);
                    TempData["success-notification"] = "Block User Successfully";
                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = null;
                    TempData["success-notification"] = "UnBlock User Successfully";
                }

                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Player));
            }

            return NotFound();
        }
    }
}
