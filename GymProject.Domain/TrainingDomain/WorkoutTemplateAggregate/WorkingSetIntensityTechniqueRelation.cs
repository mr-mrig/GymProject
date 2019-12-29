using GymProject.Domain.Base;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkingSetIntensityTechniqueRelation : RelationEntity
    {



        /// <summary>
        /// FK to the Training Plan
        /// </summary>
        public uint? WorkingSetId { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Working Set Template
        /// </summary>
        public WorkingSetTemplateEntity WorkingSet { get; private set; } = null;

        ///// <summary>
        ///// Navigation Property to the Working Set linked to the main one - Optional
        ///// </summary>
        //public WorkingSetTemplateEntity LinkedWorkingSet { get; private set; } = null;


        /// <summary>
        /// FK to the Training Plan
        /// </summary>
        public uint? LinkedWorkingSetId { get; private set; } = null;


        /// <summary>
        /// FK to the Intensity Technique
        /// </summary>
        public uint? IntensityTechniqueId { get; private set; } = null;




        #region Ctors

        private WorkingSetIntensityTechniqueRelation() { }


        private WorkingSetIntensityTechniqueRelation(WorkingSetTemplateEntity workingSet, uint? intensityTechniqueId, uint? linkedWorkingSetId = null)
        {
            WorkingSet = workingSet;
            //LinkedWorkingSet = linkedWorkingSet;
            IntensityTechniqueId = intensityTechniqueId;
            LinkedWorkingSetId = linkedWorkingSetId;
            WorkingSetId = workingSet.Id;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-MAny relation
        /// </summary>
        /// <param name="workingSet">The Working set which to apply the specified Intensity Technique to/param>
        /// <param name="intensityTechniqueId">The ID of the Intensity Technique</param>
        /// <param name="linkedWorkingSet">If the Intensity Technique links two WSs, this is the WS linked to the main one</param>
        /// <returns>The TrainingPlanMuscleFocus isntance</returns>
        public static WorkingSetIntensityTechniqueRelation BuildLink(WorkingSetTemplateEntity workingSet, uint? intensityTechniqueId, uint? linkedWorkingSetId = null)

            => new WorkingSetIntensityTechniqueRelation(workingSet, intensityTechniqueId, linkedWorkingSetId);


        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-MAny relation
        /// </summary>
        /// <param name="workingSet">The Working set which to apply the specified Intensity Technique to/param>
        /// <param name="intensityTechniqueId">The ID of the Intensity Technique</param>
        /// <param name="linkedWorkingSet">If the Intensity Technique links two WSs, this is the WS linked to the main one</param>
        /// <returns>The TrainingPlanMuscleFocus isntance</returns>
        public static WorkingSetIntensityTechniqueRelation BuildLink(WorkingSetTemplateEntity workingSet, LinkedWorkValue linkedWorkingSet)

            => BuildLink(workingSet, linkedWorkingSet.LinkingIntensityTechniqueId, linkedWorkingSet.LinkedWorkId);

        #endregion



        /// <summary>
        /// Get the linked entity or default, if this is not a linking relation
        /// </summary>
        /// <returns>The LinkedWorkValue instance representing the Link Relation</returns>
        public LinkedWorkValue GetLinkedOrDefault()

            => LinkedWorkingSetId == null
                ? default
                : LinkedWorkValue.LinkTo(LinkedWorkingSetId.Value, IntensityTechniqueId.Value);

        //=> LinkedWorkingSet == null
        //    ? default
        //    : LinkedWorkValue.LinkTo(LinkedWorkingSet.Id.Value, IntensityTechniqueId.Value);



    }
}
