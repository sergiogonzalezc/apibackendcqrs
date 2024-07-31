using BackEndProducts.Application.CQRS;
using FluentValidation;
using MediatR;
using FluentValidation.Results;
namespace BackEndProducts.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }
        return await next();
    }
}


//public class ValidationException : Exception
//{
//    public ValidationException()
//        : base("Se han producido uno o más errores de validación.")
//    {
//        Errors = new Dictionary<string, string[]>();
//    }

//    public ValidationException(IEnumerable<ValidationFailure> failures)
//        : this()
//    {
//        Errors = failures
//            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
//            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
//    }

//    public IDictionary<string, string[]> Errors { get; }
//}
