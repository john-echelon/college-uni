using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using CollegeUni.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace CollegeUni.Api.Managers
{
    public class TokenManager : ITokenManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public TokenManager(UserManager<ApplicationUser> userManager, IConfiguration configuration) {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            string domain = $"https://{_configuration["Auth0:Domain"]}/";
            string apiIdentifier = _configuration["Auth0:ApiIdentifier"];

            return new JwtSecurityToken(
                issuer: domain,
                audience: domain,
                claims: GetTokenClaims(user).Union(userClaims),
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiIdentifier)), SecurityAlgorithms.HmacSha256)
            );
        }
        private static IEnumerable<Claim> GetTokenClaims(ApplicationUser user)
        {
            return new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                };
        }
 
    }
}