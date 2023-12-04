using ECommerce.DataAccess.Data;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Dbinitializer;

public class DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public void Initialize()
    {
        // Apply any pending migration
        if (_context.Database.GetPendingMigrations().Any())
        {
            _context.Database.Migrate();
        }

        // Create roles
        if (!_roleManager.RoleExistsAsync(SD.RoleCustomer).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.RoleEmployee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.RoleCompany)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@e-commerce.com",
                Email = "admin@e-commerce.com",
                Name = "admin",
                PhoneNumber = "",
                StreetAddress = "",
                State = "",
                PostalCode = "",
                City = "",
            }, "Admin.1234").GetAwaiter().GetResult();

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@e-commerce.com")!;
            _userManager.AddToRoleAsync(user, SD.RoleAdmin).GetAwaiter().GetResult();
        }
    }
}