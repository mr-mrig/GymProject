using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPhaseRepository : ITrainingPhaseRepository
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

        public SQLTrainingPhaseRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPhaseRoot Add(TrainingPhaseRoot phase)
        {
            return _context.Add(phase).Entity;
        }


        public TrainingPhaseRoot Find(uint id)
        {
            var res = _context.Find<TrainingPhaseRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.EntryStatus).Load();
            
            return res;
        }


        public TrainingPhaseRoot Modify(TrainingPhaseRoot phase)
        {
            return _context.Update(phase).Entity;
        }


        public void Remove(TrainingPhaseRoot phase)
        {
            _context.Remove(phase);
        }
        #endregion

    }
}
