using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkUnitTemplateNoteRepository : IWorkUnitTemplateNoteRepository
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

        public SQLWorkUnitTemplateNoteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkUnitTemplateNoteRoot Add(WorkUnitTemplateNoteRoot note)
        {
            return _context.Add(note).Entity;
        }


        public WorkUnitTemplateNoteRoot Find(uint id)
        {
            var res = _context.Find<WorkUnitTemplateNoteRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.Body).Load();

            return res;
        }


        public WorkUnitTemplateNoteRoot Modify(WorkUnitTemplateNoteRoot note)
        {
            return _context.Update(note).Entity;
        }


        public void Remove(WorkUnitTemplateNoteRoot note)
        {
            _context.Remove(note);
        }
        #endregion

    }
}
