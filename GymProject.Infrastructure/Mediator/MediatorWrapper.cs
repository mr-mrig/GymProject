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



        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : IDomainNotification
        {
            NotificationWrapper<TNotification> unwrappedNotification = new NotificationWrapper<TNotification>(notification);

            await _mediator.Publish(unwrappedNotification, cancellationToken);
        }
    }
}
