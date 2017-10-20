using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CollegeUni.Services;
using Microsoft.AspNetCore.Authorization;
using CollegeUni.Models;
using SchoolUni.Database.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollegeUni.Controllers
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
        [HttpGet]
        public async Task<IActionResult> Get(int offset, int limit, int? studentID =  null)
        {
            var result = await _courseService.GetCourses(
                new StudentBrowseRequest {
                    StudentID = studentID,
                    PageInfo = new PageData
                    {
                        offset = offset,
                        limit = limit
                    }
                });
            return Ok(result);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _courseService.GetCourse(id);
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
