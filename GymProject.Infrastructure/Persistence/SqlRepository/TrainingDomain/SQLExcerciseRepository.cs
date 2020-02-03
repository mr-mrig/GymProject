using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

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
            IDbConnection db = _context.Database.GetDbConnection();

            ExcerciseRoot res = db.Query<ExcerciseRoot, string, long?, ExcerciseRoot>(
                "SELECT Id, Name, PrimaryMuscleId, Description, EntryStatusId" +
                " FROM Excercise" +
                " WHERE Id = @id",
               (exc, descr, entryStatusId) =>
               {
                   return ExcerciseRoot.AddToExcerciseLibrary(exc.Id,
                       exc.Name,
                       PersonalNoteValue.Write(descr),
                       exc.PrimaryMuscleId,
                       null,
                       entryStatusId.HasValue ? EntryStatusTypeEnum.From((int)entryStatusId.Value) : null);
               },
               param: new { id },
               splitOn: "Description, EntryStatusId")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

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
