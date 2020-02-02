using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanRepository : ITrainingPlanRepository
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

        public SQLTrainingPlanRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPlanRoot Add(TrainingPlanRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingPlanRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();
            Dictionary<uint?, TrainingPlanRoot> lookup = new Dictionary<uint?, TrainingPlanRoot>();

            TrainingPlanRoot res = db.Query<TrainingPlanRoot, TrainingWeekEntity, long?, long?, TrainingPlanRoot>(
                "SELECT TP.Id, TP.OwnerId, " +
                " TW.Id, TW.ProgressiveNumber, TW.TrainingWeekTypeId," +
                " WO.Id" +
                " FROM TrainingPlan TP" +
                " LEFT JOIN TrainingWeek TW" +
                " ON TW.TrainingPlanId = TP.Id" +
                " LEFT JOIN WorkoutTemplate WO" +
                " ON WO.TrainingWeekId = TW.Id" +
                " WHERE TP.Id = @id",
               map: (plan, week, weekTypeId, woId) =>
               {
                   TrainingPlanRoot trainingPlan;

                   if (!lookup.TryGetValue(plan.Id, out trainingPlan))
                       lookup.Add(plan.Id, trainingPlan = plan);

                   // Weeks
                   if (week?.Id != null && trainingPlan.TrainingWeeks.All(x => x.Id != week.Id))
                   {
                       trainingPlan.PlanTrainingWeek(TrainingWeekEntity.PlanTrainingWeek(
                           week.Id.Value, 
                           week.ProgressiveNumber,
                           null,
                           weekTypeId.HasValue ? TrainingWeekTypeEnum.From((int)weekTypeId.Value) : null));
                   }
                   if(woId.HasValue)
                    trainingPlan.PlanWorkout(week.ProgressiveNumber, (uint)woId.Value);

                   return trainingPlan;
               },
               param: new { id },
               splitOn: "Id, TrainingWeekTypeId, Id")
           .FirstOrDefault();

            // Do not attach to EF context again if already up-to-date
            //if (res != null && !_context.ChangeTracker.Entries<TrainingPlanRoot>().Any(x => x.Entity.Id == id))
            //    _context.Attach(res);       // Unfortunately we cannot just _context.Find(id)

            if (res != null)
                _context.Attach(res);

            return res;
        }

        public TrainingPlanRoot Modify(TrainingPlanRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingPlanRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion



        public TrainingPlanRoot FindWithEF(uint id)
        {
            var res = _context.Find<TrainingPlanRoot>(id);

            if (res != null)
                _context.Entry(res).Collection(x => x.TrainingWeeks).Load();

            return res;
        }
    }
}
