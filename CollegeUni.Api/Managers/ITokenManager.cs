using CollegeUni.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Managers
{
    public interface ITokenManager
    {
        Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user);
    }
}
