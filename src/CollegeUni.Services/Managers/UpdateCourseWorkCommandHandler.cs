using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Models;
using System;
using System.Diagnostics;

namespace CollegeUni.Services.Managers
{
    public class CourseWorkCommand
    {
        public string CourseWork { get; set; }
        public CourseResponse Response { get; set; }
    }
    public class UpdateCourseWorkCommandHandler: ICommandHandler<CourseWorkCommand>
    {
        readonly IUnitOfWork _unitOfWork;
        public UpdateCourseWorkCommandHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public void Handle(CourseWorkCommand command)
        {
            if(command?.Response != null)
            {
                command.Response.Credits++;
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
