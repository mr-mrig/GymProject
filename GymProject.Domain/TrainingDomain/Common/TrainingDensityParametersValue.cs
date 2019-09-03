﻿using GymProject.Domain.Base;
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
        public static TrainingDensityParametersValue ComputeFromWorkingSets(IEnumerable<IFullWorkingSet> workingSets)
        {
            List<IFullWorkingSet> wsCopy = workingSets?.Clone().ToList() ?? new List<IFullWorkingSet>();

            if (wsCopy.Count() == 0)
                return InitEmpty();


            // Exception or exclude from computation?
            if (wsCopy.Any(x => x == null))
                throw new ArgumentNullException($"Trying to compute the Training Density on null-containing WS list");


            return SetTrainingDensity(
                wsCopy.Sum(x => x.ToSecondsUnderTension()),
                wsCopy.Sum(x => x.ToTotalSeconds() - x.ToSecondsUnderTension()),
                wsCopy.Count());
        }


        /// <summary>
        /// Factory method - Initializes an empty density
        /// </summary>
        /// <returns>The TrainingVolumeValue instance</returns>
        public static TrainingDensityParametersValue InitEmpty()

            => new TrainingDensityParametersValue(0, 0, 0);

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
        public TrainingDensityParametersValue AddWorkingSet(IFullWorkingSet toAdd)

            => SetTrainingDensity(
                TotalSecondsUnderTension + toAdd.ToSecondsUnderTension(),
                TotalRest + toAdd.ToRest(),
                ++_totalWorkingSets);


        /// <summary>
        /// Updates the Volume parameters including the working sets added
        /// </summary>
        /// <param name="workingSetList">The list of the working sets to be added</param>
        /// <returns>The new TrainingVolumeValue instance</returns>
        public TrainingDensityParametersValue AddWorkingSets(IEnumerable<IFullWorkingSet> workingSetList)
        {
            int wsTotalTut = workingSetList.Sum(x => x.ToSecondsUnderTension());
            int wsTotalRest = workingSetList.Sum(x => x.ToRest());
            int wsTotalNumber = workingSetList.Count();

            return SetTrainingDensity(
                TotalSecondsUnderTension + wsTotalTut,
                TotalRest + wsTotalRest,
                _totalWorkingSets + wsTotalNumber);
        }


        /// <summary>
        /// Updates the Density parameters excluding the working set removed
        /// </summary>
        /// <param name="toRemove">The working set to be removed</param>
        /// <exception cref="ArgumentException">Thrown when the WS can't be removed</exception>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingDensityParametersValue RemoveWorkingSet(IFullWorkingSet toRemove)
        {
            if (_totalWorkingSets == 0)
                throw new ArgumentException($"Trying to remove a WorkingSet when no one has been added.");

            return SetTrainingDensity(
                TotalSecondsUnderTension - toRemove.ToSecondsUnderTension(),
                TotalRest - (toRemove.ToTotalSeconds() - toRemove.ToSecondsUnderTension()),
                --_totalWorkingSets);
        }


        /// <summary>
        /// Updates the Desnity parameters excluding the working sets added
        /// </summary>
        /// <param name="workingSetList">The list of the working sets to be removed</param>
        /// <returns>The new TrainingVolumeValue instance</returns>
        public TrainingDensityParametersValue RemoveWorkingSets(IEnumerable<IFullWorkingSet> workingSetList)
        {
            int wsTotalTut = workingSetList.Sum(x => x.ToSecondsUnderTension());
            int wsTotalRest = workingSetList.Sum(x => x.ToRest());
            int wsTotalNumber = workingSetList.Count();

            return SetTrainingDensity(
                TotalSecondsUnderTension - wsTotalTut,
                TotalRest - wsTotalRest,
                _totalWorkingSets - wsTotalNumber);
        }


        /// <summary>
        /// Get the average rest between sets
        /// </summary>
        /// <returns>The average rest over the WSs</returns>
        public float GetAverageRest() => _totalWorkingSets == 0 ? 0 : (float)TotalRest / (float)_totalWorkingSets;


        /// <summary>
        /// Get the average time under tension per set [s]
        /// </summary>
        /// <returns>The average time under tension [s]</returns>
        public float GetAverageSecondsUnderTension()

            => _totalWorkingSets == 0 ? 0 :(float)TotalSecondsUnderTension / (float)_totalWorkingSets;

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TotalRest;
            yield return TotalSecondsUnderTension;
        }
    }
}