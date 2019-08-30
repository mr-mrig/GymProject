﻿using System;

namespace GymProject.Domain.Base
{

    public interface IUnitOfWork : IDisposable
    {
        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        //Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Committ all changes
        /// </summary>
        void Commit();

        /// <summary>
        /// Discards all changes that has not been commited
        /// </summary>
        void Rollback();
    }

}
