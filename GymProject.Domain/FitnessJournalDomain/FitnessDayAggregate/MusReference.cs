using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class MusReference : Entity<IdType>
    {


        public string Name { get; private set; }


        private MusReference(IdType id, string name)
        {
            Id = id;
            Name = name;
        }


        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The MusId</param>
        /// <param name="name">The MUS name</param>
        /// <returns>Return a new instance of MusReference</returns>
        public static MusReference MusLink(IdType id, string name) => new MusReference(id, name);
        #endregion

    }
}
