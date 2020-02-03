using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

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
            IDbConnection db = _context.Database.GetDbConnection();

            TrainingProficiencyRoot res = db.Query<TrainingProficiencyRoot, string, long?, TrainingProficiencyRoot>(
                "SELECT Id, Name, Description, EntryStatusId " +
                " FROM TrainingProficiency " +
                " WHERE Id = @id",
               (prof, descr, entryStatusId) =>
               {
                   return TrainingProficiencyRoot.CreateTrainingProficiency(prof.Id,
                       prof.Name,
                       descr != null ? PersonalNoteValue.Write(descr) : null,
                       entryStatusId.HasValue ? EntryStatusTypeEnum.From((int)entryStatusId.Value) : null);
               },
               param: new { id },
               splitOn: "Description, EntryStatusId")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

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
