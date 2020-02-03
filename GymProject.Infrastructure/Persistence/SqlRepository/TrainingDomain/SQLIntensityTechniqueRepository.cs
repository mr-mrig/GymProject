using Dapper;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain
{
    public class SQLIntensityTechniqueRepository : IIntensityTechniqueRepository
    {


        private readonly GymContext _context;

        public IUnitOfWork UnitOfWork
        {
            get => _context;
        }



        #region Ctors

        public SQLIntensityTechniqueRepository(GymContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context), "Cannot instantiate a Repository on a NULL DB context");
        }
        #endregion



        #region IRepository Implementation

        public IntensityTechniqueRoot Add(IntensityTechniqueRoot technique)
        {
            return _context.Add(technique).Entity;
        }


        public IntensityTechniqueRoot Find(uint id)
        {
            IDbConnection db = _context.Database.GetDbConnection();

            IntensityTechniqueRoot res = db.Query<IntensityTechniqueRoot, string, long?, IntensityTechniqueRoot>(
                "SELECT Id, Name, Abbreviation, IsLinkingTechnique, OwnerId, Description, EntryStatusId " +
                " FROM IntensityTechnique " +
                " WHERE Id = @id",
               (it, descr, entryStatusId) =>
               {
                   return IntensityTechniqueRoot.CreateIntensityTechnique(it.Id,
                       it.OwnerId,
                       it.Name,
                       it.Abbreviation,
                       PersonalNoteValue.Write(descr),
                       it.IsLinkingTechnique,
                       entryStatusId.HasValue ? EntryStatusTypeEnum.From((int)entryStatusId.Value) : null);
               },
               param: new { id },
               splitOn: "Description, EntryStatusId")
           .FirstOrDefault();

            if (res != null)
                _context.Attach(res);

            return res;
        }


        public IntensityTechniqueRoot Modify(IntensityTechniqueRoot technique)
        {
            return _context.Update(technique).Entity;
        }


        public void Remove(IntensityTechniqueRoot technique)
        {
            _context.Remove(technique);
        }
        #endregion

    }
}
