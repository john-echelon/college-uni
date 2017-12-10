using CollegeUni.Api.Models;
using CollegeUni.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CollegeUni.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace CollegeUni.Api.Controllers
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
            if (!result.UserSignIn.Succeeded)
            {
                ModelState.AddModelError("Login", "Username or Password is invalid.");
                _logger.LogInformation(3, string.Format("Sign in Failed. User {0}", model.Email));
                return BadRequest(ModelState);
            }
            var token = await _authService.GetJwtSecurityToken(result);
            if (token != null)
            {
                return Ok(token);
            }
            ModelState.AddModelError("Login", "Login Failed.");
            _logger.LogError(3, string.Format("Token Generation failed. User {0}", model.Email));
            return BadRequest();
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
            return Ok();
        }
    }
}
