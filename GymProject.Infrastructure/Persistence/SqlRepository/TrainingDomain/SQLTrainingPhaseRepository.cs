using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPhaseRepository : ITrainingPhaseRepository
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

        public SQLTrainingPhaseRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPhaseRoot Add(TrainingPhaseRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingPhaseRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingPhaseRoot>(trainingPlanId);
        }


        public TrainingPhaseRoot Modify(TrainingPhaseRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingPhaseRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
