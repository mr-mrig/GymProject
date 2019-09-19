using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkoutTemplateRepository : IWorkoutTemplateRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLWorkoutTemplateRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkoutTemplateRoot Add(WorkoutTemplateRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public WorkoutTemplateRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<WorkoutTemplateRoot>(trainingPlanId);
        }


        public WorkoutTemplateRoot Modify(WorkoutTemplateRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(WorkoutTemplateRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
