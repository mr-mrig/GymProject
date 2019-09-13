using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using GymProject.Domain.Utils;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingEffortValue : ValueObject
    {



        #region Static

        /// <summary>
        /// Default effort when nothing has been specified
        /// </summary>
        public static TrainingEffortValue DefaultEffort = AsRPE(8);
        #endregion


        /// <summary>
        /// The effort value: RM > 0, RPE > 0, IntensityPerc [1,100][%]
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The effort type - %, RM, RPE
        /// </summary>
        public TrainingEffortTypeEnum EffortType { get; private set; }




        #region Ctors

        private TrainingEffortValue() { }


        private TrainingEffortValue(float effortValue, TrainingEffortTypeEnum effortType)
        {
            Value = effortValue;
            EffortType = effortType;

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - INTERNAL
        /// </summary>
        /// <param name="effort">The value</param>
        /// <param name="effortType">The effort type</param>
        /// <returns>The TrainingEffortValue instance</returns>
        internal static TrainingEffortValue TrackEffort(float effort, TrainingEffortTypeEnum effortType)

            => new TrainingEffortValue(FormatEffort(effort, effortType), effortType);


        /// <summary>
        /// Factory for creating a effort as a intensity percentage value [%]
        /// It accepts both percentage and ratios, but cannot handle <1% values
        /// </summary>
        /// <param name="effort">The value as either percentage or ratio</param>
        /// <returns>The TrainingEffortValue instance</returns>
        public static TrainingEffortValue AsIntensityPerc(float effort)
        {
            // Convert ratio to Percentage
            if (effort <= TrainingEffortTypeEnum.MaxIntensityPercentage / 100f)
                effort *= 100f;

            return TrackEffort(effort, TrainingEffortTypeEnum.IntensityPercentage);
        }


        /// <summary>
        /// Factory for creating a effort as a RM value
        /// </summary>
        /// <param name="effort">The value</param>
        /// <returns>The TrainingEffortValue instance</returns>
        public static TrainingEffortValue AsRM(float effort)

            => TrackEffort(effort, TrainingEffortTypeEnum.RM);


        /// <summary>
        /// Factory for creating a effort as a RPE value
        /// </summary>
        /// <param name="effort">The value</param>
        /// <returns>The TrainingEffortValue instance</returns>
        public static TrainingEffortValue AsRPE(float effort)

            => TrackEffort(effort, TrainingEffortTypeEnum.RPE);

        #endregion


        #region Business Methods

        /// <summary>
        /// Check whether the effort is expressed as Intensity Percentage
        /// </summary>
        /// <returns>True if Intensity Percentage</returns>
        public bool IsIntensityPercentage() => EffortType == TrainingEffortTypeEnum.IntensityPercentage;


        /// <summary>
        /// Check whether the effort is expressed as RM
        /// </summary>
        /// <returns>True if RM</returns>
        public bool IsRM() => EffortType == TrainingEffortTypeEnum.RM;


        /// <summary>
        /// Check whether the effort is expressed as RPE
        /// </summary>
        /// <returns>True if RPE</returns>
        public bool IsRPE() => EffortType == TrainingEffortTypeEnum.RPE;


        public override string ToString() => Value.ToString() + " " + EffortType.Abbreviation;

        #endregion



        #region Business Rules Validations


        /// <summary>
        /// The Effort Value must satisfy the boundaries according to its type
        /// </summary>
        /// <returns>True if business rule is met</returns>
        //private bool ValidEffortBoundaries() => EffortType.CheckEffortTypeConstraints(Value);        


        /// <summary>
        /// The Effort Value must satisfy the boundaries according to its type
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidEffortBoundaries() => EffortType.CheckEffortTypeConstraints(Value);


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!ValidEffortBoundaries())
                throw new TrainingDomainInvariantViolationException($"The Effort Value ({Value.ToString()}) must satisfy the boundaries according to its type ({EffortType.Abbreviation}).");
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a effort type compliant format
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="effortType">The effort type</param>
        /// <returns>the converted value</returns>
        private static float FormatEffort(float value, TrainingEffortTypeEnum effortType)
        {

            switch (effortType)
            {
                case var _ when effortType == TrainingEffortTypeEnum.IntensityPercentage:

                    return (float)Math.Round(value, 2);

                case var _ when effortType == TrainingEffortTypeEnum.RM:

                    return (float)Math.Round(value, 1);

                case var _ when effortType == TrainingEffortTypeEnum.RPE:

                    //return (float)CommonUtilities.RoundToPointFive(value);
                    return (float)Math.Round(value, 1);

                default:

                    return -1;
            }
        }
        #endregion


        #region Conversion Formulas

        /// <summary>
        /// Convert this effort value to RM, without rounding to the 0-th decimal
        /// To be used when performing intermediate conversions in order to keep the highest precision possible
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <exception cref="ArgumentException">Thrown if repetitions are invalid</exception>
        /// <returns>The new TrainingEffortValue instance</returns>
        private float ToRmExact(WSRepetitionsValue targetReps = null)
        {
            // From Intenisty Percentage
            if (EffortType == TrainingEffortTypeEnum.IntensityPercentage)
            {
                float input = Math.Min(Value, TrainingEffortTypeEnum.OneRMIntensityPercentage);

                return (float)(
                    (324.206809067032 - 18.0137586362208 * input + 0.722425494099458 * Math.Pow(input, 2) - 0.018674659779516 * Math.Pow(input, 3)
                    + 0.00025787003728422 * Math.Pow(input, 4) - 1.65095582844966E-06 * Math.Pow(input, 5) + 2.75225269851 * Math.Pow(10, -9) * Math.Pow(input, 6)
                    + 8.99097867 * Math.Pow(10, -12) * Math.Pow(input, 7)));
            }

            // From RPE
            if (EffortType == TrainingEffortTypeEnum.RPE)
            {
                if (targetReps == null || !targetReps.IsValueSpecified())
                    throw new ArgumentException($"Invalid repetions when converting from RPE.", nameof(targetReps));

                if (targetReps.IsAMRAP())
                    throw new ArgumentException($"Cannot convert from RPE when the reps are AMRAP.", nameof(targetReps));
                else
                    return targetReps.Value + (TrainingEffortTypeEnum.AMRAPAsRPE - Value);
            }
            return Value;
        }


        /// <summary>
        /// Convert this effort value to the RM expression
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <exception cref="ArgumentException">Thrown if repetitions are invalid</exception>
        /// <returns>The new TrainingEffortValue instance</returns>
        public TrainingEffortValue ToRm(WSRepetitionsValue targetReps = null)
        {
            // From Intenisty Percentage
            if (EffortType == TrainingEffortTypeEnum.IntensityPercentage)
            {
                float saturatedInput = Math.Min(Value, TrainingEffortTypeEnum.OneRMIntensityPercentage);   // Truncate to 100%

                double estimatedVal = 
                    (324.206809067032 - 18.0137586362208 * saturatedInput + 0.722425494099458 * Math.Pow(saturatedInput, 2) - 0.018674659779516 * Math.Pow(saturatedInput, 3) 
                    + 0.00025787003728422 * Math.Pow(saturatedInput, 4) - 1.65095582844966E-06 * Math.Pow(saturatedInput, 5) + 2.75225269851 * Math.Pow(10, -9) * Math.Pow(saturatedInput, 6) 
                    + 8.99097867 * Math.Pow(10, -12) * Math.Pow(saturatedInput, 7));

                return AsRM((int)Math.Round(estimatedVal));
            }

            // From RPE
            if (EffortType == TrainingEffortTypeEnum.RPE)
            {
                float saturatedInput;

                if (targetReps.Value == 1)
                    saturatedInput = Math.Min(Value, TrainingEffortTypeEnum.AMRAPAsRPE);   // Cannot do more than 1RM -> Truncate to 10RPE
                else
                    saturatedInput = Value;

                if (targetReps == null || !targetReps.IsValueSpecified())
                    throw new ArgumentException($"Invalid repetions when converting from RPE.", nameof(targetReps));

                if (targetReps.IsAMRAP())
                    throw new ArgumentException($"Cannot convert from RPE when the reps are AMRAP.", nameof(targetReps));
                else
                    return AsRM((int)(targetReps.Value + (TrainingEffortTypeEnum.AMRAPAsRPE - saturatedInput)));    // Truncate
            }

            // From RM
            return this;
        }


        /// <summary>
        /// Convert this effort value to the Intensity Percentage expression
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <returns>The new TrainingEffortValue instance</returns>
        public TrainingEffortValue ToIntensityPercentage(WSRepetitionsValue targetReps = null)
        {
            // From RM
            if (EffortType == TrainingEffortTypeEnum.RM)

                return AsIntensityPerc(Math.Min(
                    (float)Math.Round(0.4167 * Value - 14.2831 * Math.Pow(Value, 0.5) + 115.6122, 1)
                    , TrainingEffortTypeEnum.OneRMIntensityPercentage));

            // From RPE
            if (EffortType == TrainingEffortTypeEnum.RPE)
            {
                float saturatedInput;

                if (targetReps.Value == 1)
                    saturatedInput = Math.Min(Value, TrainingEffortTypeEnum.AMRAPAsRPE);     // Cannot do more than 1RM -> Truncate to 10RPE
                else
                    saturatedInput = Value;

                if (targetReps == null || !targetReps.IsValueSpecified())
                    throw new ArgumentException($"Invalid repetions when converting from RPE.", nameof(targetReps));

                if (targetReps.IsAMRAP())
                    throw new ArgumentException($"Cannot convert from RPE when the reps are AMRAP.", nameof(targetReps));

                int rmValue = (int)(targetReps.Value + (TrainingEffortTypeEnum.AMRAPAsRPE - saturatedInput));

                return AsIntensityPerc(((float)Math.Round(0.4167 * rmValue - 14.2831 * Math.Pow(rmValue, 0.5) + 115.6122, 1)));
            }

            // From Intenisty Percentage -> No conversion
            return this;
        }


        /// <summary>
        /// Convert this effort value to the RPE expression
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <returns>The new TrainingEffortValue instance</returns>
        public TrainingEffortValue ToRPE(WSRepetitionsValue targetReps = null)
        {
            float rmValue;

            // From RPE -> No conversion
            if (EffortType == TrainingEffortTypeEnum.RPE)
                return this;

            if (targetReps == null)
                throw new ArgumentException($"Null repetions object when converting to RPE.", nameof(targetReps));

            if (targetReps.IsAMRAP())
                return AsRPE(TrainingEffortTypeEnum.AMRAPAsRPE);

            if (!targetReps.IsValueSpecified())
                throw new ArgumentException($"Invalid repetions when converting to RPE.", nameof(targetReps));

            // From Intenisty Percentage
            if (EffortType == TrainingEffortTypeEnum.IntensityPercentage)

                rmValue = ToRmExact();     // Convert to RM first

            // From RM
            else
                rmValue = Value;

            // RM to RPE
            return AsRPE(Math.Max(TrainingEffortTypeEnum.AMRAPAsRPE - (rmValue - targetReps.Value), TrainingEffortTypeEnum.MinRPE));
        }

        #endregion


        #region Operators

        //public static TrainingEffortValue operator +(TrainingEffortValue left, TrainingEffortValue right)
        //{
        //    if (left == null)
        //        throw new ArgumentNullException($"No null allowed when summing two TrainingEffortValues", nameof(left));

        //    if (right == null)
        //        throw new ArgumentNullException($"No null allowed when summing two TrainingEffortValues", nameof(right));

        //    if (left.EffortType == right.EffortType)
        //        return TrackEffort(left.Value + right.Value, left.EffortType);

        //    else
        //    {
        //        if (left.IsRPE() || right.IsRPE())
        //            throw new TrainingDomainInvariantViolationException($"Cannot convert to RPE without knowing the repetitions");

        //        if (left.IsIntenistyPercentage())
        //            return TrackEffort(left.Value + right.ToIntensityPercentage().Value, left.EffortType);

        //        // RM
        //        return TrackEffort(left.Value + right.ToRm().Value, left.EffortType);
        //    }
        //}


        //public static TrainingEffortValue operator -(TrainingEffortValue left, TrainingEffortValue right)
        //{
        //    if (left == null)
        //        throw new ArgumentNullException($"No null allowed when subtracting two TrainingEffortValues", nameof(left));

        //    if (right == null)
        //        throw new ArgumentNullException($"No null allowed when subtracting two TrainingEffortValues", nameof(right));

        //    if (left.EffortType == right.EffortType)
        //        return TrackEffort(left.Value - right.Value, left.EffortType);

        //    else
        //    {
        //        if (left.IsRPE() || right.IsRPE())
        //            throw new TrainingDomainInvariantViolationException($"Cannot convert to RPE without knowing the repetitions");

        //        if (left.IsIntenistyPercentage())
        //            return TrackEffort(left.Value - right.ToIntensityPercentage().Value, left.EffortType);

        //        // RM
        //        return TrackEffort(left.Value - right.ToRm().Value, left.EffortType);
        //    }
        //}
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return EffortType;
        }
    }
}