using GymProject.Domain.Base.Mediator;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Infrastructure.Mediator
{
    public class MediatorWrapper : IMediatorService
    {

        private readonly IMediator _mediator;



        public MediatorWrapper(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }



        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : IMediatorNotification
        {
            NotificationWrapper<TNotification> notification2 = new NotificationWrapper<TNotification>(notification);

            return _mediator.Publish(notification2, cancellationToken);
        }
    }
}
