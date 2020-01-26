using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;

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
            var res = _context.Find<IntensityTechniqueRoot>(id);

            if (res != null)
                _context.Entry(res).Reference(x => x.EntryStatus).Load();

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
