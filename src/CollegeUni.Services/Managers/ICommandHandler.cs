using System;
using System.Collections.Generic;
using System.Text;

namespace CollegeUni.Services.Managers
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}
