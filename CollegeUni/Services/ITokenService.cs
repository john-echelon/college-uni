using SchoolUni.Database.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Services
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user);
    }
}
