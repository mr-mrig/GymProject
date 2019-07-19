using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class CircumferenceMeasureValue : ValueObject
    {




        #region Properties

        /// <summary>
        /// The formula used
        /// </summary>
        public CircumferenceFormulaEnum Formula { get; private set; } = null;

        /// <summary>
        /// Weight
        /// </summary>
        public BodyWeightValue Weight { get; private set; } = null;

        /// <summary>
        /// Body Height
        /// </summary>
        public BodyMeasureValue Height { get; private set; } = null;


        /// <summary>
        /// Body Height
        /// </summary>
        public GenderTypeEnum Gender { get; private set; } = null;

        /// <summary>
        /// Body Fat
        /// </summary>
        public BodyFatValue BF { get; private set; } = null;

        /// <summary>
        /// Free-Fat Mass
        /// </summary>
        public BodyWeightValue FFM { get; private set; } = null;

        /// <summary>
        /// Fat Mass
        /// </summary>
        public BodyWeightValue FM { get; private set; } = null;

        /// <summary>
        /// Left Forearm Circumference
        /// </summary>
        public BodyMeasureValue LeftForearm { get; private set; } = null;

        /// <summary>
        /// Right Forearm Circumference
        /// </summary>
        public BodyMeasureValue RightForearm { get; private set; } = null;

        /// <summary>
        /// Left Arm Circumference
        /// </summary>
        public BodyMeasureValue LeftArm { get; private set; } = null;

        /// <summary>
        /// Right Arm Circumference
        /// </summary>
        public BodyMeasureValue RightArm { get; private set; } = null;

        /// <summary>
        /// Neck Circumference
        /// </summary>
        public BodyMeasureValue Neck { get; private set; } = null;

        /// <summary>
        /// Shoulders Circumference
        /// </summary>
        public BodyMeasureValue Shoulders { get; private set; } = null;

        /// <summary>
        /// Chest Circumference
        /// </summary>
        public BodyMeasureValue Chest { get; private set; } = null;

        /// <summary>
        /// Abdomen Circumference
        /// </summary>
        public BodyMeasureValue Abdomen { get; private set; } = null;

        /// <summary>
        /// Waist Circumference
        /// </summary>
        public BodyMeasureValue Waist { get; private set; } = null;

        /// <summary>
        /// Hips Circumference
        /// </summary>
        public BodyMeasureValue Hips { get; private set; } = null;

        /// <summary>
        /// Left Leg Circumference
        /// </summary>
        public BodyMeasureValue LeftLeg { get; private set; } = null;

        /// <summary>
        /// Right Forearm Circumference
        /// </summary>
        public BodyMeasureValue RightLeg { get; private set; } = null;

        /// <summary>
        /// Left Calf Circumference
        /// </summary>
        public BodyMeasureValue LeftCalf { get; private set; } = null;

        /// <summary>
        /// Right Calf Circumference
        /// </summary>
        public BodyMeasureValue RightCalf { get; private set; } = null;
        #endregion





        #region Ctors

        /// <summary>
        /// Requires all the values to be provided as params.
        /// The computation is not done here, hence the formula is not required
        /// </summary>
        private CircumferenceMeasureValue
        (
            BodyMeasureValue height = null,
            BodyWeightValue weight = null,
            GenderTypeEnum gender = null,
            BodyFatValue bf = null,
            BodyWeightValue ffm = null,
            BodyWeightValue fm = null,
            BodyMeasureValue leftForearm = null,
            BodyMeasureValue rightForearm = null,
            BodyMeasureValue leftArm = null,
            BodyMeasureValue rightArm = null,
            BodyMeasureValue neck = null,
            BodyMeasureValue shoulders = null,
            BodyMeasureValue chest = null,
            BodyMeasureValue abdomen = null,
            BodyMeasureValue waist = null,
            BodyMeasureValue hips = null,
            BodyMeasureValue leftLeg = null,
            BodyMeasureValue rightLeg = null,
            BodyMeasureValue leftCalf = null,
            BodyMeasureValue rightCalf = null
        )
        {
            Formula = CircumferenceFormulaEnum.NotSet;

            Weight = weight;
            Height = height;
            Gender = gender;
            BF = bf;
            FFM = ffm;
            FM = fm;
            LeftForearm = leftForearm;
            RightForearm = rightForearm;
            LeftArm = leftArm;
            RightArm = rightArm;
            Neck = neck;
            Shoulders = shoulders;
            Chest = chest;
            Abdomen = abdomen;
            Waist = waist;
            Hips = hips;
            LeftLeg = leftLeg;
            RightLeg = rightLeg;
            LeftCalf =  leftCalf;
            RightCalf = rightCalf;


            if (CheckNullMeasures())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

            if (!HasValidMeasUnits())
                throw new UnsupportedMeasureException($"Incompatible meas units provided to {GetType().Name} Ctor");
        }

        /// <summary>
        /// Requires the skinfolds to be provided; then it computes the derived values
        /// </summary>
        private CircumferenceMeasureValue
        (
            CircumferenceFormulaEnum formulaType,
            GenderTypeEnum gender = null,
            BodyWeightValue weight = null,
            BodyMeasureValue height = null,
            BodyMeasureValue leftForearm = null,
            BodyMeasureValue rightForearm = null,
            BodyMeasureValue leftArm = null,
            BodyMeasureValue rightArm = null,
            BodyMeasureValue neck = null,
            BodyMeasureValue shoulders = null,
            BodyMeasureValue chest = null,
            BodyMeasureValue abdomen = null,
            BodyMeasureValue waist = null,
            BodyMeasureValue hips = null,
            BodyMeasureValue leftLeg = null,
            BodyMeasureValue rightLeg = null,
            BodyMeasureValue leftCalf = null,
            BodyMeasureValue rightCalf = null
        )
        {
            Formula = formulaType;

            Weight = weight;
            Height = height;
            Gender = gender;
            LeftForearm = leftForearm;
            RightForearm = rightForearm;
            LeftArm = leftArm;
            RightArm = rightArm;
            Neck = neck;
            Shoulders = shoulders;
            Chest = chest;
            Abdomen = abdomen;
            Waist = waist;
            Hips = hips;
            LeftLeg = leftLeg;
            RightLeg = rightLeg;
            LeftCalf = leftCalf;
            RightCalf = rightCalf;


            if (CheckNullMeasures())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

            if (!HasValidMeasUnits())
                throw new UnsupportedMeasureException($"Incompatible meas units provided to {GetType().Name} Ctor");

            // Compute derivable measures
            BF = formulaType.ApplyFormula(Gender, HodgonAndBeckettMeasures());          // Null if invalid parameters
            FM = BodyMeasuresCalculator.ComputeFatMass(Weight, BF);                     // Null if invalid parameters
            FFM = BodyMeasuresCalculator.ComputeFatFreeMass(Weight, FM);                // Null if invalid parameters
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory for as-is measures, no automatic computations performed here.
        /// </summary>
        /// <returns>A new CircumferenceMeasureValue instance</returns>
        public static CircumferenceMeasureValue TrackMeasures
        (
               BodyMeasureValue height = null,
               BodyWeightValue weight = null,
               GenderTypeEnum gender = null,
               BodyFatValue bf = null,
               BodyWeightValue ffm = null,
               BodyWeightValue fm = null,
               BodyMeasureValue leftForearm = null,
               BodyMeasureValue rightForearm = null,
               BodyMeasureValue leftArm = null,
               BodyMeasureValue rightArm = null,
               BodyMeasureValue neck = null,
               BodyMeasureValue shoulders = null,
               BodyMeasureValue chest = null,
               BodyMeasureValue abdomen = null,
               BodyMeasureValue waist = null,
               BodyMeasureValue hips = null,
               BodyMeasureValue leftLeg = null,
               BodyMeasureValue rightLeg = null,
               BodyMeasureValue leftCalf = null,
               BodyMeasureValue rightCalf = null
        )

            => new CircumferenceMeasureValue(height, weight, gender, bf, ffm, fm, leftForearm, rightForearm, leftArm, rightArm, neck, shoulders, chest, abdomen, waist, hips, leftLeg, rightLeg, leftCalf, rightCalf);




        /// <summary>
        /// Factory for tracking measures and applying the Hodgdon and Beckett formula for the derived values
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <returns>A new CircumferenceMeasureValue instance</returns>
        public static CircumferenceMeasureValue ComputeHodgonAndBeckett
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyMeasureValue height,
            BodyMeasureValue leftForearm = null,
            BodyMeasureValue rightForearm = null,
            BodyMeasureValue leftArm = null,
            BodyMeasureValue rightArm = null,
            BodyMeasureValue neck = null,
            BodyMeasureValue shoulders = null,
            BodyMeasureValue chest = null,
            BodyMeasureValue abdomen = null,
            BodyMeasureValue waist = null,
            BodyMeasureValue hips = null,
            BodyMeasureValue leftLeg = null,
            BodyMeasureValue rightLeg = null,
            BodyMeasureValue leftCalf = null,
            BodyMeasureValue rightCalf = null
        )

            => new CircumferenceMeasureValue(CircumferenceFormulaEnum.HodgonAndBeckett, gender, weight, height, leftForearm, rightForearm, leftArm, rightArm, neck, shoulders, chest, abdomen, waist, hips, leftLeg, rightLeg, leftCalf, rightCalf);

        #endregion



        #region Business Methods

        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
            => GetAtomicValues().All(x => x is null);


        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullMeasures()
            => GetAtomicValues().Where(x => x != null && x?.GetType() == typeof(BodyMeasureValue)).Count() == 0;

        /// <summary>
        /// Checks whether all the properties have coherent measure units
        /// </summary>
        /// <returns>True if ok, false if ko</returns>
        public bool HasValidMeasUnits()
        {

            // Check if weight and height have non-coherent meas units
            if (Height?.Unit.MeasureSystemType.Equals(Weight?.Unit.MeasureSystemType) != null
                && !(Height?.Unit.MeasureSystemType.Equals(Weight?.Unit.MeasureSystemType)).Value)
                return false;

            // Check that all the skinfolds have coherent meas system, by comparing one of them with all the others
            MeasurmentSystemEnum rhs = GetValidMeasures().FirstOrDefault()?.Unit.MeasureSystemType;

            return GetMeasures().Where(x => !x.Unit.MeasureSystemType.Equals(rhs)).Count() == 0;
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Checks if Meas Units are invalid
        /// </summary>
        /// <param name="left">LHS</param>
        /// <param name="right">RHS</param>
        /// <returns>Returns null if valid</returns>
        private BodyMeasureValue CheckInvalidMeasUnit(BodyMeasureValue left, BodyMeasureValue right) => left.Unit.MeasureSystemType.Equals(right.Unit.MeasureSystemType) ? null : left;

        /// <summary>
        /// Get the measures Values list
        /// </summary>
        /// <returns>The Skinfold Values list</returns>
        private IEnumerable<BodyMeasureValue> GetMeasures()
            => GetAtomicValues().Where(x => x?.GetType() == typeof(BodyMeasureValue)).Select(x => (BodyMeasureValue)x);

        /// <summary>
        /// Get the measures values which represent a valid measure
        /// </summary>
        /// <returns>The Skinfold Values list</returns>
        private IEnumerable<BodyMeasureValue> GetValidMeasures()
            => GetAtomicValues().Where(x => x != null && x?.GetType() == typeof(BodyMeasureValue)).Select(x => (BodyMeasureValue)x).Where(x => x.Value > 0);


        /// <summary>
        /// Get the measures needed by the Hodgon and Beckett formula, ordered as they must be used
        /// </summary>
        /// <returns>The measures to be provided to the formula</returns>
        private IList<BodyMeasureValue> HodgonAndBeckettMeasures()
        {
            if (Gender.IsMale())
            {
                return new List<BodyMeasureValue>()
                {
                    Abdomen,
                    Neck,
                    Height,
                };
            }
            else if (Gender.IsFemale())
            {
                return new List<BodyMeasureValue>()
                {
                    Waist,
                    Hips,
                    Neck,
                    Height,
                };
            }
            else
                return null;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Formula;
            yield return Gender;
            yield return Weight;
            yield return Height;
            yield return LeftForearm;
            yield return RightForearm;
            yield return LeftArm;
            yield return RightArm;
            yield return Neck;
            yield return Shoulders;
            yield return Chest;
            yield return Abdomen;
            yield return Waist;
            yield return Hips;
            yield return LeftLeg;
            yield return RightLeg;
            yield return LeftCalf;
            yield return RightCalf;
        }
        #endregion

    }
}
