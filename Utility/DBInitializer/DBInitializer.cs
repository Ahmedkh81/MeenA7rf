using DataAccess.Data;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public DBInitializer(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            if(_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.SuperAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Player)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "SuperAdmin",
                    Email = "Superadmin@ahkh.com",
                    EmailConfirmed = true
                }, "Admin123+").GetAwaiter().GetResult();

                var user = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.SuperAdmin).GetAwaiter().GetResult();
                }
            }
        }
    }
}
