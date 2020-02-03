using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

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
            IDbConnection db = _context.Database.GetDbConnection();

            TrainingPlanMessageRoot res = db.Query<TrainingPlanMessageRoot, string, TrainingPlanMessageRoot>(
               "SELECT Id, Body" +
               " FROM TrainingPlanMessage TPM" +
               " WHERE TPM.Id = @id",
               (note, body) =>
               {
                   return TrainingPlanMessageRoot.Write(note.Id, PersonalNoteValue.Write(body));
               },
               param: new { id },
               splitOn: "Body")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

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
