using GymProject.Infrastructure.Persistence.EFContext;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    public abstract class DbContextFactory
    {
        public DatabaseSeed ContextSeed { get; protected set; }


        public abstract Task<GymContext> CreateContextAsync();


        /// <summary>
        /// Implements a dummy Mediator which does nothing but allows to use the GymContext class
        /// </summary>
        protected class DummyMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<TResponse>(default);
            }

            public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.CompletedTask;
            }
        }
    }
}
