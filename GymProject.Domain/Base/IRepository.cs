
namespace GymProject.Domain.Base
{

    public interface IRepository<T> where T : IAggregateRoot
    {


        // DDD note: no need for the classic UoW implementation, as DD is intrinsically transaction consistent
        // As long as event sourcing and idempotency is ensured
        // https://blog.sapiensworks.com/post/2015/09/02/DDD-and-UoW

        /// <summary>
        /// The entity accountable to Save/Discard the changes made by the repository
        /// </summary>
        IUnitOfWork UnitOfWork { get; }



        /// <summary>
        /// Load the entire aggregate root with the specified ID
        /// </summary>
        /// <param name="id">The ID of the root entry of the aggregate to be loaded</param>
        T Find(uint id);


        /// <summary>
        /// Adds an aggregate
        /// </summary>
        /// <param name="aggregateRoot">The root entry of the aggregate to be added</param>
        T Add(T aggregateRoot);


        /// <summary>
        /// Modifies an aggregate
        /// </summary>
        /// <param name="aggregateRoot">The root entry of the aggregate to be modified</param>
        T Modify(T aggregateRoot);


        /// <summary>
        /// Remove the selected aggregate
        /// </summary>
        /// <param name="aggregateRoot">The root entry of the aggregate to be removed</param>
        void Remove(T aggregateRoot);

    }

}
