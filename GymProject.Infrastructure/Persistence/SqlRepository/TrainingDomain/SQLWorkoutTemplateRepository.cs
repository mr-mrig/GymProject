using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Data;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkoutTemplateRepository : IWorkoutTemplateRepository
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

        public SQLWorkoutTemplateRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkoutTemplateRoot Add(WorkoutTemplateRoot workout)
        {
            return _context.Add(workout).Entity;
        }


        public WorkoutTemplateRoot Find(uint workoutId)
        {

            // Perform the query only if the entity is not already loaded into the context
            if (_context.ChangeTracker.Entries<WorkoutTemplateRoot>().Count(x => x.Entity.Id == workoutId) > 0)
                return _context.WorkoutTemplates.Find(workoutId);   //RIGM: are we sure this always works?

            IDbConnection db = _context.Database.GetDbConnection();
            Dictionary<uint?, WorkoutTemplateRoot> workoutsDictionary = new Dictionary<uint?, WorkoutTemplateRoot>();

            WorkoutTemplateRoot res = db.Query(
               "SELECT WO.Id, WO.ProgressiveNumber, WO.Name, WO.TrainingWeekId, WO.SpecificWeekday as SpecificWeekdayId," +
               " WU.Id, WU.ProgressiveNumber, WU.LinkingIntensityTechniqueId, WU.ExcerciseId, WU.WorkUnitNoteId," +
               " WS.Id, WS.ProgressiveNumber, WS.Cadence as TUT, WS.Rest, WS.TargetRepetitions as Repetitions," +
               " WS.Repetitions_WorkTypeId as WorkTypeId, " +
               " CAST(WS.Effort AS Real) as Effort, WS.Effort_EffortTypeId as EffortTypeId, WSIT.IntensityTechniqueId" +
               " FROM WorkoutTemplate WO" +
               " LEFT JOIN WorkUnitTemplate WU" +
               " ON WO.Id = WU.WorkoutTemplateId" +
               " LEFT JOIN WorkingSetTemplate WS" +
               " ON WU.Id = WS.WorkUnitTemplateId" +
               " LEFT JOIN WorkingSetIntensityTechnique WSIT" +
               " ON WS.Id = WSIT.WorkingSetId" +
               " WHERE WO.Id = @workoutId",
               types: new[]
               {
                    typeof(WorkoutTemplateRoot),
                    typeof(long?),
                    typeof(WorkUnitTemplateEntity),
                    typeof(long?),
                    typeof(WorkingSetTemplateEntity),
                    typeof(string),
                    typeof(long?),
                    typeof(long?),
                    typeof(long?),
                    typeof(float?),
                    typeof(long?),
                    typeof(long?)
               },
               map: objects =>
               {
                   WorkoutTemplateRoot workout;

                   WorkoutTemplateRoot wo = objects[0] as WorkoutTemplateRoot;
                   long? weekId = objects[1] as long?;
                   WorkUnitTemplateEntity wu = objects[2] as WorkUnitTemplateEntity;
                   long? wuNoteId = objects[3] as long?;
                   WorkingSetTemplateEntity ws = objects[4] as WorkingSetTemplateEntity;
                   string tut = objects[5] as string;
                   long? rest = objects[6] as long?;
                   long? reps = objects[7] as long?;
                   long? workId = objects[8] as long?;
                   double? effort = objects[9] as double?;
                   long? effortTypeId = objects[10] as long?;
                   long? techniqueId = objects[11] as long?;

                   if (!workoutsDictionary.TryGetValue(wo.Id, out workout))
                   {
                       workoutsDictionary.Add(wo.Id, workout = wo);

                       if (weekId.HasValue)
                           workout.ScheduleToSpecificDay(WeekdayEnum.From((int)weekId.Value));
                   }

                   // Work Units
                   if (wu?.Id != null && workout.WorkUnits.Count(x => x.Id == wu.Id) == 0)
                   {
                       workout.PlanExcercise(wu);
                       workout.AttachWorkUnitNote(wu.ProgressiveNumber, (uint?)wuNoteId); 
                   }

                   // Working Sets
                   if (ws?.Id != null && workout.CloneAllWorkingSets().Count(x => x.Id == ws.Id) == 0)
                   {
                       workout.AddWorkingSet(wu.ProgressiveNumber, ws);

                       if (!string.IsNullOrEmpty(tut))
                           workout.ReviseWorkingSetLiftingTempo(wu.ProgressiveNumber, ws.ProgressiveNumber, TUTValue.PlanTUT(tut));

                       if (rest.HasValue)
                           workout.ReviseWorkingSetRestPeriod(wu.ProgressiveNumber, ws.ProgressiveNumber, RestPeriodValue.SetRest((int)rest.Value, RestPeriodValue.DefaultRestMeasUnit));

                       if (effort.HasValue)
                       {
                           TrainingEffortTypeEnum effortType = effortTypeId.HasValue ? TrainingEffortTypeEnum.From((int)effortTypeId.Value) : null;
                           workout.ReviseWorkingSetEffort(wu.ProgressiveNumber, ws.ProgressiveNumber, TrainingEffortValue.FromEffort((float)effort.Value, effortType));
                       }
                       if (reps.HasValue)
                       {
                           WSWorkTypeEnum workType = workId.HasValue ? WSWorkTypeEnum.From((int)workId.Value) : null;
                           workout.ReviseWorkingSetRepetitions(wu.ProgressiveNumber, ws.ProgressiveNumber, WSRepetitionsValue.TrackWork((int)reps.Value, workType));
                       }

                   }
                   // Intensity Techniques
                   if (techniqueId.HasValue)
                       workout.AddWorkingSetIntensityTechnique(wu.ProgressiveNumber, ws.ProgressiveNumber, (uint)techniqueId.Value);

                   return workout;
               },
               param: new { workoutId },
               splitOn: "SpecificWeekdayId, Id, WorkUnitNoteId, Id, TUT, Rest, Repetitions, WorkTypeId, Effort, EffortTypeId, IntensityTechniqueId")
           .FirstOrDefault();

            _context.Attach(res);
            return res;
        }

        public WorkoutTemplateRoot Modify(WorkoutTemplateRoot workout)
        {
            return _context.Update(workout).Entity;
        }


        public void Remove(WorkoutTemplateRoot workout)
        {
            _context.Remove(workout);
        }
        #endregion


        public WorkoutTemplateRoot FindWithEF(uint workoutId)
        {
            var res2 = _context.Find<WorkoutTemplateRoot>(workoutId);

            if (res2 != null)
            {
                _context.Entry(res2).Collection(x => x.WorkUnits).Query().OfType<WorkUnitTemplateEntity>()
                    .Include(wu => wu.WorkingSets)
                        .ThenInclude(ws => ws.Repetitions)
                            .ThenInclude(rep => rep.WorkType)
                    .Include(wu => wu.WorkingSets)
                        .ThenInclude(ws => ws.Effort)
                            .ThenInclude(eff => eff.EffortType)
                    .Load();
            }

            return res2;
        }
    }
}
