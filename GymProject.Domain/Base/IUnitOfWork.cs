using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Domain.Base
{

    public interface IUnitOfWork : IDisposable
    {
        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        //Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// Commit all changes - Async
        /// </summary>
        Task<bool> SaveAsync(CancellationToken cancellationToken);

    }

}
