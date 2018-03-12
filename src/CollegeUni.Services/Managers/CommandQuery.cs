using CollegeUni.Api.Utilities.Extensions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeUni.Services.Managers
{
    /**
     * Intended for use in the Command Handler Decorator pattern.
     */
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
    
    public class ValidationResults
    {
        public Dictionary<string, string[]> ModelState { get; set; }
    }

    /**
     * Intended for use in the batch registration of validators.
     */
    public interface IValidator<T>
    {
        ValidationResults Validate(T instance);
    }

    /**
     * A single composite type that wraps IEnumerable<IValidator<T>> and presents it to the consumer as a single instance.
     */
    public class CompositeValidator<T> : IValidator<T>
    {
        private readonly IEnumerable<IValidator<T>> validators;

        public CompositeValidator(IEnumerable<IValidator<T>> validators)
        {
            this.validators = validators;
        }

        public ValidationResults Validate(T instance)
        {
            var allResults = new ValidationResults { ModelState = new Dictionary<string, string[]>() };

            foreach (var validator in validators)
            {
                var results = validator.Validate(instance);
                foreach (var kvp in results.ModelState)
                {
                    allResults.ModelState.TryAdd(kvp.Key, kvp.Value);
                }
            }
            return allResults;
        }
    }

    public interface IQuery<TResult>{}

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

    /**
     * The IQueryProcessor is a mediator that sits between the consumers and the query handlers.
     * It depends on the IQuery<TResult> interface. This allows us to have compile time support in our consumers that depend on the IQueryProcessor.
     */
    public interface IQueryProcessor
    {
        TResult Process<TResult>(IQuery<TResult> query);
    }

    /**
     * The responsibility of the implementation of the IQueryProcessor is to find the right IQueryHandler.
     * The QueryProcessor class constructs a specific IQueryHandler<TQuery, TResult> type based on the type of the supplied query instance.
     * This type is used to ask the supplied container class to get an instance of that type.
     */
    public sealed class QueryProcessor : IQueryProcessor
    {
        private readonly Container container;

        public QueryProcessor(Container container)
        {
            this.container = container;
        }

        [DebuggerStepThrough]
        public TResult Process<TResult>(IQuery<TResult> query)
        {
            var handlerType =
                typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }
    }
    public class QueryMeta<T, U>: IQuery<PaginatedResult<U>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public IQueryable<T> Source { get; set; }
        public PaginatedResult<U> Result { get; set; }
    }
 
    public class QueryMetaAsync<T, U>: IQuery<Task<PaginatedResult<U>>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public IQueryable<T> Source { get; set; }
        public Task<PaginatedResult<U>> Result { get; set; }
    }
 
    public class BrowseQueryHandler<T,U> : IQueryHandler<QueryMeta<T,U>, PaginatedResult<U>>
    {
        public PaginatedResult<U> Handle(QueryMeta<T,U> query)
        {
            return query.Source.ToPageResult<T, U>(query.Offset, query.Limit);
        }
    }
  
    public class BrowseQueryHandlerAsync<T,U> : IQueryHandler<QueryMetaAsync<T,U>, Task<PaginatedResult<U>>>
    {
        public Task<PaginatedResult<U>> Handle(QueryMetaAsync<T,U> query)
        {
            return query.Source.ToPageResultAsync<T, U>(query.Offset, query.Limit);
        }
    }
}
