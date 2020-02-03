using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

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
            IDbConnection db = _context.Database.GetDbConnection();

            TrainingPlanNoteRoot res = db.Query<TrainingPlanNoteRoot, string, TrainingPlanNoteRoot>(
               "SELECT Id, Body" +
               " FROM TrainingPlanNote TPN" +
               " WHERE TPN.Id = @id",
               (note, body) =>
               {
                   return TrainingPlanNoteRoot.Write(note.Id, PersonalNoteValue.Write(body));
               },
               param: new { id },
               splitOn: "Body")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

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
