using CollegeUni.Api.Managers;
using CollegeUni.Api.Models;
using Microsoft.AspNetCore.Identity;
using CollegeUni.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Services
{
    public interface IAuthService
    {
        Task<AuthServiceResult> ValidateUser(LoginViewModel model);
        Task<TokenResponseViewModel> GetJwtSecurityToken(AuthServiceResult serviceResult);
        Task<AuthServiceResult> RegisterUser(RegisterViewModel model);
    }

    public class AuthServiceResult
    {
        public ApplicationUser User { get; set; }
        public SignInResult UserSignIn { get; set; }
        public IdentityResult UserIdentity { get; set; }
    }
}
