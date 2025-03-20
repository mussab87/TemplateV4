using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Infrastructure.UserService
{ }
public class UserHelper
{
    private readonly AppDbContext _dbContext;
    private readonly RoleManager<Role> _roleManager;

    public UserHelper(AppDbContext dbContext, RoleManager<Role> roleManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
    }

    public async Task AddClaimsToRole(User user, Role role, List<Claim> claims)
    {
        var roleClaims = new List<RoleClaim>();
        foreach (var clm in claims)
        {
            var roleClaim = new RoleClaim
            {
                RoleId = role.Id,
                ClaimType = clm.Value,
                ClaimValue = clm.Value,
                CreatedById = user.Id
            };
            roleClaims.Add(roleClaim);
        }
        // Add list of claims to role
        if (roleClaims.Count > 0)
        {
            await _dbContext.RoleClaims.AddRangeAsync(roleClaims);
            await _dbContext.SaveChangesAsync();
        }
    }
}

