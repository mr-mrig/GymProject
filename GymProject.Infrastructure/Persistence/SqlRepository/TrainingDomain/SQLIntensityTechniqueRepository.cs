using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLIntensityTechniqueRepository : IIntensityTechniqueRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLIntensityTechniqueRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public IntensityTechniqueRoot Add(IntensityTechniqueRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public IntensityTechniqueRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<IntensityTechniqueRoot>(trainingPlanId);
        }


        public IntensityTechniqueRoot Modify(IntensityTechniqueRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(IntensityTechniqueRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
