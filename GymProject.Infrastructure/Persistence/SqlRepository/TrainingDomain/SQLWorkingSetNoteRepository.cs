using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkingSetNoteRepository : IWorkingSetNoteRepository
    {


        private readonly GymContext _context;


        #region Ctors

        public SQLWorkingSetNoteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkingSetNoteRoot Add(WorkingSetNoteRoot trainingPlan)
        {
            return _context.Add(trainingPlan).Entity;
        }


        public WorkingSetNoteRoot Find(uint trainingPlanId)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            return _context.Find<WorkingSetNoteRoot>(trainingPlanId);
        }


        public WorkingSetNoteRoot Modify(WorkingSetNoteRoot trainingPlan)
        {
            return _context.Update(trainingPlan).Entity;
        }


        public void Remove(WorkingSetNoteRoot trainingPlan)
        {
            _context.Remove(trainingPlan);
        }
        #endregion

    }
}
