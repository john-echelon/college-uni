using CollegeUni.Services.Managers;
using CollegeUni.Services.Models;
using CollegeUni.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace CollegeUni.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenManager _tokenManager;
        public AuthService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ITokenManager tokenManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenManager = tokenManager;
        }
      
        public async Task<TokenResponse> GetJwtSecurityToken(AuthServiceResult serviceResult)
        {
            if (serviceResult.UserSignIn.Succeeded && serviceResult.User != null)
            {
                var securityToken = await _tokenManager.GetJwtSecurityToken(serviceResult.User);
                return new TokenResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                    Expiration = securityToken.ValidTo
                };
            }
            return null;
        }
        public async Task<AuthServiceResult> ValidateUser(LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            var result = await _signInManager.CheckPasswordSignInAsync(user,
            model.Password, lockoutOnFailure: false);
            return new AuthServiceResult
            {
                User = user,
                UserSignIn = result
            };
        }
        public async Task<AuthServiceResult> RegisterUser(RegisterRequest model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var identityResult = await _userManager.CreateAsync(user, model.Password);
            return new AuthServiceResult
            {
                User = user,
                UserIdentity = identityResult
            };
        }
    }
}
