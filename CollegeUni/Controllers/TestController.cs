﻿using CollegeUni.Filters;
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
            ModelState.AddModelError("Login", "Token request has failed.");
            //var result = new ServiceResult { Message = "Failed to generate token.", ModelState = ModelState };
            var result = new ServiceResult<LoginViewModel>
            {
                Message = "Failed to generate token.",
                Data = new LoginViewModel { Email = "mbriggs@example.com" }
            };
            return BadRequest(result);
        }
        [HttpGet("notfound")]
        public IActionResult NotFoundAction()
        {
            return NotFound();
        }
        [HttpGet("exception")]
        public IActionResult ThrowExceptionAction()
        {
            ModelState.AddModelError("Login", "Token request has failed.");
            throw new CustomException("Failed to generate token.", ModelState);
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
