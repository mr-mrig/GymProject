using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingDensityParametersValue : ValueObject
    {



        private int _totalWorkingSets = 0;




        /// <summary>
        /// Total rest time between sets [s]
        /// </summary>
        public int TotalRest { get; private set; } = 0;

        /// <summary>
        /// Total time spent workingout [s]
        /// </summary>
        public int TotalSecondsUnderTension { get; private set; } = 0;





        #region Ctors

        private TrainingDensityParametersValue(int totalTimeUnderTension, int totalRest, int totalWorkingSets)
        {
            TotalSecondsUnderTension = totalTimeUnderTension;
            TotalRest = totalRest;
            _totalWorkingSets = totalWorkingSets;

            //TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="totalTimeUnderTension">The number of repetitions</param>
        /// <param name="totalRest">The number of working sets</param>
        /// <returns>The TrainingDensityValue instance</returns>
        public static TrainingDensityParametersValue SetTrainingDensity(int totalTimeUnderTension, int totalRest, int workingSetsNumber)

            => new TrainingDensityParametersValue(totalTimeUnderTension, totalRest, workingSetsNumber);


        /// <summary>
        /// Facotry method - Computes the volume parameters from a list of WSs
        /// </summary>
        /// <param name="workingSets">The working sets</param>
        /// <exception cref="ArgumentNullException">Thrown if input contains NULL elements</exception>
        /// <returns>The TrainingDensityValue instance</returns>
        public static TrainingDensityParametersValue ComputeFromWorkingSets(IEnumerable<IWorkingSet> workingSets)
        {
            List<IWorkingSet> wsCopy = workingSets.Clone().ToList() ?? new List<IWorkingSet>();

            if (wsCopy.Count() == 0)
                return new TrainingDensityParametersValue(0, 0, 0);


            // Exception or exclude from computation?
            if (wsCopy.Any(x => x == null))
                throw new ArgumentNullException($"Trying to compute the Training Density on null-containing WS list");


            return SetTrainingDensity(
                wsCopy.Sum(x => x.ToSecondsUnderTension()),
                wsCopy.Sum(x => x.Rest.Value),
                wsCopy.Count());
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
        /// Updates the Density parameters including the working set added
        /// </summary>
        /// <param name="toAdd">The working set to be added</param>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingDensityParametersValue AddWorkingSet(IWorkingSet toAdd)

            => SetTrainingDensity(
                TotalSecondsUnderTension + toAdd.ToTotalSeconds(),
                TotalRest + toAdd.Rest.Value,
                ++_totalWorkingSets);


        /// <summary>
        /// Updates the Density parameters excluding the working set removed
        /// </summary>
        /// <param name="toRemove">The working set to be removed</param>
        /// <exception cref="ArgumentException">Thrown when the WS can't be removed</exception>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingDensityParametersValue RemoveWorkingSet(IWorkingSet toRemove)
        {
            if (_totalWorkingSets == 0)
                throw new ArgumentException($"Trying to remove a WorkingSet when no one has been added.");

            return SetTrainingDensity(
                TotalSecondsUnderTension - toRemove.ToTotalSeconds(),
                TotalRest - toRemove.Rest.Value,
                --_totalWorkingSets);
        }


        /// <summary>
        /// Get the average rest between sets
        /// </summary>
        /// <returns>The average rest over the WSs</returns>
        public float GetAverageRest() => (float)TotalRest / (float)_totalWorkingSets;


        /// <summary>
        /// Get the average time under tension per set [s]
        /// </summary>
        /// <returns>The average time under tension [s]</returns>
        public float GetAverageSecondsUnderTension()

            => (float)TotalSecondsUnderTension / (float)_totalWorkingSets;

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TotalRest;
            yield return TotalSecondsUnderTension;
        }
    }
}