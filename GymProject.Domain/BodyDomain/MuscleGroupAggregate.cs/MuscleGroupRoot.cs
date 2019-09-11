using GymProject.Domain.Base;
using GymProject.Domain.BodyDomain.Exceptions;
using System;

namespace GymProject.Domain.BodyDomain.MuscleGroupAggregate
{
    public class MuscleGroupRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {




        /// <summary>
        /// The identifying name
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The description
        /// </summary>
        public string Abbreviation { get; private set; } = string.Empty;





        #region Ctors

        private MuscleGroupRoot() : base(null) { }


        private MuscleGroupRoot(uint? id, string name, string abbreviation) : base(id)
        {
            Name = name?.Trim() ?? string.Empty;
            Abbreviation = abbreviation?.Trim() ?? string.Empty;
            //OwnerId = ownerId;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method - For adding new objects
        /// </summary>
        /// <param name="id">The object ID</param>
        /// <param name="name">The identifying name</param>
        /// <param name="abbreviation">The tag used when abbraviating the name</param>
        /// <param name="ownerId">The User which the phase was created by</param>
        /// <returns>The Excercise instance</returns>
        public static MuscleGroupRoot AddToMusclesLibrary
        (
            uint? id,
            string name,
            string abbreviation
        )
            => new MuscleGroupRoot(id, name, abbreviation);


        /// <summary>
        /// Factory method - For adding transient objects
        /// </summary>
        /// <param name="name">The identifying name</param>
        /// <param name="abbreviation">The tag used when abbraviating the name</param>
        /// <param name="ownerId">The User which the phase was created by</param>
        /// <returns>The Excercise instance</returns>
        public static MuscleGroupRoot AddToMusclesLibrary
        (
            string name,
            string abbreviation
        )
            => new MuscleGroupRoot(null, name, abbreviation);

        #endregion



        #region Business Methods
        #endregion




        #region Business Rules Validation

        /// <summary>
        /// The Training Phase must have a valid name.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NameIsMandatory() => !string.IsNullOrWhiteSpace(Name);


        /// <summary>
        /// The Training Phase requires the Entry Status to be set.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AbbreviationIsMandatory() => !string.IsNullOrWhiteSpace(Abbreviation);




        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="BodyDomainInvariantViolationException">Thrown if business rules violation</exception>
        protected void TestBusinessRules()
        {
            if (!NameIsMandatory())
                throw new BodyDomainInvariantViolationException($"The Excercise must have a valid name.");

            if (!AbbreviationIsMandatory())
                throw new BodyDomainInvariantViolationException($"The Excercise must have a valid name.");
        }

        #endregion


        #region ICloneable Implementation

        public object Clone()

            => AddToMusclesLibrary(Name, Abbreviation);

        #endregion


    }
}