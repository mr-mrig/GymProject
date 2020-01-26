using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingHashtagRepository : ITrainingHashtagRepository
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

        public SQLTrainingHashtagRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingHashtagRoot Add(TrainingHashtagRoot hashtag)
        {
            return _context.Add(hashtag).Entity;
        }


        public TrainingHashtagRoot Find(uint id)
        {
            var res = _context.Find<TrainingHashtagRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.EntryStatus).Load();

            return res;
        }


        public TrainingHashtagRoot Modify(TrainingHashtagRoot hashtag)
        {
            return _context.Update(hashtag).Entity;
        }


        public void Remove(TrainingHashtagRoot hashtag)
        {
            _context.Remove(hashtag);
        }
        #endregion

    }
}
