using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPlanNoteRepository : ITrainingPlanNoteRepository
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

        public SQLTrainingPlanNoteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPlanNoteRoot Add(TrainingPlanNoteRoot note)
        {
            return _context.Add(note).Entity;
        }


        public TrainingPlanNoteRoot Find(uint id)
        {
            var res = _context.Find<TrainingPlanNoteRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.Body).Load();

            return res;
        }


        public TrainingPlanNoteRoot Modify(TrainingPlanNoteRoot note)
        {
            return _context.Update(note).Entity;
        }


        public void Remove(TrainingPlanNoteRoot note)
        {
            _context.Remove(note);
        }
        #endregion

    }
}
