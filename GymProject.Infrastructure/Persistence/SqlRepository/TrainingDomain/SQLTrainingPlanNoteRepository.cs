using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanNoteRepository : ITrainingPlanNoteRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLTrainingPlanNoteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPlanNoteRoot Add(TrainingPlanNoteRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public TrainingPlanNoteRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<TrainingPlanNoteRoot>(trainingPlanId);
        }


        public TrainingPlanNoteRoot Modify(TrainingPlanNoteRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(TrainingPlanNoteRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
