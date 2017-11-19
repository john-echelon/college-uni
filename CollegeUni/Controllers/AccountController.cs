using CollegeUni.Models;
using CollegeUni.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolUni.Database.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace CollegeUni.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : CollegeUniBaseController
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ValidateUser(model);
            var token = await _authService.GetJwtSecurityToken(result);
            if (token != null)
            {
                return Ok(new TokenResponseViewModel
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            ModelState.AddModelError("Login", "Username or Password is invalid.");
            return BadRequest(ModelState);

        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterUser(model);
            var handledResult = HandleResult(result.UserIdentity);
            if (handledResult != null)
            {
                return handledResult;
            }
            _logger.LogInformation(3, "User registered an account.");
            var token = await _authService.GetJwtSecurityToken(result);
            if (token != null)
            {
                return Ok(new TokenResponseViewModel
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Ok(result);
        }
    }
}
