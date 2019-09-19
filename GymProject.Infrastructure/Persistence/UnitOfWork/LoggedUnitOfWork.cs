using GymProject.Domain.Base;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.UnitOfWork
{
    public class LoggedUnitOfWork : IUnitOfWork
    {

        private const int MaxNumberOfRetry = 3;


        private readonly GymContext _context;


        // Dispose pattern
        private bool _disposed = false;



        #region Repositories

        //public ITrainingPlanRepository TrainingPlanRepository TrainingPlanRepository;
        #endregion


        #region Ctors

        public LoggedUnitOfWork(DbContext dbContext/*, ILogger logger*/)
        {
            _context = dbContext as GymContext ?? new GymContext();
            //_logger = logger;
        }
        #endregion


        #region IUnitOfWork Implementation

        public void Save()
        {
            if (_context == null)
                return;

            int counter = 0;

            bool saved = false;
            do
            {
                try
                {
                    _context.SaveChanges();
                    saved = true;
                }

                catch (DbUpdateException ex)
                {
                    // Get the current entity values and the values in the database 
                    var entry = ex.Entries.Single();
                    //var currentValues = entry.CurrentValues;

                    switch (entry.State)
                    {
                        case EntityState.Added:

                            // added on client, non in store - store wins
                            entry.State = EntityState.Modified;
                            break;

                        case EntityState.Deleted:

                            //deleted on client, modified in store
                            entry.Reload();
                            entry.State = EntityState.Deleted;
                            break;

                        case EntityState.Modified:

                            PropertyValues currentValues = entry.CurrentValues.Clone();

                            //Modified on client, Modified in store
                            entry.Reload();
                            entry.CurrentValues.SetValues(currentValues);

                            break;

                        default:

                            //For good luck
                            entry.Reload();
                            break;
                    }
                }
            } while (!saved || counter++ < MaxNumberOfRetry);
        }


        public void Discard()
        {
            foreach (var entry in _context.ChangeTracker.Entries()
                  .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:

                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Modified:
                    case EntityState.Deleted:

                        entry.Reload();
                        break;
                }
            }
        }
        #endregion


        #region IDispose Implementation

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
