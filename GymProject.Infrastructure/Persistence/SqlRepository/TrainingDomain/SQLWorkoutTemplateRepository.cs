using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkoutTemplateRepository : IWorkoutTemplateRepository
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


        public WorkoutTemplateRoot Find(uint workoutId)
        {
            //var res = _context.WorkoutTemplates.Where(x => x.Id == workoutId)
            //        .Include(wo => wo.WorkUnits)
            //            .ThenInclude(wu => wu.WorkingSets)
            //        .SingleOrDefault();

            var res = _context.Find<WorkoutTemplateRoot>(workoutId);

            if (res != null)
            {
                //_context.Entry(res).Collection(x => x.WorkUnits).Load();
                _context.Entry(res).Collection(x => x.WorkUnits).Query()
                    .Include(wu => wu.WorkingSets).Load();

            }
            return res;
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
