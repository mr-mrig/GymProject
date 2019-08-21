using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using GymProject.Domain.Utils.Extensions;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingIntensityParametersValue : ValueObject
    {



        #region Consts

        public static readonly TrainingEffortTypeEnum DefaultEffortType = TrainingEffortTypeEnum.IntensityPerc;
        #endregion


        private int _totalWorkingSets = 0;

        private float _intensitySumAccumulator = 0f;



        /// <summary>
        /// Average Intensity
        /// </summary>
        public TrainingEffortValue AverageIntensity { get; private set; } = null;





        #region Ctors

        private TrainingIntensityParametersValue(float intensitySum, TrainingEffortTypeEnum effortType, int workingSetsNumber)
        {
            _totalWorkingSets = workingSetsNumber;
            _intensitySumAccumulator = intensitySum;

            AverageIntensity = _totalWorkingSets == 0 ? null : TrainingEffortValue.TrackEffort((float)_intensitySumAccumulator / (float)_totalWorkingSets, effortType);

            //TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="intensitySum">The sum of the intensities</param>
        /// <param name="effortType">The target effort type for the intensity</param>
        /// <param name="workingSetsNumber">The number of working sets</param>
        /// <returns>The TrainingDensityValue instance</returns>
        protected static TrainingIntensityParametersValue SetTrainingIntensity(float intensitySum, TrainingEffortTypeEnum effortType, int workingSetsNumber)

            => new TrainingIntensityParametersValue(intensitySum, effortType, workingSetsNumber);


        /// <summary>
        /// Facotry method - Computes the intenisty parameters from a list of WSs
        /// </summary>
        /// <param name="workingSets">The working sets</param>
        /// <param name="effortType">The effort type - DefaultEffortType if left null</param>
        /// <exception cref="ArgumentNullException">Thrown if input contains NULL elements</exception>
        /// <returns>The TrainingDensityValue instance</returns>
        public static TrainingIntensityParametersValue ComputeFromWorkingSets(IEnumerable<IWorkingSet> workingSets, TrainingEffortTypeEnum effortType = null)
        {
            TrainingEffortTypeEnum toEffortType = effortType ?? DefaultEffortType;

            List<IWorkingSet> wsCopy = workingSets?.Clone().ToList() ?? new List<IWorkingSet>();

            if (wsCopy.Count() == 0)
                return new TrainingIntensityParametersValue(0, toEffortType, 0);


            foreach (IWorkingSet ws in wsCopy)
            {
                // Exception or exclude from computation?
                if (ws == null)
                    throw new ArgumentNullException($"Trying to compute the Training Intensity on null-containing WS list");

                // Ensure the effort types are as expected
                ws.ToNewEffortType(toEffortType);
            }

            return SetTrainingIntensity(
                    wsCopy.Sum(x => x?.Effort?.Value ?? TrainingEffortValue.DefaultEffort.Value), toEffortType, wsCopy.Count());
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


        #region Public Methods

        /// <summary>
        /// Updates the Density parameters including the working set added
        /// </summary>
        /// <param name="toAdd">The working set to be added</param>
        /// <exception cref="ArgumentNullException">Thrown when input WS is null</exception>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingIntensityParametersValue AddWorkingSet(IWorkingSet toAdd)
        {
            if (toAdd == null)
                throw new ArgumentNullException($"Trying to add a NULL working set.");

            if (toAdd.Effort.EffortType != AverageIntensity.EffortType)
                toAdd.ToNewEffortType(AverageIntensity.EffortType);


            return SetTrainingIntensity(
                    toAdd.Effort.Value + _intensitySumAccumulator, AverageIntensity.EffortType,_totalWorkingSets + 1);


            //// Incremental average
            //return SetTrainingIntensity(
            //    TrainingEffortValue.TrackEffort(
            //        (toAdd.Effort.Value - (AverageIntensity?.Value ?? 0)) / (_totalWorkingSets + 1) + AverageIntensity?.Value ?? 0,
            //        AverageIntensity.EffortType),
            //    _totalWorkingSets + 1);
        }


        /// <summary>
        /// Updates the Density parameters excluding the working set removed
        /// </summary>
        /// <param name="toRemove">The working set to be removed</param>
        /// <exception cref="ArgumentException">Thrown when the WS can't be removed</exception>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingIntensityParametersValue RemoveWorkingSet(IWorkingSet toRemove)
        {
            if (_totalWorkingSets == 0)
                throw new ArgumentException($"Trying to remove a WorkingSet when no one has been added.");

            if (_totalWorkingSets == 1)
                return SetTrainingIntensity(0, AverageIntensity.EffortType, 0);


            return SetTrainingIntensity(
                _intensitySumAccumulator - toRemove.Effort.Value, AverageIntensity.EffortType, _totalWorkingSets - 1);


            //return SetTrainingIntensity(
            //TrainingEffortValue.TrackEffort(
            //    ((AverageIntensity?.Value ?? 0) * _totalWorkingSets - toRemove.Effort.Value) / (_totalWorkingSets - 1),
            //    AverageIntensity.EffortType),
            //_totalWorkingSets + 1);
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return AverageIntensity;
        }
    }
}