using AutoMapper;
using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Models;
using CollegeUni.Utilities.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Services.Managers
{
    public class IntroCourseCreditsValidator: IValidator<CourseInsertCommand>
    {
        public ValidationResults Validate(CourseInsertCommand instance)
        {
            var results = new ValidationResults { ModelState = new Dictionary<string, string[]>() };
            if (instance.Id < 200 && instance.Credits > 6)
            {
                results.ModelState.TryAdd("Credits", new[] { "100-level courses are not allowed to be greater than 6 credits." });
            }
            return results;
        }
    }
    public class UniqueCourseValidator: IValidator<CourseInsertCommand>
    {
        readonly IUnitOfWork _unitOfWork;
        public UniqueCourseValidator(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public ValidationResults Validate(CourseInsertCommand instance)
        {
            var courses = _unitOfWork.CourseRepository.Get(c => c.Title == instance.Entity.Title || c.Id == instance.Entity.Id).ToList();

            var results = new ValidationResults { ModelState = new Dictionary<string, string[]>() };
            if (courses.Any(c => c.Title == instance.Entity.Title))
            {
                results.ModelState.TryAdd("Title", new[] { "A course of that title already exists in the system." });
            }
            if (courses.Any(c => c.Id == instance.Entity.Id))
            {
                results.ModelState.TryAdd("Id", new[] { "A course of that Id already exists in the system." });
            }
            return results;
        }
    }
    public class CourseInsertCommand: IResult<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public Course Entity { get; set; }
        public int Result { get; set; }
    }
    public class CourseInsertCommandHandler: ICommandHandler<CourseInsertCommand, int>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IValidator<CourseInsertCommand> _validator;
        public CourseInsertCommandHandler(IUnitOfWork unitOfWork, IValidator<CourseInsertCommand> validator) {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<int> Handle(CourseInsertCommand command)
        {
            command.Entity = Mapper.Map<CourseInsertCommand, Course>(command);
            var results = _validator.Validate(command);
            if (results.ModelState.Any())
            {
                throw new ApiResponseException("Insert Error", results.ModelState, 400);
            }
            _unitOfWork.CourseRepository.Insert(command.Entity);
            command.Result = 1;
            return command.Result;
        }
    }
    public class CourseDeleteCommand: IResult<int>
    {
        public int Id { get; set; }
        public int Result { get; set; }
    }
    public class CourseDeleteCommandHandler : ICommandHandler<CourseDeleteCommand, int>
    {
        readonly IUnitOfWork _unitOfWork;
        public CourseDeleteCommandHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(CourseDeleteCommand command)
        {
            _unitOfWork.CourseRepository.Delete(command.Id);
            command.Result = 1;
            return command.Result;
        }
    }
    public class CourseUpdateCommand: CourseRequest, IResult<int>
    {
        public new Course Entity { get; set; }
        public int Result { get; set; }
    }
    public class CourseUpdateCommandHandler: ICommandHandler<CourseUpdateCommand, int>
    {
        readonly IUnitOfWork _unitOfWork;
        IValidator<CourseRequest> _validator;
        public CourseUpdateCommandHandler(IUnitOfWork unitOfWork, IValidator<CourseRequest> validator) {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<int> Handle(CourseUpdateCommand command)
        {
            command.Entity = Mapper.Map<CourseRequest, Course>(command);
            _unitOfWork.CourseRepository.Update(command.Entity);
            command.Result = 1;
            return command.Result;
        }
    }
}
