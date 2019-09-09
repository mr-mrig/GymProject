using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class MusReference : Entity<uint?>, ICloneable
    {


        public string Name { get; private set; }


        private MusReference(uint? id, string name) : base(id)
        {
            Id = id;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException($"Cannot create a MUS object with an empty name","name")
                : name;
        }


        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The MusId</param>
        /// <param name="name">The MUS name</param>
        /// <returns>Return a new instance of MusReference</returns>
        public static MusReference MusLink(uint? id, string name) => new MusReference(id, name);

        #endregion


        #region IClonable Implementation

        public object Clone()

            => MusLink(Id, Name);

        #endregion
    }
}
