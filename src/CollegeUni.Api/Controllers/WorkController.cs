using CollegeUni.Services.Models;
using CollegeUni.Services.Services;
using CollegeUni.Services.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollegeUni.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class WorkController : CollegeUniBaseController
    {
        private readonly ICourseService _courseService;
        private readonly ICommandHandler<CourseWorkCommand> _handler;
        public WorkController(ICourseService courseService, ICommandHandler<CourseWorkCommand> handler)
        {
            _courseService = courseService;
            _handler = handler;
        }
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResult<CourseResponse>), 200)]
        public async Task<IActionResult> CourseWork([FromBody]CourseRequest value)
        {
            var cmd = new CourseWorkCommand { Response = new CourseResponse() };
            _handler.Handle(cmd);
            var result = new ServiceResult<CourseResponse> { Data = cmd.Response };
            return Ok(result);
        }
    }
}