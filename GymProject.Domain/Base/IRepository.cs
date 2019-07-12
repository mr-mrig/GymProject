

using System.Collections;
using System.Collections.Generic;

namespace GymProject.Domain.Base
{

    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }


        /// <summary>
        /// Get all the entities in the table
        /// </summary>
        /// <returns>The list of entities</returns>
        IEnumerable<T> FindAll();

        /// <summary>
        /// Adds an entity
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        void Add(T entity);

        /// <summary>
        /// Remove the selected entity
        /// </summary>
        /// <param name="entity">The entity to be removed</param>
        void Remove(T entity);

    }

}
