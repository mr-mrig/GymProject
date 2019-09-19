using System;

namespace GymProject.Domain.Base
{

    public interface IUnitOfWork : IDisposable
    {
        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        //Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Commit all changes
        /// </summary>
        void Save();

        /// <summary>
        /// Discards all changes that has not been commited
        /// </summary>
        void Discard();
    }

}
