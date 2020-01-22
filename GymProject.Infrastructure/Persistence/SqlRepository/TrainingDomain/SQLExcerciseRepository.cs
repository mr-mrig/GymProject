using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLExcerciseRepository : IExcerciseRepository
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

        public SQLExcerciseRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public ExcerciseRoot Add(ExcerciseRoot excercise)
        {
            return _context.Add(excercise).Entity;
        }


        public ExcerciseRoot Find(uint id)
        {
            var res = _context.Find<ExcerciseRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.EntryStatus).Load();

            return res;
        }


        public ExcerciseRoot Modify(ExcerciseRoot excercise)
        {
            return _context.Update(excercise).Entity;
        }


        public void Remove(ExcerciseRoot excercise)
        {
            _context.Remove(excercise);
        }
        #endregion

    }
}
