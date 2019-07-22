using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.FitnessJournalDomain.Exceptions;


namespace GymProject.Domain.FitnessJournalDomain.MusAggregate
{
    public class Mus : StatusTrackingEntity, IAggregateRoot
    {


        public string Name { get; private set; }

        public string Description { get; private set; }




        #region Ctors

        private Mus(string name, string description = null)
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
        /// <returns>The new Mus object</returns>
        public static Mus Diagnose(string name, string description = null)
        {
            return new Mus(name, description);
        }
        #endregion



        #region Business Methods

        // Inherited by StatusTrackingEntity
        #endregion

    }
}