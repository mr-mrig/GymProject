using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLAthleteRepository : IAthleteRepository
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

        public SQLAthleteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public AthleteRoot Add(AthleteRoot athlete)
        {
            return _context.Add(athlete).Entity;
        }


        public AthleteRoot Find(uint id)
        {
            var res = _context.Find<AthleteRoot>(id);

            if (res != null)
            {
                _context.Entry(res).Collection(x => x.TrainingPhases).Query().Include(x => x.EntryStatus).Load();
                _context.Entry(res).Collection(x => x.TrainingProficiencies).Load();
                _context.Entry(res).Collection(x => x.TrainingPlans).Query()
                    .Include("_trainingPlanPhases")
                    .Include("_trainingPlanProficiencies")
                    .Include("_trainingPlanMuscleFocusIds")
                    .Include("_trainingPlanHashtags")
                    .Load();

            }
            return res;
        }


        public AthleteRoot Modify(AthleteRoot athlete)
        {
            return _context.Update(athlete).Entity;
        }


        public void Remove(AthleteRoot athlete)
        {
            _context.Remove(athlete);
        }

        #endregion


        #region IAthleteRepository Implementation

        public int CountAthletesWithTrainingPlanInLibrary(uint trainingPlanId)
        
            => _context.Athletes.SelectMany(x => x.TrainingPlans).Where(x => x.TrainingPlanId == trainingPlanId).Count();
        
        #endregion
    }
}
