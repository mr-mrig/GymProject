using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkoutSessionRepository : IWorkoutSessionRepository
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


        public WorkoutSessionRoot Find(uint id)
        {
            //return _context.TrainingPlans.Where(x => x.Id == trainingPlanId)
            //        .Include(wo => wo.TrainingWeeks)
            //        .SingleOrDefault();

            var res = _context.Find<WorkoutSessionRoot>(id);

            if (res != null)
            {
                //_context.Entry(res).Collection(x => x.WorkUnits).Load();
                _context.Entry(res).Collection(x => x.WorkUnits).Query()
                    .Include(wu => wu.WorkingSets)
                    .Load();

            }
            return res;
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
