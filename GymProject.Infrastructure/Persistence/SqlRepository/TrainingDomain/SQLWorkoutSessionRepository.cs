using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkoutSessionRepository : IWorkoutSessionRepository
    {


        private readonly GymContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        #region Ctors

        public SQLWorkoutSessionRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkoutSessionRoot Add(WorkoutSessionRoot workout)
        {
            return _context.Add(workout).Entity;
        }


        public WorkoutSessionRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();
            Dictionary<uint?, WorkoutSessionRoot> lookup = new Dictionary<uint?, WorkoutSessionRoot>();

            WorkoutSessionRoot res = db.Query<WorkoutSessionRoot, WorkUnitEntity, double?, WorkingSetEntity, long?, double?, WorkoutSessionRoot>(
                "SELECT WO.Id, WO.StartTime, WO.EndTime, WO.WorkoutTemplateId," +
                " WU.Id, WU.ProgressiveNumber, WU.ExcerciseId, WU.Rating," +
                " WS.Id, WS.ProgressiveNumber, WS.NoteId, WS.Repetitions, WS.WeightKg as Load" +
                " FROM WorkoutSession WO" +
                " LEFT JOIN WorkUnit WU" +
                " ON WO.Id = WU.WorkoutSessionId" +
                " LEFT JOIN WorkingSet WS" +
                " ON WU.Id = WS.WorkUnitId" +
                " WHERE WO.Id = id",
              (wo, wu, rating, ws, reps, weight) =>
               {
                   WorkoutSessionRoot workout;

                   if (!lookup.TryGetValue(wo.Id, out workout))
                       lookup.Add(wo.Id, workout = wo);

                   // Work Units
                   if (wu?.Id != null && workout.WorkUnits.All(x => x.Id != wu.Id))
                   {
                       workout.TrackExcercise(wu);

                       if (rating.HasValue)
                           workout.RatePerformance(wu.ProgressiveNumber, RatingValue.Rate((float)rating));
                   }

                   // Working Sets
                   if (ws?.Id != null && workout.CloneAllWorkingSets().All(x => x.Id != ws.Id))
                   {
                       WorkingSetEntity workingSet = WorkingSetEntity.TrackWorkingSet(
                           ws.Id,
                           ws.ProgressiveNumber,
                           reps.HasValue ? WSRepetitionsValue.TrackRepetitionSerie((uint)reps.Value) : null,
                           weight.HasValue ? WeightPlatesValue.MeasureKilograms((float)weight) : null,
                           ws.NoteId);
                   }
                   return workout;
               },
               param: new { id },
               splitOn: "Id, Rating, Id, Repetitions, Load")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

            return res;
        }


        public WorkoutSessionRoot Modify(WorkoutSessionRoot workout)
        {
            return _context.Update(workout).Entity;
        }


        public void Remove(WorkoutSessionRoot workout)
        {
            _context.Remove(workout);
        }
        #endregion

    }
}
