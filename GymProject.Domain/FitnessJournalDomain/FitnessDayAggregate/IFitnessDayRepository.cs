using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class IFitnessDayRepository : IRepository<FitnessDay>
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public void Add(FitnessDay entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FitnessDay> FindAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(FitnessDay entity)
        {
            throw new NotImplementedException();
        }
    }
}
