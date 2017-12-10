using CollegeUni.Services.Models;
using CollegeUni.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace CollegeUni.Api.Filters
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
            var result = new ServiceResult<LoginRequest> { Message = "Failed to generate token.", ModelState = ModelState.ToStringDictionary() };
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
            throw new ApiResponseException("Failed to generate token.", ModelState.ToStringDictionary(), statusCode);
            // Any uncaught exception is equivalent to the general InternalServerError: 500.
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
