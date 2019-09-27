using GymProject.Domain.Base.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Infrastructure.Mediator
{
    public class NotificationHandlerWrapper<T1, T2> : INotificationHandler<T1> 
        where T1 : NotificationWrapper<T2> 
        where T2 : IDomainNotification
    {

        private readonly IEnumerable<IDomainNotificationHandler<T2>> _handlers;



        //the IoC should inject all domain handlers here
        public NotificationHandlerWrapper(IEnumerable<IDomainNotificationHandler<T2>> handlers)
        {
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }



        public Task Handle(T1 notification, CancellationToken cancellationToken)
        {
            var handlingTasks = _handlers.Select(h =>
                h.Handle(notification.Notification, cancellationToken));

            return Task.WhenAll(handlingTasks);
        }

    }
}
