using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
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

        public TrainingScheduleRoot Add(TrainingScheduleRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingScheduleRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingScheduleRoot>(trainingPlanId);
        }


        public TrainingScheduleRoot Modify(TrainingScheduleRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingScheduleRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
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
