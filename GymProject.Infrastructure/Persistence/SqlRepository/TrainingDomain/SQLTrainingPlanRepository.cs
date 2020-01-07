using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
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
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(x => x.TrainingWeeks)
            //        .Include(x => x.WorkoutIds)
            //        .SingleOrDefault();

            var res = _context.Find<TrainingPlanRoot>(id);

            if (res != null)
                _context.Entry(res).Collection(x => x.TrainingWeeks).Load();

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



    }
}
