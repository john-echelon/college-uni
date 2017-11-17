using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using CollegeUni.Models;
using Microsoft.AspNetCore.Identity;
using SchoolUni.Database.Models.Entities;
using CollegeUni.Managers;

namespace CollegeUni.Services
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
        public async Task<JwtSecurityToken> GetJwtSecurityToken(AuthServiceResult serviceResult)
        {
            if (serviceResult.UserSignIn.Succeeded)
            {
                return await _tokenManager.GetJwtSecurityToken(serviceResult.User);
            }
            return null;
        }
        public async Task<AuthServiceResult> ValidateUser(LoginViewModel model)
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
        public async Task<AuthServiceResult> RegisterUser(RegisterViewModel model)
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
