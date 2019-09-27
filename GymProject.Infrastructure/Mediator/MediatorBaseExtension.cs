using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;
using GymProject.Infrastructure.Persistence.EFContext;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GymProject.Infrastructure.Mediator
{
    
    static class MediatorBaseExtension
    {


        public static async Task PublishDomainEventsAsync(this IMediator mediator, GymContext context, ILogger logger)
        {
            var domainEntities = context.ChangeTracker
                .Entries<Entity<uint?>>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) => 
                {
                    logger.LogInformation($"Domain Event publishing: {domainEvent}");
                    await mediator.Publish(domainEvent);
                    logger.LogInformation($"Domain Event published: {domainEvent}");
                });

            await Task.WhenAll(tasks);
        }
    }
}
