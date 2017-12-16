using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollegeUni.Api.Controllers
{
    [Route("api/[controller]")]
    public abstract class CollegeUniBaseController : Controller
    {
        protected IActionResult HandleResult(IdentityResult result)
        {
            if (result == null)
            {
                return StatusCode(500);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    AddErrors(result);
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

    }
}
