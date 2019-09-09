using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.FitnessJournalDomain.Exceptions;


namespace GymProject.Domain.FitnessJournalDomain.MusAggregate
{
    public class MusRoot : StatusTrackingEntity<uint?>, IAggregateRoot
    {


        public string Name { get; private set; }

        public string Description { get; private set; }




        #region Ctors

        private MusRoot() : base(null, null) { }

        private MusRoot(string name, string description = null, EntryStatusTypeEnum status = null) 
            : base(null, status)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new FitnessJournalDomainInvariantViolationException($"Trying to create a Mus with blank name");

            Name = name;
            Description = description;
        }
        #endregion


        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="name">MUS name - cannot be empty</param>
        /// <param name="description">MUS description</param>
        /// <param name="entryStatus">The status of the entry</param>
        /// <returns>The new Mus object</returns>
        public static MusRoot Diagnose(string name, string description = null, EntryStatusTypeEnum entryStatus = null)
        {
            return new MusRoot(name, description, entryStatus);
        }
        #endregion



        #region Business Methods

        // Inherited by StatusTrackingEntity
        #endregion

    }
}