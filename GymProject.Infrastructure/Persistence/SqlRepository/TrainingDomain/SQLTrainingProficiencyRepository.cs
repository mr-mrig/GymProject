using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingProficiencyRepository : ITrainingProficiencyRepository
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

        public SQLTrainingProficiencyRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingProficiencyRoot Add(TrainingProficiencyRoot proficiency)
        {
            return _context.Add(proficiency).Entity;
        }


        public TrainingProficiencyRoot Find(uint id)
        {
            var res = _context.Find<TrainingProficiencyRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.EntryStatus).Load();

            return res;
        }


        public TrainingProficiencyRoot Modify(TrainingProficiencyRoot proficiency)
        {
            return _context.Update(proficiency).Entity;
        }


        public void Remove(TrainingProficiencyRoot proficiency)
        {
            _context.Remove(proficiency);
        }
        #endregion

    }
}
