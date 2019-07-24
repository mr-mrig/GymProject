using MediatR;

namespace GymProject.Infrastructure.Mediator
{
    public class NotificationWrapper<T> : INotification
    {


        public T Notification { get; }



        public NotificationWrapper(T notification)
        {
            Notification = notification;
        }
    }
}
