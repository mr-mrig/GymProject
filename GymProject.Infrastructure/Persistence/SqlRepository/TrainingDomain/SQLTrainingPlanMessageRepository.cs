using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanMessageRepository : ITrainingPlanMessageRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLTrainingPlanMessageRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPlanMessageRoot Add(TrainingPlanMessageRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingPlanMessageRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingPlanMessageRoot>(trainingPlanId);
        }


        public TrainingPlanMessageRoot Modify(TrainingPlanMessageRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingPlanMessageRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
