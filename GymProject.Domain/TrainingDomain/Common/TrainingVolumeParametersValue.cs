using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using GymProject.Domain.Utils;
using System.Linq;
using GymProject.Domain.Utils.Extensions;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingVolumeParametersValue : ValueObject
    {




        /// <summary>
        /// The total number of repetitions
        /// </summary>
        public int TotalReps { get; private set; } = 0;

        /// <summary>
        /// Number of working sets
        /// </summary>
        public int TotalWorkingSets { get; private set; } = 0;

        /// <summary>
        /// Total Workload as repetitions * weight - For effective WS only
        /// </summary>
        public WeightPlatesValue TotalWorkload { get; private set; }



        #region Ctors

        private TrainingVolumeParametersValue(int totalReps, int totalWorkingSets, WeightPlatesValue totalWorkload)
        {
            TotalReps = totalReps;
            TotalWorkingSets = totalWorkingSets;
            TotalWorkload = totalWorkload ?? WeightPlatesValue.MeasureKilograms(0);

            //TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="totalReps">The number of repetitions</param>
        /// <param name="totalWorkload">The workload</param>
        /// <param name="totalWorkingSets">The number of working sets</param>
        /// <returns>The TrainingVolumeValue instance</returns>
        public static TrainingVolumeParametersValue SetTrainingVolume(uint totalReps, uint totalWorkingSets, WeightPlatesValue totalWorkload)

            => new TrainingVolumeParametersValue((int)totalReps, (int)totalWorkingSets, totalWorkload);


        /// <summary>
        /// Facotry method - Computes the volume parameters from a list of WSs
        /// </summary>
        /// <param name="workingSets">The working sets</param>
        /// <exception cref="ArgumentNullException">Thrown if input contains NULL elements</exception>
        /// <returns>The TrainingVolumeValue instance</returns>
        public static TrainingVolumeParametersValue ComputeFromWorkingSets(IEnumerable<IWorkingSet> workingSets)
        {
            List<IWorkingSet> wsCopy = workingSets?.Clone().ToList() ?? new List<IWorkingSet>();

            if (wsCopy.Count() == 0)
                return SetTrainingVolume(0, 0, WeightPlatesValue.MeasureKilograms(0));

            // Exception or exclude from computation?
            if (wsCopy.Any(x => x == null))
                throw new ArgumentNullException($"Trying to compute the Training Volume on null-containing WS list");


            return SetTrainingVolume(
                (uint)wsCopy.Sum(x => x.ToRepetitions()),
                (uint)wsCopy.Count(),
                WeightPlatesValue.Measure(wsCopy.Sum(x => x.ToWorkload().Value), wsCopy.FirstOrDefault().ToWorkload().Unit));
        }

        #endregion



        #region Business Rules Validations

        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            //if (!ValidEffortBoundaries())
            //    throw new TrainingDomainInvariantViolationException($"");
        }
        #endregion



        #region Private Methods
        #endregion


        #region Public Methods

        /// <summary>
        /// Updates the Volume parameters including the working set added
        /// </summary>
        /// <param name="toAdd">The working set to be added</param>
        /// <returns>The new TrainingVolumeValue instance</returns>
        public TrainingVolumeParametersValue AddWorkingSet(IWorkingSet toAdd)

            => SetTrainingVolume(
                (uint)(TotalReps + toAdd.ToRepetitions()),
                (uint)(TotalWorkingSets + 1),
                TotalWorkload + toAdd.ToWorkload());
        

        /// <summary>
        /// Updates the Volume parameters excluding the working set removed
        /// </summary>
        /// <param name="toRemove">The working set to be removed</param>
        /// <exception cref="ArgumentException">Thrown when the WS can't be removed</exception>
        /// <returns>The new TrainingVolumeValue instance</returns>
        public TrainingVolumeParametersValue RemoveWorkingSet(IWorkingSet toRemove)
        {
            if (TotalWorkingSets == 0)
                throw new ArgumentException($"Trying to remove a WorkingSet when no one has been added.");

            return SetTrainingVolume(
                (uint)(TotalReps - toRemove.ToRepetitions()),
                (uint)(TotalWorkingSets - 1),
                TotalWorkload + toRemove.ToWorkload());
        }


        /// <summary>
        /// Get the average repetitions number - the result is exact IE: has decimal values
        /// </summary>
        /// <returns>The average repetitions over the WSs</returns>
        public float GetAverageRepetitions() => TotalWorkingSets == 0 ? 0 : (float)TotalReps / (float)TotalWorkingSets;


        ///// <summary>
        ///// Get the average repetitions object . the result is truncated IE: no decimal places
        ///// </summary>
        ///// <returns>The average repetitions over the WSs</returns>
        //public WSRepetitionValue GetAverageRepetitions() => WSRepetitionValue.TrackRepetitionSerie((uint)(TotalReps / TotalWorkingSets));


        /// <summary>
        /// Get the average workload per WSs
        /// </summary>
        /// <returns>The average repetitions over the WSs</returns>
        public WeightPlatesValue GetAverageWorkloadPerSet() 
            
            => WeightPlatesValue.Measure(TotalWorkingSets == 0 ? 0 : (float)TotalWorkload.Value / (float)TotalWorkingSets, TotalWorkload.Unit);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TotalReps;
            yield return TotalWorkingSets;
            yield return TotalWorkload;
        }
    }
}