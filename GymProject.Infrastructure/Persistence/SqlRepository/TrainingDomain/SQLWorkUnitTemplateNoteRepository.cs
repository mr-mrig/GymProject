using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

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
            IDbConnection db = _context.Database.GetDbConnection();

            WorkUnitTemplateNoteRoot res = db.Query<WorkUnitTemplateNoteRoot, string, WorkUnitTemplateNoteRoot>(
               "SELECT Id, Body" + 
               " FROM WorkUnitTemplateNote WUN" +
               " WHERE WUN.Id = @id",
               (note, body) =>
               {
                   return WorkUnitTemplateNoteRoot.Write(note.Id, PersonalNoteValue.Write(body));
               },
               param: new { id },
               splitOn: "Body")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

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
