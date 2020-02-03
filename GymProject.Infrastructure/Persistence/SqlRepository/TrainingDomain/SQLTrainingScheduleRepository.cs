using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingScheduleRepository : ITrainingScheduleRepository
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

        public SQLTrainingScheduleRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingScheduleRoot Add(TrainingScheduleRoot schedule)
        {
            return _context.Add(schedule).Entity;
        }


        public TrainingScheduleRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();
            Dictionary<uint?, TrainingScheduleRoot> lookup = new Dictionary<uint?, TrainingScheduleRoot>();

            TrainingScheduleRoot res = db.Query<TrainingScheduleRoot, TrainingScheduleFeedbackEntity, double?, string, TrainingScheduleRoot>(
                "SELECT TS.Id, TS.StartDate, TS.EndDate, TS.TrainingPlanId, TS.AthleteId," +
                " TF.Id, TF.UserId," +
                " TF.Rating, TF.Comment" +
                " FROM TrainingSchedule TS" +
                " LEFT JOIN TrainingScheduleFeedback TF" +
                " ON TS.Id = TF.TrainingScheduleId" +
                " WHERE TS.Id = @id",
              (sched, fbk, rating, comment) =>
              {
                  TrainingScheduleRoot schedule;

                  if (!lookup.TryGetValue(sched.Id, out schedule))
                      lookup.Add(sched.Id, schedule = sched);

                  // Feebacks
                  if (fbk?.Id != null)
                  {
                      RatingValue ratingValue = rating.HasValue ? RatingValue.Rate((float)rating.Value) : null;
                      PersonalNoteValue commentValue = comment != null ? PersonalNoteValue.Write(comment) : null;

                      schedule.ProvideFeedback(TrainingScheduleFeedbackEntity.ProvideFeedback(fbk.Id, fbk.UserId, ratingValue, commentValue));
                  }
                  return schedule;
              },
               param: new { id },
               splitOn: "Id, Rating, Comment")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

            return res;
        }


        public TrainingScheduleRoot Modify(TrainingScheduleRoot schedule)
        {
            return _context.Update(schedule).Entity;
        }


        public void Remove(TrainingScheduleRoot schedule)
        {
            _context.Remove(schedule);
        }
        #endregion


        public TrainingScheduleRoot GetCurrentScheduleByAthleteOrDefault(uint athleteId)
        {
            return _context.TrainingSchedules
                .Where(x => x.AthleteId == athleteId && x.EndDate > DateTime.UtcNow.Date)
                .FirstOrDefault();
        }

    }
}
