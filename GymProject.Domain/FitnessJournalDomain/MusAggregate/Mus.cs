using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate;
using System.Collections.Generic;
using GymProject.Domain.FitnessJournalDomain.Exceptions;

namespace GymProject.Domain.FitnessJournalDomain.MusAggregate
{
    public class Mus : Entity, IAggregateRoot
    {


        public string Name { get; private set; }

        public string Description { get; private set; }


        public EntryStatusTypeEnum EntryStatusType { get; private set; }



        #region Ctors

        private Mus(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new FitnessJournalDomainGenericException($"Trying to create a Mus with blank name");

            Name = name;
            Description = description;
        }
        #endregion


        #region Factories

        public static Mus Diagnose(string name, string description = null)
        {
            return new Mus(name, description);
        }
        #endregion

    }
}