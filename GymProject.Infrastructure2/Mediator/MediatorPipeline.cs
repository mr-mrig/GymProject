using System.Threading;
using System.Threading.Tasks;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Infrastructure.Mediator
{
    public class MediatorPipeline<TNotification> : IMediatorNotificationHandler<TNotification> where TNotification : IMediatorNotification
    {
        private readonly IMediatorNotificationHandler<TNotification> _inner;
        //private readonly IEnumearble<IMessageValidator<TRequest>> _validators;
        //private readonly IMessageAuthorizer _authorizer;
        //private readonly IEnumerable<IPreRequestProcessor<TRequest>> _preProcessors;
        //private readonly IEnumerable<IPostRequestProcessor<TRequest, TResponse>> _postProcessors;
        //private readonly IEnumerable<IResponseProcessor<TResponse>> _responseProcessors;




        //public MediatorPipeline(IRequestHandler<TRequest, TResponse> inner,
        //    IEnumerable<IMessageValidator<TRequest>> validator,
        //    IMessageAuthorizor authorizer,
        //    IEnumerable<IPreRequestProcessor<TRequest>> preProcessors,
        //    IEnumerable<IPostRequestProcessor<TRequest, TResponse>> postProcessors,
        //    IEnumerable<IResponseProcessor<TResponse>> responseProcessors
        //    )
        //{
        //    _inner = inner;
        //    _validators = validators;
        //    _authorizer = authorizer;
        //    _preProcessors = preProcessors;
        //    _postProcessors = postProcessors;
        //    _responseProcessors = responseProcessors;
        //}

        public MediatorPipeline(IMediatorNotificationHandler<TNotification> inner)
        {
            _inner = inner;
        }





        public Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            return _inner.Handle(notification);
        }



        //public TResponse Handle(TRequest message)
        //{
        //    using (LogContext.PushProperty(LogConstants.MediatRRequestType, requestType))
        //    using (Metrics.Time(Timers.MediatRRequest))
        //    {
        //        _securityHandler.Evaluate(message);

        //        foreach (var preProcessor in _preProcessors)
        //            preProcessor.Handle(request);

        //        var failures = _validators
        //            .Select(v => v.Validate(message))
        //            .SelectMany(result => result.Errors)
        //            .Where(f => f != null)
        //            .ToList();
        //        if (failures.Any())
        //            throw new ValidationException(failures);

        //        var response = _inner.Handle(request);

        //        foreach (var postProcessor in _postProcessors)
        //            postProcessor.Handle(request, response);

        //        foreach (var responseProcessor in _responseProcessors)
        //            responseProcessor.Handle(response);

        //        return response;
        //    }
    }
}
