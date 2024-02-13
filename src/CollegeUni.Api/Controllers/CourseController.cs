using CollegeUni.Services.Models;
using CollegeUni.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Managers;
using System.Collections.Generic;
using System.Linq;
using CollegeUni.Api.Utilities.Extensions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollegeUni.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CourseController : CollegeUniBaseController
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IQueryProcessor _queryProcessor;
        readonly ICommandHandler<CourseInsertCommand, int> _courseInsertHandler;
        readonly ICommandHandler<CourseUpdateCommand, int> _courseUpdateHandler;
        readonly ICommandHandler<CourseDeleteCommand, int> _courseDeleteHandler;

        public CourseController(
            IUnitOfWork unitOfWork,
            IQueryProcessor queryProcessor,
            ICommandHandler<CourseInsertCommand, int> courseInsertHandler,
            ICommandHandler<CourseUpdateCommand, int> courseUpdateHandler,
            ICommandHandler<CourseDeleteCommand, int> courseDeleteHandler
            )
        {
            _unitOfWork = unitOfWork;
            _queryProcessor = queryProcessor;
            _courseInsertHandler = courseInsertHandler;
            _courseUpdateHandler = courseUpdateHandler;
            _courseDeleteHandler = courseDeleteHandler;
        }
        // GET: api/values
        /// <summary>
        /// Returns courses.
        /// </summary>
        /// <param name="offset">The offset determines the number of courses to skip. Set to 0 to skip none.</param>
        /// <param name="limit">Limits the number of course items retrieved.</param>
        /// <param name="search">An optional filter criteria to filter courses by title.</param>
        /// <param name="studentId">An optional filter criteria to filter courses by student id.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResult<BrowseResponse<CourseResponse>>), 200)]
        public async Task<IActionResult> Get(int offset, int limit, string search, int? studentId = null)
        {
            var request = new CourseBrowseRequest
            {
                StudentId = studentId,
                Search = search,
                PageInfo = new PageMeta
                {
                    Offset = offset,
                    Limit = limit
                }
            };
            var querySearchCourses = new GetCoursesQuery {
                Search = request.Search,
                Result = _unitOfWork.CourseRepository.Get()
            };
            var queryCoursesByStudent = new GetCoursesByStudentQuery {
                StudentId = request.StudentId,
            };
            var workItems = new List<IQuery<IQueryable<Course>>>
            {
                querySearchCourses,
                queryCoursesByStudent
            };

            var workFlow = new QueryFlow(_queryProcessor);
            var completedQueryResult = workFlow.Post(workItems);
            var response = new BrowseResponse<CourseResponse>
            {
                PageInfo = request.PageInfo,
                PageResult = await completedQueryResult.ToPageResultAsync<Course, CourseResponse>(request.PageInfo.Offset, request.PageInfo.Limit)
            };
            return Ok(new ServiceResult<BrowseResponse<CourseResponse>> { Data = response });
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
            var result = await GetCourse(id);
            return Ok(result);
        }

        // POST api/values
        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResult<CourseResponse>), 200)]
        public async Task<IActionResult> Post([FromBody]CourseRequest request)
        {
            var modelState = new Dictionary<string, string[]>();
            var cmd = Mapper.Map<CourseRequest, CourseInsertCommand>(request);
            await _courseInsertHandler.Handle(cmd);
            var result = await GetServiceResult(cmd.Entity, modelState, cmd.Result);
            return Ok(result);
        }

        // PUT api/values/5
        /// <summary>
        /// Updates a course.
        /// </summary>
        /// <param name="value"></param>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResult<CourseResponse>), 200)]
        public async Task<IActionResult> Put([FromBody]CourseRequest request)
        {
            request.ConflictStrategy = CollegeUni.Utilities.Enumeration.ResolveStrategy.ShowUnresolvedConflicts;
            var cmd = Mapper.Map<CourseRequest, CourseUpdateCommand>(request);

            var cmdResult = await _courseUpdateHandler.Handle(cmd);

            var result = await GetServiceResult(cmd.Entity, cmd.ModelState, cmd.Result);
            if (result.HasErrors)
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
            _courseDeleteHandler.Handle(new CourseDeleteCommand { Id = id });
        }
        public async Task<ServiceResult<CourseResponse>> GetCourse(int courseId)
        {
            var response = Mapper.Map<Course, CourseResponse>(await _unitOfWork.CourseRepository.GetByIdAsync(courseId));
            var serviceResult = new ServiceResult<CourseResponse> { Data = response };
            return serviceResult;
        }
        private async Task<ServiceResult<CourseResponse>> GetServiceResult(Course courseEntity, Dictionary<string, string[]> modelState, int result)
        {
            if (result > 0)
            {
                return await GetCourse(courseEntity.Id);
            }
            else
            {
                CourseResponse response = Mapper.Map<Course, CourseResponse>(courseEntity);
                return new ServiceResult<CourseResponse> { Message = "There were errors saving Course.", ModelState = modelState, Data = response };
            }
        }
    }
}