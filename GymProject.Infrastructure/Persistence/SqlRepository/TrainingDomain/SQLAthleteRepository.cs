using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

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


        public AthleteRoot Find(uint athleteId)
        {
            return _context.Find<AthleteRoot>(athleteId);
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

    }
}
