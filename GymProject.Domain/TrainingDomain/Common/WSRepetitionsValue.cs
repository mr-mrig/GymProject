using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class WSRepetitionsValue : ValueObject
    {


        #region Consts

        /// <summary>
        /// The value which means "As Many Reps As Possible"
        /// </summary>
        public const int AMRAPValue = -1;


        /// <summary>
        /// The value which means "Repetitions not set"
        /// </summary>
        public const int NotSetValue = 0;
        #endregion




        /// <summary>
        /// The Repetitions value
        /// </summary>
        public int Value { get; private set; } = NotSetValue;


        /// <summary>
        /// The type of the working set- Repetitions Vs Timed serie
        /// </summary>
        public WSWorkTypeEnum WorkType { get; private set; } = null;




        #region Ctors

        private WSRepetitionsValue() { }


        private WSRepetitionsValue(int serieLength, WSWorkTypeEnum serieType)
        {
            Value = serieLength;
            WorkType = serieType ?? WSWorkTypeEnum.RepetitionBasedSerie;

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - generic
        /// </summary>
        /// <param name="workValue">The value</param>
        /// <returns>The RepetitionValue instance</returns>
        public static WSRepetitionsValue TrackWork(int workValue, WSWorkTypeEnum workType)

            => new WSRepetitionsValue(workValue, workType);


        /// <summary>
        /// Factory method - repetitions based work
        /// </summary>
        /// <param name="repetitions">The value</param>
        /// <returns>The RepetitionValue instance</returns>
        public static WSRepetitionsValue TrackRepetitionSerie(uint repetitions)

            => TrackWork((int)repetitions, WSWorkTypeEnum.RepetitionBasedSerie);


        /// <summary>
        /// Factory method - repetitions not set
        /// </summary>
        /// <param name="workValue">The value</param>
        /// <returns>The RepetitionValue instance</returns>
        public static WSRepetitionsValue TrackNotSetRepetitions()

            => TrackWork(NotSetValue, WSWorkTypeEnum.RepetitionBasedSerie);


        /// <summary>
        /// Factory method for "As Many Reps As Possible"
        /// </summary>
        /// <param name="repetitionsValue">The value</param>
        /// <returns>The RepetitionValue instance</returns>
        public static WSRepetitionsValue TrackAMRAP()

            => TrackWork(AMRAPValue, WSWorkTypeEnum.RepetitionBasedSerie);


        /// <summary>
        /// Factory method - time based work
        /// </summary>
        /// <param name="duration">The value</param>
        /// <returns>The RepetitionValue instance</returns>
        public static WSRepetitionsValue TrackTimedSerie(uint duration)

            => TrackWork((int)duration, WSWorkTypeEnum.TimeBasedSerie);


        /// <summary>
        /// Factory method - repetitions not set
        /// </summary>
        /// <param name="workValue">The value</param>
        /// <returns>The RepetitionValue instance</returns>
        public static WSRepetitionsValue TrackNotSetTime()

            => TrackWork(NotSetValue, WSWorkTypeEnum.TimeBasedSerie);


        #endregion


        #region Business Methods

        /// <summary>
        /// Check if the repetitions are "As Many Reps As Possible"
        /// </summary>
        /// <returns>True if AMRAP</returns>
        public bool IsAMRAP() => Value == AMRAPValue;


        /// <summary>
        /// Check if work value has been specified
        /// </summary>
        /// <returns>True if value has been specified</returns>
        public bool IsValueSpecified() => Value != NotSetValue;


        /// <summary>
        /// Check if the WS work type is timed based: the WSRepetitionValue stores the duration of the set
        /// </summary>
        /// <returns>True if WS is a time based serie</returns>
        public bool IsTimedBasedSerie() => WorkType == WSWorkTypeEnum.TimeBasedSerie;


        /// <summary>
        /// Check if the WS work type is repetition based: the WSRepetitionValue stores the repetitions number of the set
        /// </summary>
        /// <returns>True if WS is a time based serie</returns>
        public bool IsRepetitionBasedSerie() => WorkType == WSWorkTypeEnum.RepetitionBasedSerie;

        #endregion



        #region Business Rules Validation

        /// <summary>
        /// Checks whether the value specified is valid
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidRepetitionsValue() => Value > 0 || Value == NotSetValue || Value == AMRAPValue;


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!ValidRepetitionsValue())
                throw new TrainingDomainInvariantViolationException($"The WS repetitions value must be a positive number.");
        }

        #endregion


        public override string ToString() => Value.ToString() + (WorkType.Equals(WSWorkTypeEnum.TimeBasedSerie) ? WorkType.MeasUnit : "");


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return WorkType;
        }
    }
}