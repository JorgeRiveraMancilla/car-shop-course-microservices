using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
    public class CustomProfileService(UserManager<ApplicationUser> userManager) : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user =
                await _userManager.GetUserAsync(context.Subject)
                ?? throw new ArgumentException("User not found");

            var existingClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim> { new("username", user.UserName!), };

            context.IssuedClaims.AddRange(claims);
            context.IssuedClaims.Add(
                existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)!
            );
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
