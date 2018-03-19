using AutoMapper;
using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CollegeUni.Services.Managers
{
    #region Remove Contrived Examples
    public class CourseValidatorA: IValidator<CourseResponse>
    {
        public ValidationResults Validate(CourseResponse instance)
        {
            var results = new ValidationResults { ModelState = new Dictionary<string, string[]>() };
            results.ModelState.Add("Alpha", new[] { "Required." });
            return results;
        }
    }
    public class CourseValidatorB: IValidator<CourseResponse>
    {
        public ValidationResults Validate(CourseResponse instance)
        {
            var results = new ValidationResults { ModelState = new Dictionary<string, string[]>() };
            results.ModelState.Add("Bravo", new[] { "Required." });
            return results;
        }
    }
    public class StudentValidatorA: IValidator<Student>
    {
        public ValidationResults Validate(Student instance)
        {
            var results = new ValidationResults { ModelState = new Dictionary<string, string[]>() };
            results.ModelState.Add("Able", new[] { "Required." });
            return results;
        }
    }
    public class CourseWorkCommand
    {
        public string CourseWork { get; set; }
        public CourseResponse Response { get; set; }
    }
    public class CourseWorkCommandHandler: ICommandHandler<CourseWorkCommand>
    {
        readonly IUnitOfWork _unitOfWork;
        IValidator<CourseResponse> _validator;
        public CourseWorkCommandHandler(IUnitOfWork unitOfWork, IValidator<CourseResponse> validator) {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public void Handle(CourseWorkCommand command)
        {
            if(command?.Response != null)
            {
                command.Response.Credits++;
                var results = _validator.Validate(command.Response);
                command.Response.ModelState = results.ModelState;
            }
        }
    }
    public class ScaleCommandHandlerDecorator<TCommand>: ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> _decorated;
        public ScaleCommandHandlerDecorator(ICommandHandler<TCommand> decorated) {
            _decorated = decorated;
        }
        public void Handle(TCommand command)
        {
            Trace.WriteLine("Start Scaling..");
            _decorated.Handle(command);
            Trace.WriteLine("End Scaling..");

        }
    }
    public class AugmentedCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> _decorated;
        public AugmentedCommandHandlerDecorator(ICommandHandler<TCommand> decorated)
        {
            _decorated = decorated;
        }
        public void Handle(TCommand command)
        {
            Trace.WriteLine("Start Augmenting..");
            _decorated.Handle(command);
            Trace.WriteLine("End Augmenting..");

        }
    }
    #endregion

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
        IValidator<CourseInsertCommand> _validator;
        public CourseInsertCommandHandler(IUnitOfWork unitOfWork, IValidator<CourseInsertCommand> validator) {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<int> Handle(CourseInsertCommand command)
        {
            var results = _validator.Validate(command);
            command.Entity = Mapper.Map<CourseInsertCommand, Course>(command);
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
        public Course Entity { get; set; }
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
