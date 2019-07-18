using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class MeasuresEntry : Entity<IdType>, IAggregateRoot
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
        public string OwnerNote { get; private set; } = string.Empty;

        // FK -> Don't fetch any other fields, as they might slow the process a lot
        public long PostId { get; private set; }

        /// <summary>
        /// Reference to the owner of the check
        /// </summary>
        public Owner Owner { get; private set; }

        #endregion

    }
}
