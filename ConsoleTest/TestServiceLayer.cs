using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
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
            WorkoutTemplateRepositoryUpdate(wo);
            UnitOfWorkSave();
        }


        public WorkoutTemplateRoot PlanWorkout(uint trainingPlanId, uint trainingWeekProgressiveNumber)
        {
            TrainingPlanRoot plan = TrainingPlanRepositoryFind(trainingPlanId);
            TrainingWeekEntity week = plan.CloneTrainingWeek(trainingWeekProgressiveNumber);

            int nextProgressiveNumber = week.WorkoutIds.Count;

            // Build the Workout
            WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(week.Id.Value, (uint)nextProgressiveNumber);
            //workout.PlanToWeek(week.Id.Value);

            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    // Workout Template Aggregate
                    WorkoutTemplateRepositoryUpdate(workout);
                    UnitOfWorkSave();

                    //throw new ArgumentException();

                    // Training Plan Aggregate
                    plan.PlanWorkout(trainingWeekProgressiveNumber, workout.Id.Value);
                    plan = TrainingPlanRepositoryUpdate(plan);
                    UnitOfWorkSave();

                    _context.CommitTransaction(transaction);
                }
                catch
                {
                    _context.RollbackTransaction();
                }
            }

            return workout;
        }


        public WorkoutTemplateRoot CreateWorkoutTemplateOnly(uint trainingWeekId, uint workoutProgressiveNumber)
        {
            //TrainingPlanRoot plan = TrainingPlanRepositoryFind(trainingPlanId);
            //TrainingWeekEntity week = plan.CloneTrainingWeek(trainingWeekProgressiveNumber);

            //int nextProgressiveNumber = week.WorkoutIds.Count;

            // Build the Workout
            WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(trainingWeekId, (uint)workoutProgressiveNumber);
            //workout.PlanToWeek(week.Id.Value);

            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    // Workout Template Aggregate
                    WorkoutTemplateRepositoryUpdate(workout);
                    UnitOfWorkSave();

                    //throw new ArgumentException();


                    _context.CommitTransaction(transaction);
                }
                catch(Exception e)
                {
                    _context.RollbackTransaction();
                }
            }

            return workout;
        }


        public void ModifyWorkoutAttributes(uint workoutId, string workoutName, WeekdayEnum weeklyOccurance = null)
        {
            WorkoutTemplateRoot workout = WorkoutTemplateRepositoryFind(workoutId);
            workout.GiveName(workoutName);
            workout.ScheduleToSpecificDay(weeklyOccurance);

            WorkoutTemplateRepositoryUpdate(workout);
            UnitOfWorkSave();
        }


        public WorkoutSessionRoot StartWorkoutSession(uint workoutTemplateId)
        {
            WorkoutSessionRoot session = WorkoutSessionRoot.BeginWorkout(workoutTemplateId);

            WorkoutSessionRepositoryUpdate(session);
            UnitOfWorkSave();

            return session;
        }

        public void StartOnTheFlyWorkout()
        {
            throw new NotImplementedException();
        }


        public WorkUnitEntity StartExcercise(uint workoutId, uint excerciseId)
        {
            WorkoutSessionRoot session = WorkoutSessionRepositoryFind(workoutId);
            session.StartTrackingExcercise(excerciseId);

            WorkoutSessionRepositoryUpdate(session);
            UnitOfWorkSave();

            return session.CloneLastWorkUnit();
        }


        public void TrackWorkingSet(uint workoutId, uint excerciseProgressiveNumber, WSRepetitionsValue repetitionsPerformed, WeightPlatesValue weightLifted = null)
        {
            WorkoutSessionRoot session = WorkoutSessionRepositoryFind(workoutId);
            session.TrackWorkingSet(excerciseProgressiveNumber, repetitionsPerformed, weightLifted);

            WorkoutSessionRepositoryUpdate(session);
            UnitOfWorkSave();
        }


        public void AttachWorkingSetNote(uint workoutId, uint excerciseProgressiveNumber, uint workingSetProgressiveNumber, uint? workingSetNoteId)
        {
            WorkoutSessionRoot session = WorkoutSessionRepositoryFind(workoutId);
            session.WriteWorkingSetNote(excerciseProgressiveNumber, workingSetProgressiveNumber, workingSetNoteId);

            WorkoutSessionRepositoryUpdate(session);
            UnitOfWorkSave();
        }

        public void RateExcercisePerformance(uint workoutId, uint excerciseProgressiveNumber, float rating)
        {
            WorkoutSessionRoot session = WorkoutSessionRepositoryFind(workoutId);
            session.RatePerformance(excerciseProgressiveNumber, RatingValue.Rate(rating));

            WorkoutSessionRepositoryUpdate(session);
            UnitOfWorkSave();
        }


        public void FinishWorkoutSession(uint workoutId, DateTime? when = null)
        {
            WorkoutSessionRoot session = WorkoutSessionRepositoryFind(workoutId);
            session.FinishWorkout(when ?? DateTime.UtcNow);

            WorkoutSessionRepositoryUpdate(session);
            UnitOfWorkSave();
        }


        public uint WriteWorkingSetNote(string body)
        {
            WorkingSetNoteRoot note = WorkingSetNoteRoot.WriteTransient(PersonalNoteValue.Write(body));
            _context.Add<WorkingSetNoteRoot>(note);

            UnitOfWorkSave();

            return note.Id.Value;
        }


        /// <summary>
        /// This is the simulation of what the Repository will do
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public WorkoutTemplateRoot WorkoutTemplateRepositoryFind(uint workoutId)
        {
            WorkoutTemplateRoot workout = null;


            //return _context.WorkoutTemplates.Find(workoutId);

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


        /// <summary>
        /// This is the simulation of what the Repository will do
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public WorkoutSessionRoot WorkoutSessionRepositoryUpdate(WorkoutSessionRoot session)
        {
            return _context.Update(session).Entity;
        }


        private TrainingPlanRoot TrainingPlanRepositoryFind(uint trainingPlanId)
        {
            TrainingPlanRoot plan = null;

            if (_enableEagerLoading)
            {
                // One query for all objects
                plan = _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
                    .Include(wo => wo.TrainingWeeks)
                    .SingleOrDefault();
            }
            else
            {
                // No query if object already tracked -> If State = Added, but contt not saved hte WO is still retrieved
                plan = _context.TrainingPlans.Find(trainingPlanId);

                if (plan != null)
                {
                    // One query for each Load -> wasteful
                    throw new NotImplementedException();
                }
            }
            return plan;
        }


        private WorkoutSessionRoot WorkoutSessionRepositoryFind(uint sessionId)
        {
            WorkoutSessionRoot session = null;

            if (_enableEagerLoading)
            {
                // One query for all objects
                session = _context.WorkoutSessions.Where(x => x.Id == sessionId)
                    .Include(wo => wo.WorkUnits)
                        .ThenInclude(wu => wu.WorkingSets)
                    .SingleOrDefault();
            }
            else
            {
                // No query if object already tracked -> If State = Added, but contt not saved hte WO is still retrieved
                session = _context.WorkoutSessions.Find(sessionId);

                if (session != null)
                {
                    // One query for each Load -> wasteful
                    throw new NotImplementedException();
                }
            }
            return session;
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
        public async void UnitOfWorkSave()
        {
            // Avoid double insert for tables that should not be tracked
            IgnoreStaticEnumTables();
            IgnoreEmbeddedValueObjects();
            DebugChangeTracker();
            await _context.SaveAsync();
        }



        private void IgnoreStaticEnumTables()
        {
            // Avoid double insert for tables that should not be tracked
            //foreach (var entity in _context.ChangeTracker.Entries<TrainingEffortTypeEnum>())
            //    entity.State = EntityState.Unchanged;

            //foreach (var entity in _context.ChangeTracker.Entries<Enumeration>())
            //    if(!(entity.Entity is WeekdayEnum))
            //        entity.State = EntityState.Unchanged;


            // Avoid double insert for tables that should not be tracked
            foreach (var entity in _context.ChangeTracker.Entries<Enumeration>())
            {
                try
                {
                    entity.State = EntityState.Unchanged;
                }
                catch(InvalidOperationException)
                {
                    // Enumeration as field, instead of separate table -> No need to set it as Unchanged - IE: see WeekdayEnum
                    continue;
                }
            }
        }

        private void IgnoreEmbeddedValueObjects()
        {
            // Value objects are part of the parent table
            foreach (var entity in _context.ChangeTracker.Entries<ValueObject>())
            {
                try
                {
                    if(entity.State == EntityState.Added)
                        entity.State = EntityState.Modified;
                }
                catch (InvalidOperationException)
                {
                    // Enumeration as field, instead of separate table -> No need to set it as Unchanged - IE: see WeekdayEnum
                    continue;
                }
            }
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

            //foreach (var entity in _context.ChangeTracker.Entries<WeekdayEnum>())
            //{
            //    Debugger.Break();           // Do what you need before the breakpoint

            //    var state = entity.State;

            //    if (state == EntityState.Added || state == EntityState.Deleted)
            //        entity.State = EntityState.Unchanged;

            //    Debugger.Break();           // Do what you need before the breakpoint
            //}

        }


    }
}
