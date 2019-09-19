using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanRepository : ITrainingPlanRepository
    {


        private readonly GymContext _context;


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


        public TrainingPlanRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingPlanRoot>(trainingPlanId);
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

    }
}
