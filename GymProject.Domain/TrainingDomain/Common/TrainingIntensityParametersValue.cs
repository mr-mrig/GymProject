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
        public static TrainingIntensityParametersValue ComputeFromWorkingSets(IEnumerable<IFullWorkingSet> workingSets, TrainingEffortTypeEnum effortType = null)
        {
            TrainingEffortTypeEnum toEffortType = effortType ?? DefaultEffortType;

            List<IFullWorkingSet> wsCopy = workingSets?.Clone().ToList() ?? new List<IFullWorkingSet>();

            if (wsCopy.Count() == 0)
                return InitEmpty(effortType);


            foreach (IFullWorkingSet ws in wsCopy)
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


        /// <summary>
        /// Factory method - Initializes an empty intensity
        /// </summary>
        /// <param name="effortType">The effort type - optional</param>
        /// <returns>The TrainingVolumeValue instance</returns>
        public static TrainingIntensityParametersValue InitEmpty(TrainingEffortTypeEnum effortType = null)

            => new TrainingIntensityParametersValue(0, effortType ?? DefaultEffortType, 0);

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


        ///// <summary>
        ///// Updates the Volume parameters including the working sets added
        ///// </summary>
        ///// <param name="workingSetList">The list of the working sets to be added</param>
        ///// <returns>The new TrainingVolumeValue instance</returns>
        //public TrainingIntensityParametersValue AddWorkingSets(IEnumerable<IWorkingSet> workingSetList)
        //{
        //    float wsIntensitySum = workingSetList.Sum(x => x.Effort.Value);

        //    int wsTotalNumber = workingSetList.Count();

        //    TrainingEffortValue wsTotalWorkload = WeightPlatesValue.MeasureKilograms(workingSetList.Sum(x => x.ToWorkload().Value));

        //    return SetTrainingIntensity(
        //        _intensitySumAccumulator + wsIntensitySum,
        //        TotalWorkingSets + wsTotalNumber,
        //        _totalWorkingSets + wsTotalNumber);
        //}


        [Obsolete("This function works well only if the intial EffortType computed by the Factory method stays unchanged after " +
            "all the WS added. But adding more sets might make the original EffortType wrong")]
        /// <summary>
        /// Updates the Density parameters including the working set added
        /// </summary>
        /// <param name="toAdd">The working set to be added</param>
        /// <param name="effortType">The effort type to convert to</param>
        /// <exception cref="ArgumentNullException">Thrown when input WS is null</exception>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingIntensityParametersValue AddWorkingSet(IFullWorkingSet toAdd)
        {
            if (toAdd == null)
                throw new ArgumentNullException($"Trying to add a NULL working set.");


            if (toAdd.Effort.EffortType != AverageIntensity.EffortType)
                toAdd.ToNewEffortType(AverageIntensity.EffortType);


            return SetTrainingIntensity(
                    toAdd.Effort.Value + _intensitySumAccumulator, AverageIntensity.EffortType, _totalWorkingSets + 1);


            //// Incremental average
            //return SetTrainingIntensity(
            //    TrainingEffortValue.TrackEffort(
            //        (toAdd.Effort.Value - (AverageIntensity?.Value ?? 0)) / (_totalWorkingSets + 1) + AverageIntensity?.Value ?? 0,
            //        AverageIntensity.EffortType),
            //    _totalWorkingSets + 1);
        }



        [Obsolete("This function works well only if the intial EffortType computed by the Factory method stays unchanged after " +
            "all the WS added. But adding more sets might make the original EffortType wrong")]
        /// <summary>
        /// Updates the Density parameters excluding the working set removed
        /// </summary>
        /// <param name="toRemove">The working set to be removed</param>
        /// <exception cref="ArgumentException">Thrown when the WS can't be removed</exception>
        /// <returns>The new TrainingDensityValue instance</returns>
        public TrainingIntensityParametersValue RemoveWorkingSet(IFullWorkingSet toRemove)
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

        ///// <summary>
        ///// Updates the Intensity parameters excluding the working sets added
        ///// </summary>
        ///// <param name="workingSetList">The list of the working sets to be removed</param>
        ///// <returns>The new TrainingVolumeValue instance</returns>
        //public TrainingIntensityParametersValue RemoveWorkingSets(IEnumerable<IWorkingSet> workingSetList)
        //{
        //    TrainingIntensityParametersValue result = this;

        //    foreach (IWorkingSet ws in workingSetList)
        //        result = RemoveWorkingSet(ws);

        //    return result;
        //}

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return AverageIntensity;
        }
    }
}