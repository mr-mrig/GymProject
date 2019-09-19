using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkUnitTemplateNoteRepository : IWorkUnitTemplateNoteRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLWorkUnitTemplateNoteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkUnitTemplateNoteRoot Add(WorkUnitTemplateNoteRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public WorkUnitTemplateNoteRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<WorkUnitTemplateNoteRoot>(trainingPlanId);
        }


        public WorkUnitTemplateNoteRoot Modify(WorkUnitTemplateNoteRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(WorkUnitTemplateNoteRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
