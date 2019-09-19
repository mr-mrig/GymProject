using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkoutSessionRepository : IWorkoutSessionRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLWorkoutSessionRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkoutSessionRoot Add(WorkoutSessionRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public WorkoutSessionRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<WorkoutSessionRoot>(trainingPlanId);
        }


        public WorkoutSessionRoot Modify(WorkoutSessionRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(WorkoutSessionRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
