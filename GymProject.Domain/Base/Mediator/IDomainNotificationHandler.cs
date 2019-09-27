using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Domain.Base.Mediator
{
    public interface IDomainNotificationHandler<in TNotification>
    {

        /// <summary>
        /// Handle the Notifiation
        /// </summary>
        /// <param name="notification">The notification</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result</returns>
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }
}
