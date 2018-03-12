using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CollegeUni.Services.Managers
{

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
    public class UpdateCourseWorkCommandHandler: ICommandHandler<CourseWorkCommand>
    {
        readonly IUnitOfWork _unitOfWork;
        IValidator<CourseResponse> _validator;
        public UpdateCourseWorkCommandHandler(IUnitOfWork unitOfWork, IValidator<CourseResponse> validator) {
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
}
