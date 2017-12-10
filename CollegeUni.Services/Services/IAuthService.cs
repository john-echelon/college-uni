using CollegeUni.Services.Managers;
using CollegeUni.Services.Models;
using Microsoft.AspNetCore.Identity;
using CollegeUni.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Services.Services
{
    public interface IAuthService
    {
        Task<AuthServiceResult> ValidateUser(LoginRequest model);
        Task<TokenResponse> GetJwtSecurityToken(AuthServiceResult serviceResult);
        Task<AuthServiceResult> RegisterUser(RegisterRequest model);
    }

    public class AuthServiceResult
    {
        public ApplicationUser User { get; set; }
        public SignInResult UserSignIn { get; set; }
        public IdentityResult UserIdentity { get; set; }
    }
}
