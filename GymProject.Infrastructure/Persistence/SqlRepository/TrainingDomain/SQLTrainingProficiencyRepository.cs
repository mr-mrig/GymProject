using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingProficiencyRepository : ITrainingProficiencyRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLTrainingProficiencyRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingProficiencyRoot Add(TrainingProficiencyRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingProficiencyRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingProficiencyRoot>(trainingPlanId);
        }


        public TrainingProficiencyRoot Modify(TrainingProficiencyRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingProficiencyRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
