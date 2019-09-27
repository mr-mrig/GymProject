//using GymProject.Domain.Base;
//using GymProject.Infrastructure.Persistence.EFContext;
//using Microsoft.EntityFrameworkCore;
//using System;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using System.Linq;
//using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
//using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
//using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
//using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
//using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
//using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
//using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
//using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
//using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
//using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
//using GymProject.Domain.TrainingDomain.WorkingSetNote;
//using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
//using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;

//namespace GymProject.Infrastructure.Persistence.UnitOfWork
//{
//    public class UnitOfWork : IUnitOfWork

//    {

//        private const int MaxNumberOfRetry = 3;


//        private readonly GymContext _context;


//        // Dispose pattern
//        private bool _disposed = false;



//        #region Repositories

//        private ITrainingPlanRepository _trainingPlanRepository;

//        public ITrainingPlanRepository TrainingPlanRepository
//        {
//            get => _trainingPlanRepository 
//                ?? new SQLTrainingPlanRepository(_context);
//        }


//        private IWorkoutTemplateRepository _workoutTemplateRepository;

//        public IWorkoutTemplateRepository WorkoutTemplateRepository
//        {
//            get => _workoutTemplateRepository 
//                ?? new SQLWorkoutTemplateRepository(_context);
//        }


//        private IWorkoutSessionRepository _workoutSessionRepository;

//        public IWorkoutSessionRepository WorkoutSessionRepository
//        {
//            get => _workoutSessionRepository
//                ?? new SQLWorkoutSessionRepository(_context);
//        }


//        private ITrainingScheduleRepository _trainingScheduleRepository;

//        public ITrainingScheduleRepository TrainingScheduleRepository
//        {
//            get => _trainingScheduleRepository
//                ?? new SQLTrainingScheduleRepository(_context);
//        }


//        private IIntensityTechniqueRepository _intensityTechniqueRepository;

//        public IIntensityTechniqueRepository IntensityTechniqueRepository
//        {
//            get => _intensityTechniqueRepository
//                ?? new SQLIntensityTechniqueRepository(_context);
//        }


//        private ITrainingHashtagRepository _trainingHashtagRepository;

//        public ITrainingHashtagRepository TrainingHashtagRepository
//        {
//            get => _trainingHashtagRepository
//                ?? new SQLTrainingHashtagRepository(_context);
//        }


//        private ITrainingProficiencyRepository _trainingProficiencyRepository;

//        public ITrainingProficiencyRepository TrainingProficiencyRepository
//        {
//            get => _trainingProficiencyRepository
//                ?? new SQLTrainingProficiencyRepository(_context);
//        }


//        private ITrainingPhaseRepository _trainingPhaseRepository;

//        public ITrainingPhaseRepository TrainingPhaseRepository
//        {
//            get => _trainingPhaseRepository
//                ?? new SQLTrainingPhaseRepository(_context);
//        }


//        private IWorkUnitTemplateNoteRepository _workUnitTemplateNoteRepository;

//        public IWorkUnitTemplateNoteRepository WorkUnitTemplateNoteRepository
//        {
//            get => _workUnitTemplateNoteRepository
//                ?? new SQLWorkUnitTemplateNoteRepository(_context);
//        }


//        private IWorkingSetNoteRepository _workingSetNoteRepository;

//        public IWorkingSetNoteRepository WorkingSetNoteRepository
//        {
//            get => _workingSetNoteRepository
//                ?? new SQLWorkingSetNoteRepository(_context);
//        }


//        private ITrainingPlanNoteRepository _trainingPlanNoteRepository;

//        public ITrainingPlanNoteRepository TrainingPlanNoteRepository
//        {
//            get => _trainingPlanNoteRepository
//                ?? new SQLTrainingPlanNoteRepository(_context);
//        }


//        private ITrainingPlanMessageRepository _trainingPlanMessageRepository;

//        public ITrainingPlanMessageRepository TrainingPlanMessageRepository
//        {
//            get => _trainingPlanMessageRepository
//                ?? new SQLTrainingPlanMessageRepository(_context);
//        }


//        #endregion


//        #region Ctors

//        public UnitOfWork(DbContext dbContext)
//        {
//            //_context = dbContext as GymContext ?? new GymContext();
//            _context = dbContext as GymContext ?? new GymContext();
//        }
//        #endregion


//        #region IUnitOfWork Implementation

//        public void Save()
//        {
//            if (_context == null)
//                return;

//            int counter = 0;

//            bool saved = false;
//            do
//            {
//                try
//                {
//                    _context.SaveChanges();
//                    saved = true;
//                }

//                catch (DbUpdateException ex)
//                {
//                    // Get the current entity values and the values in the database 
//                    var entry = ex.Entries.Single();
//                    //var currentValues = entry.CurrentValues;

//                    switch (entry.State)
//                    {
//                        case EntityState.Added:

//                            // added on client, non in store - store wins
//                            entry.State = EntityState.Modified;
//                            break;

//                        case EntityState.Deleted:

//                            //deleted on client, modified in store
//                            entry.Reload();
//                            entry.State = EntityState.Deleted;
//                            break;

//                        case EntityState.Modified:

//                            PropertyValues currentValues = entry.CurrentValues.Clone();

//                            //Modified on client, Modified in store
//                            entry.Reload();
//                            entry.CurrentValues.SetValues(currentValues);

//                            break;

//                        default:

//                            //For good luck
//                            entry.Reload();
//                            break;
//                    }
//                }
//            } while (!saved || counter++ < MaxNumberOfRetry);
//        }


//        public void Discard()
//        {
//            foreach (var entry in _context.ChangeTracker.Entries()
//                  .Where(e => e.State != EntityState.Unchanged))
//            {
//                switch (entry.State)
//                {
//                    case EntityState.Added:

//                        entry.State = EntityState.Detached;
//                        break;

//                    case EntityState.Modified:
//                    case EntityState.Deleted:

//                        entry.Reload();
//                        break;
//                }
//            }
//        }
//        #endregion


//        #region IDispose Implementation

//        public virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    _context.Dispose();
//                }
//            }
//            _disposed = true;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//        #endregion
//    }
//}
