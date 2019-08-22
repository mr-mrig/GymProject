using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Domain.Base.Mediator
{
    public interface IMediatorService
    {

        /// <summary>
        /// Published the notification to the Mediator
        /// </summary>
        /// <typeparam name="TNotification">IMediatorNotification implementation</typeparam>
        /// <param name="notification">The notification message</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result</returns>
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : IMediatorNotification;

    }
}
