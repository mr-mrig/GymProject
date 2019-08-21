using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class MeasuresEntry : Entity<IdTypeValue>, IAggregateRoot
    {


        #region Properties

        /// <summary>
        /// Fitness Journal date
        /// </summary>
        public DateTime EntryDate { get; private set; }

        /// <summary>
        /// Rating
        /// </summary>
        public RatingValue Rating { get; private set; } = null;

        /// <summary>
        /// The bodyfat measured - If more measures type are pesente then choose the more precise one
        /// </summary>
        public BodyFatValue BodyFat { get; private set; } = null;

        /// <summary>
        /// The circumferences measures
        /// </summary>
        public CircumferenceMeasureValue CircumferencesMeasure { get; private set; } = null;

        /// <summary>
        /// the plicometry
        /// </summary>
        public PlicometryValue Plicometry { get; private set; } = null;

        /// <summary>
        /// The BIA analysis
        /// </summary>
        public BiaMeasureValue BiaMeasure { get; private set; } = null;

        /// <summary>
        /// The owner note
        /// </summary>
        public string OwnerNote { get; private set; } = null;

        // FK -> Don't fetch any other fields, as they might slow the process a lot
        public IdTypeValue PostId { get; private set; }

        /// <summary>
        /// Reference to the one who performed the measures check
        /// </summary>
        public Owner Owner { get; private set; }

        #endregion



        #region Ctors

        private MeasuresEntry(DateTime date, RatingValue rating, string ownerNote, Owner owner)
        {
            EntryDate = date;
            Rating = rating;
            OwnerNote = ownerNote;
            Owner = owner;
            BodyFat = null;
        }


        private MeasuresEntry(DateTime date, RatingValue rating)
        {
            EntryDate = date;
            Rating = rating;
            BodyFat = null;
        }
        #endregion



        #region Factories


        /// <summary>
        /// Factory for creating an entry with no measures attached
        /// </summary>
        /// <param name="dayDate">The date of the day to be tracked</param>
        /// <param name="rating">The rating</param>
        /// <returns>The new FitnessDay instance</returns>
        public static MeasuresEntry StartTrackingMeasures(DateTime dayDate, RatingValue rating = null)
            => new MeasuresEntry(dayDate, rating);

        #endregion



        #region Aggregate Root Methods

        /// <summary>
        /// Change the Rating of the entry
        /// </summary>
        /// <param name="newRating">The new rating</param>
        public void ChangeRating(RatingValue newRating)
        {
            Rating = newRating;
            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }

        /// <summary>
        /// Change the DayDate of the entry
        /// </summary>
        /// <param name="newDate">The new date</param>
        /// 
        public void ChangeDate(DateTime newDate)
        {
            EntryDate = newDate;
            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Changes the owner note
        /// </summary>
        /// <param name="newNote">The new note</param>
        public void UpdateNote(string newNote)
        {
            OwnerNote = newNote;
            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Changes the owner
        /// </summary>
        /// <param name="newOwner">The new owner</param>
        public void RegisterOwner(Owner newOwner)
        {
            Owner = newOwner;
            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Linke the entry to a Post
        /// </summary>
        /// <param name="postId">The ID of the post to be linked</param>
        public void LinkToPost(IdTypeValue postId)
        {
            PostId = postId;
        }

        #endregion


        #region Plicometry Methods

        /// <summary>
        /// Attach the plicometry to the measures entry, overriding the existing one if necessary.
        /// </summary>
        /// <param name="plicometry">The plicometry to be attached</param>
        public void AttachPlicometryCheck(PlicometryValue plicometry)
        {
            Plicometry = plicometry;
            BodyFat = GetBodyFat();

            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Deletes the plicometry check
        /// </summary>
        public void RemovePlicometry()
        {
            Plicometry = null;
            BodyFat = GetBodyFat();

            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));

            if (CheckNullMeasures())
                AddDomainEvent(new MeasuresHaveBeenClearedDomainEvent(this, PostId));
        }

        #endregion


        #region BIA Methods

        /// <summary>
        /// Attach the BIA  to the measures entry, overriding the existing one if necessary.
        /// </summary>
        /// <param name="plicometry">The BIA to be attached</param>
        public void AttachBiaCheck(BiaMeasureValue bia)
        {
            BiaMeasure = bia;
            BodyFat = GetBodyFat();

            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Deletes the BIA
        /// </summary>
        public void RemoveBia()
        {
            BiaMeasure = null;
            BodyFat = GetBodyFat();

            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));

            if (CheckNullMeasures())
                AddDomainEvent(new MeasuresHaveBeenClearedDomainEvent(this, PostId));
        }

        #endregion


        #region Circumferences Methods

        /// <summary>
        /// Attach the Circumferences check  to the measures entry, overriding the existing one if necessary.
        /// </summary>
        /// <param name="plicometry">The Circumferences check to be attached</param>
        public void AttachCircumferenceCheck(CircumferenceMeasureValue circumferences)
        {
            CircumferencesMeasure = circumferences;
            BodyFat = GetBodyFat();

            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Deletes the Circumferences check
        /// </summary>
        public void RemoveCircumferencesCheck()
        {
            CircumferencesMeasure = null;
            BodyFat = GetBodyFat();

            AddDomainEvent(new MeasuresChangedDomainEvent(this, PostId));

            if (CheckNullMeasures())
                AddDomainEvent(new MeasuresHaveBeenClearedDomainEvent(this, PostId));
        }

        #endregion


        /// <summary>
        /// Get the bodyfat with respect to the measures attached to the entry
        /// </summary>
        /// <returns>The body fat</returns>
        private BodyFatValue GetBodyFat()
        {
            if (BiaMeasure != null && BiaMeasure?.BF != null && BiaMeasure?.BF.Value > 0)
                return BiaMeasure.BF;

            if (Plicometry != null && Plicometry?.BF != null && Plicometry?.BF.Value > 0)
                return Plicometry.BF;

            return CircumferencesMeasure?.BF;
        }


        /// <summary>
        /// Checks wether all the measures entry are null -> invalid state
        /// </summary>
        /// <returns>True if all measures entries are null</returns>
        private bool CheckNullMeasures() => CircumferencesMeasure == null && Plicometry == null && BiaMeasure == null;


        private IEnumerable<object> GetAtomicValues()
        {
            yield return EntryDate;
            yield return Rating;
            yield return CircumferencesMeasure;
            yield return Plicometry;
            yield return BiaMeasure;
            yield return OwnerNote;
            yield return Owner;
            yield return PostId;
        }
    }
}
