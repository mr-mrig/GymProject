using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleTest
{
    public class TestServiceLayer
    {



        private bool _enableEagerLoading;


        private GymContext _context;




        public TestServiceLayer(GymContext context, bool enableEagerLoading = true)
        {
            _context = context;
            _enableEagerLoading = enableEagerLoading;
        }


        public void AddWorkingSet(uint workoutId, uint wunitProgressiveNumber, WSRepetitionsValue repetitions = null,
            RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IEnumerable<uint?> intensityTechniques = null)
        {
            WorkoutTemplateRoot wo = WorkoutTemplateRepositoryFind(workoutId);

            wo.AddTransientWorkingSet(wunitProgressiveNumber, repetitions, rest, effort, tempo, intensityTechniques);

            WorkoutTemplateRepositoryUpdate(wo);
            UnitOfWorkSave();
        }


        public void AddLinkedSet(uint workoutId, uint wunitProgressiveNumber, uint linkingIntensityTechniqueId, WSRepetitionsValue repetitions = null,
            RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null)
        {
            if (!_context.IntensityTechniques.Find(linkingIntensityTechniqueId).IsLinkingTechnique)
                throw new ArgumentException("The Intensity Technique is not a linking one", nameof(linkingIntensityTechniqueId));

            WorkoutTemplateRoot wo = WorkoutTemplateRepositoryFind(workoutId);

            wo.AddTransientWorkingSet(wunitProgressiveNumber, repetitions, rest, effort, tempo, new List<uint?>() { linkingIntensityTechniqueId });

            WorkoutTemplateRepositoryUpdate(wo);
            UnitOfWorkSave();
        }


        public void AddLinkedExcercise(uint workoutId, uint excerciseId, uint linkingIntensityTechniqueId)
        {
            if (!_context.IntensityTechniques.Find(linkingIntensityTechniqueId).IsLinkingTechnique)
                throw new ArgumentException("The Intensity Technique is not a linking one", nameof(linkingIntensityTechniqueId));

            WorkoutTemplateRoot wo = WorkoutTemplateRepositoryFind(workoutId);

            wo.DraftExcercise(excerciseId);

            WorkoutTemplateRepositoryUpdate(wo);
            UnitOfWorkSave();

            wo.LinkWorkUnits((uint)wo.WorkUnits.Count - 2, linkingIntensityTechniqueId);
            UnitOfWorkSave();
        }


        public WorkoutTemplateRoot PlanWorkout(uint trainingPlanId, uint trainingWeekProgressiveNumber, string workoutName, WeekdayEnum weeklyOccurance = null)
        {
            WorkoutTemplateReferenceEntity workoutAdded = AddWorkoutToPlan(trainingPlanId, trainingWeekProgressiveNumber);

            return CreateWorkout(workoutAdded.Id.Value, workoutName, weeklyOccurance);
        }

        /// <summary>
        /// Add an empty Workout to the Plan
        /// </summary>
        /// <param name="trainingPlanId"></param>
        /// <param name=""></param>
        public WorkoutTemplateReferenceEntity AddWorkoutToPlan(uint trainingPlanId, uint trainingWeekProgressiveNumber)
        {
            // Link the Workout to the Plan it belongs
            TrainingPlanRoot plan = TrainingPlanRepositoryFind(trainingPlanId);

            plan.PlanWorkout(trainingWeekProgressiveNumber, new List<WorkingSetTemplateEntity>());
            plan = TrainingPlanRepositoryUpdate(plan);

            UnitOfWorkSave();

            return plan.CloneLastWorkout(trainingWeekProgressiveNumber);
        }


        public WorkoutTemplateRoot CreateWorkout(uint workoutId, string workoutName, WeekdayEnum weeklyOccurance = null)
        {
            // Build the Workout
            WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlanWorkout(workoutId, new List<WorkUnitTemplateEntity>(), workoutName, weeklyOccurance);
            WorkoutTemplateRepositoryUpdate(workout);

            UnitOfWorkSave();

            return workout;
        }


        /// <summary>
        /// This is the simulation of what the Repository will do
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public WorkoutTemplateRoot WorkoutTemplateRepositoryFind(uint workoutId)
        {
            WorkoutTemplateRoot workout = null;

            if (_enableEagerLoading)
            {
                // One query for all objects
                workout = _context.WorkoutTemplates.Where(x => x.Id == workoutId)
                    .Include(wo => wo.WorkUnits)
                        .ThenInclude(wu => wu.WorkingSets)
                    .SingleOrDefault();
            }
            else
            {
                // No query if object already tracked -> If State = Added, but contt not saved hte WO is still retrieved
                workout = _context.WorkoutTemplates.Find(workoutId);

                if (workout != null)
                {
                    // One query for each Load -> wasteful
                    _context.Entry(workout).Collection(x => x.WorkUnits).Load();
                    // TODO
                    //_context.Entry(workout).Reference(x => x.n).Load();
                    throw new NotImplementedException();
                }
            }
            return workout;
        }


        /// <summary>
        /// This is the simulation of what the Repository will do
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public WorkoutTemplateRoot WorkoutTemplateRepositoryUpdate(WorkoutTemplateRoot workout)
        {
            return _context.Update(workout).Entity;
        }


        private TrainingPlanRoot TrainingPlanRepositoryFind(uint trainingPlanId)
        {
            TrainingPlanRoot workout = null;

            if (_enableEagerLoading)
            {
                // One query for all objects
                workout = _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
                    .Include(wo => wo.TrainingWeeks)
                        .ThenInclude(tw => tw.Workouts)
                    .SingleOrDefault();
            }
            else
            {
                // No query if object already tracked -> If State = Added, but contt not saved hte WO is still retrieved
                workout = _context.TrainingPlans.Find(trainingPlanId);

                if (workout != null)
                {
                    // One query for each Load -> wasteful
                    throw new NotImplementedException();
                }
            }
            return workout;
        }


        /// <summary>
        /// This is the simulation of what the Repository will do
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public TrainingPlanRoot TrainingPlanRepositoryUpdate(TrainingPlanRoot plan)
        {
            return _context.Update(plan).Entity;
        }



        /// <summary>
        /// This is the simulation of what the UnitOfWork will do
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public void UnitOfWorkSave()
        {
            // Avoid double insert for tables that should not be tracked
            IgnoreStaticEnumTables();
            DebugChangeTracker();
            _context.SaveChanges();
        }



        private void IgnoreStaticEnumTables()
        {
            // Avoid double insert for tables that should not be tracked
            //foreach (var entity in _context.ChangeTracker.Entries<TrainingEffortTypeEnum>())
            //    entity.State = EntityState.Unchanged;

            //foreach (var entity in _context.ChangeTracker.Entries<Enumeration>())
            //    if(!(entity.Entity is WeekdayEnum))
            //        entity.State = EntityState.Unchanged;

            foreach (var entity in _context.ChangeTracker.Entries<Enumeration>())
                entity.State = EntityState.Unchanged;
        }

        [Conditional("DEBUG")]
        private void DebugChangeTracker()
        {
            //foreach (var entity in _context.ChangeTracker.Entries<WorkUnitTemplateEntity>())
            //{
            //    var state = entity.State;

            //    if(state != EntityState.Unchanged)
            //        Debugger.Break();           // Do what you need before the breakpoint
            //}

            //foreach (var entity in _context.ChangeTracker.Entries<LinkedWorkValue>())
            //{
            //    var state = entity.State;

            //    if(state == EntityState.Added || state == EntityState.Deleted)
            //        entity.State = EntityState.Modified;

            //    Debugger.Break();           // Do what you need before the breakpoint
            //}
                
        }


    }
}
