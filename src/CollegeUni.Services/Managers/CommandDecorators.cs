using CollegeUni.Api.Utilities.Extensions;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Models;
using CollegeUni.Utilities.Enumeration;
using CollegeUni.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CollegeUni.Services.Managers
{
    public class TransactionCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : IResult<TResult>
    {
        private readonly ILogger _logger;
        readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand, TResult> decorated;

        public TransactionCommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated, IUnitOfWork unitOfWork, ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<TransactionCommandHandlerDecorator<TCommand, TResult>>();
            _unitOfWork = unitOfWork;
            this.decorated = decorated;
        }

        public async Task<TResult> Handle(TCommand command)
        {
            // TODO: Combined use of Async Transactions and SaveAsync currently not supported.
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    await decorated.Handle(command);
                    transaction.Commit();
                }
                catch (ApiResponseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    command.Result = default(TResult);
                    // transaction.RollBack() should not be called when scope is wrapped in a using statement
                    _logger.LogError(3, ex, "Transaction Rollback");
                }
            }
            return command.Result;
        }
    }
    public class NonOptimisticConcurrencyCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : IResult<TResult>
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand, TResult> decorated;

        public NonOptimisticConcurrencyCommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            this.decorated = decorated;
        }

        public async Task<TResult> Handle(TCommand command)
        {
            await decorated.Handle(command);
            //await _unitOfWork.SaveAsync();
            _unitOfWork.Save();
            return command.Result;
        }
    }

    public class OptimisticConcurrencyCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : IResult<TResult>, IResolveable
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand, TResult> decorated;

        public OptimisticConcurrencyCommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            this.decorated = decorated;
        }

        public async Task<TResult> Handle(TCommand command)
        {
            await decorated.Handle(command);
            if (command.ConflictStrategy == ResolveStrategy.ShowUnresolvedConflicts)
            {
                var resolveConflicts = ConcurrencyHelper.ResolveConflicts(command.Entity, command.ModelState);
                //await _unitOfWork.SaveAsync(resolveConflicts, userResolveConflict: true);
                _unitOfWork.Save(resolveConflicts, userResolveConflict: true);
            }
            else
            {
                RefreshConflict refreshMode = (RefreshConflict)command.ConflictStrategy;
                if (!EnumExtensions.IsFlagDefined(refreshMode))
                    refreshMode = RefreshConflict.StoreWins;
                _unitOfWork.SaveSingleEntry(refreshMode);
            }
            return command.Result;
        }
    }
}
