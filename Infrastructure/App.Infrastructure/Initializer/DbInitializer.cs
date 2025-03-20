using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Infrastructure.Mail.Initializer { }

public class DbInitializer : IDbInitializer
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public DbInitializer(AppDbContext db, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async void InitializeAsync()
    {
        try
        {
            // Apply pending migrations before seeding data
            if (_db.Database.GetPendingMigrations().Any())
            {
                await _db.Database.MigrateAsync();
            }

            //Check role SuperAdmin exist or no
            if (!await _roleManager.RoleExistsAsync(Roles.SuperAdmin))
            {
                //In case SuperAdmin role not Exist
                //system run at first time - Create SuperAdmin role and Admin user
                var roleToAdd = new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Roles.SuperAdmin,
                    RoleNameArabic = "مدير النظام",
                    CreatedById = "System Super Admin"
                };
                var newRole = await _roleManager.CreateAsync(roleToAdd);

                //Create User admin and link with the Role was created a top SuperAdmin
                var adminUser = await CreateAdminUser();
                //var userNew = _userManager.FindByIdAsync(adminUser.Id).Result;

                var role = await _roleManager.FindByNameAsync(Roles.SuperAdmin);
                if (role != null)
                {
                    var allPermissions = GetAllClaimsPermissions.GetAllControllerActionsUpdated();
                    UserHelper userHelper = new(_db, _roleManager);
                    await userHelper.AddClaimsToRole(adminUser, role, allPermissions);
                }
            }
            else
            {
                return;
            }
        }
        catch (Exception)
        {
        }
    }

    async Task<User> CreateAdminUser()
    {
        User adminUser = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin",
            Email = "admin@admin.com",
            EmailConfirmed = true,
            PhoneNumber = "111111111111",
            FirstName = "admin",
            LastName = "test",
            UserStatus = true,
            FirstLogin = true,
            IsActive = true,
            CreatedBy = "System Super Admin"
        };

        await _userManager.CreateAsync(adminUser, "Aa@123456");
        await _userManager.AddToRoleAsync(adminUser, Roles.SuperAdmin);
        return adminUser;
    }
}

