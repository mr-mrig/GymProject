using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLTrainingPhaseRepository : ITrainingPhaseRepository
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

        public SQLTrainingPhaseRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public TrainingPhaseRoot Add(TrainingPhaseRoot phase)
        {
            return _context.Add(phase).Entity;
        }


        public TrainingPhaseRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();

            TrainingPhaseRoot res = db.Query<TrainingPhaseRoot, long?, TrainingPhaseRoot>(
                "SELECT Id, Name, EntryStatusId " +
                " FROM TrainingPhase  " +
                " WHERE Id = @id",
               (phase, entryStatusId) =>
               {
                   return TrainingPhaseRoot.CreateTrainingPhase(phase.Id,
                       phase.Name,
                       entryStatusId.HasValue ? EntryStatusTypeEnum.From((int)entryStatusId.Value) : null);
               },
               param: new { id },
               splitOn: "EntryStatusId")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

            return res;
        }


        public TrainingPhaseRoot Modify(TrainingPhaseRoot phase)
        {
            return _context.Update(phase).Entity;
        }


        public void Remove(TrainingPhaseRoot phase)
        {
            _context.Remove(phase);
        }
        #endregion

    }
}
