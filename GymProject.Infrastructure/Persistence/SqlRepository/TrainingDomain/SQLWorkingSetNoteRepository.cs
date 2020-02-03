using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLWorkingSetNoteRepository : IWorkingSetNoteRepository
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

        public SQLWorkingSetNoteRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public WorkingSetNoteRoot Add(WorkingSetNoteRoot note)
        {
            return _context.Add(note).Entity;
        }


        public WorkingSetNoteRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();

            WorkingSetNoteRoot res = db.Query<WorkingSetNoteRoot, string, WorkingSetNoteRoot>(
               "SELECT Id, Body" +
               " FROM WorkingSetNote WSN" +
               " WHERE WSN.Id = @id",
               (note, body) =>
               {
                   return WorkingSetNoteRoot.Write(note.Id, PersonalNoteValue.Write(body));
               },
               param: new { id },
               splitOn: "Body")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

            return res;
        }


        public WorkingSetNoteRoot Modify(WorkingSetNoteRoot note)
        {
            return _context.Update(note).Entity;
        }


        public void Remove(WorkingSetNoteRoot note)
        {
            _context.Remove(note);
        }
        #endregion

    }
}
