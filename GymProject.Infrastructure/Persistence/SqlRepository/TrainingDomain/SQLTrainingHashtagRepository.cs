using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingHashtagRepository : ITrainingHashtagRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLTrainingHashtagRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingHashtagRoot Add(TrainingHashtagRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingHashtagRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingHashtagRoot>(trainingPlanId);
        }


        public TrainingHashtagRoot Modify(TrainingHashtagRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingHashtagRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
