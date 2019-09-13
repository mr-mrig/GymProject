using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class LinkedWorkValue : ValueObject
    {



        /// <summary>
        /// FK to the Intensity Technique which links the two Work Units
        /// </summary>
        public uint? LinkingIntensityTechniqueId { get; private set; } = null;


        /// <summary>
        /// FK to the Work Unit appended
        /// </summary>
        public uint? LinkedWorkId { get; private set; } = null;



        #region Ctors

        private LinkedWorkValue() { }


        private LinkedWorkValue(uint linkedWorkId, uint linkingIntensityTechniqueId)
        {
            LinkedWorkId = linkedWorkId;
            LinkingIntensityTechniqueId = linkingIntensityTechniqueId;
        }
        #endregion


        #region Factories

        /// <summary>
        /// Link the caller entity to the specified one by applying the Linking Intensity Technique
        /// Can be used both with WorkUnits and with Working Sets
        /// </summary>
        /// <param name="toBeLinkedId">The ID of the object to be linked</param>
        /// <param name="linkingIntensityTechniqueId">The ID of the Intensity Technique used for linking</param>
        /// <returns>The LinkedWorkValue instance</returns>
        public static LinkedWorkValue LinkTo(uint toBeLinkedId, uint linkingIntensityTechniqueId)

            => new LinkedWorkValue(toBeLinkedId, linkingIntensityTechniqueId);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return LinkedWorkId;
            yield return LinkingIntensityTechniqueId;
        }
    }
}
