using CollegeUni.Filters;
using CollegeUni.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CollegeUni.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        [HttpGet("ok")]
        public IActionResult OkAction()
        {
            return Ok();
        }
        [HttpGet("unauthorized")]
        public IActionResult UnAuthorizedAction()
        {
            return Unauthorized();
        }
        [HttpGet("forbid")]
        public IActionResult ForbidAction()
        {
            return Forbid();
        }
        [HttpGet("bad")]
        public IActionResult BadRequestAction()
        {
            return BadRequest();
        }
        [HttpGet("badresult")]
        public IActionResult BadRequestResultAction()
        {
            // Demonstrates expection handling here. See also ApiExceptionFilter
            ModelState.AddModelError("Login", "Token request has failed.");
            var result = new ServiceResult<LoginViewModel> { Message = "Failed to generate token.", ModelState = ModelState };
            return BadRequest(result);
        }
        [HttpGet("notfound")]
        public IActionResult NotFoundAction()
        {
            return NotFound();
        }
        [HttpGet("exception")]
        public IActionResult ThrowExceptionAction(int statusCode = 500)
        {
            ModelState.AddModelError("Login", "Token request has failed.");
            throw new CustomException("Failed to generate token.", ModelState, statusCode);
            // Any uncaught exception is equivalent to the general InternalServerError: 500.
        }
        [HttpGet("badgateway")]
        public IActionResult BadGateway()
        {
            // Status Code 502.3
            return BadGateway();
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
