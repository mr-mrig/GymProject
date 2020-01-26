using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanMessageRepository : ITrainingPlanMessageRepository
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

        public SQLTrainingPlanMessageRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPlanMessageRoot Add(TrainingPlanMessageRoot message)
        {
            return _context.Add(message).Entity;
        }


        public TrainingPlanMessageRoot Find(uint id)
        {
            var res = _context.Find<TrainingPlanMessageRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.Body).Load();

            return res;
        }


        public TrainingPlanMessageRoot Modify(TrainingPlanMessageRoot message)
        {
            return _context.Update(message).Entity;
        }


        public void Remove(TrainingPlanMessageRoot message)
        {
            _context.Remove(message);
        }
        #endregion

    }
}
