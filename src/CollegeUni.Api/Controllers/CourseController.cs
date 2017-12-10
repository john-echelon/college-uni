using CollegeUni.Services.Models;
using CollegeUni.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollegeUni.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CourseController : CollegeUniBaseController
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        // GET: api/values
        /// <summary>
        /// Returns courses.
        /// </summary>
        /// <param name="offset">The offset determines the number of courses to skip. Set to 0 to skip none.</param>
        /// <param name="limit">Limits the number of course items retrieved.</param>
        /// <param name="studentID">An optional filter criteria to filter courses by student id.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResult<BrowseResponse<CourseResponse>>), 200)]
        public async Task<IActionResult> Get(int offset, int limit, int? studentID = null)
        {
            var result = await _courseService.GetCourses(
                new CourseBrowseRequest
                {
                    StudentId = studentID,
                    PageInfo = new PageMeta
                    {
                        Offset = offset,
                        Limit = limit
                    }
                });
            return Ok(result);
        }

        // GET api/values/5
        /// <summary>
        /// Returns a course by the course id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceResult<CourseResponse>), 200)]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _courseService.GetCourse(id);
            return Ok(result);
        }

        // POST api/values
        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResult<CourseResponse>), 200)]
        public async Task<IActionResult> Post([FromBody]CourseRequest value)
        {
            var result = await _courseService.SaveCourse(value, isInsert: true);
            return Ok(result);
        }

        // PUT api/values/5
        /// <summary>
        /// Updates a course.
        /// </summary>
        /// <param name="value"></param>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResult<CourseResponse>), 200)]
        public async Task<IActionResult> Put([FromBody]CourseRequest value)
        {
            var result = await _courseService.SaveCourse(value, isInsert: false);
            if (!result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE api/values/5
        /// <summary>
        /// Removes a course.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _courseService.RemoveCourse(id);
        }
    }
}