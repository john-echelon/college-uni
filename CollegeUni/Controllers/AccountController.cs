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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            var result = await _signInManager.CheckPasswordSignInAsync(user,
            model.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var token = await _tokenService.GetJwtSecurityToken(user);

                return Ok(new
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
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            _logger.LogInformation(3, "User created a new account with password.");
            var handledResult = HandleResult(result);
            if (handledResult != null)
            {
                return handledResult;
            }
            // Deprecated
            //await _signInManager.SignInAsync(user, isPersistent: false);
            //_logger.LogInformation(3, "User signed in with a new account.");
            return Ok(result);
        }

        /*
        [System.Obsolete]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, "User logged in.");
                return Ok();

            }
            return BadRequest();
        }

        [System.Obsolete]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok();
        }
        */
    }
}
