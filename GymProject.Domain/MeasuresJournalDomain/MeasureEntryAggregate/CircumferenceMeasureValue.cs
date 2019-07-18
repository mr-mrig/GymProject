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
        public BodyCircumferenceValue Height { get; private set; } = null;


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
        public BodyCircumferenceValue LeftForearm { get; private set; } = null;

        /// <summary>
        /// Right Forearm Circumference
        /// </summary>
        public BodyCircumferenceValue RightForearm { get; private set; } = null;

        /// <summary>
        /// Left Arm Circumference
        /// </summary>
        public BodyCircumferenceValue LeftArm { get; private set; } = null;

        /// <summary>
        /// Right Arm Circumference
        /// </summary>
        public BodyCircumferenceValue RightArm { get; private set; } = null;

        /// <summary>
        /// Neck Circumference
        /// </summary>
        public BodyCircumferenceValue Neck { get; private set; } = null;

        /// <summary>
        /// Shoulders Circumference
        /// </summary>
        public BodyCircumferenceValue Shoulders { get; private set; } = null;

        /// <summary>
        /// Chest Circumference
        /// </summary>
        public BodyCircumferenceValue Chest { get; private set; } = null;

        /// <summary>
        /// Abdomen Circumference
        /// </summary>
        public BodyCircumferenceValue Abdomen { get; private set; } = null;

        /// <summary>
        /// Waist Circumference
        /// </summary>
        public BodyCircumferenceValue Waist { get; private set; } = null;

        /// <summary>
        /// Hips Circumference
        /// </summary>
        public BodyCircumferenceValue Hips { get; private set; } = null;

        /// <summary>
        /// Left Leg Circumference
        /// </summary>
        public BodyCircumferenceValue LeftLeg { get; private set; } = null;

        /// <summary>
        /// Right Forearm Circumference
        /// </summary>
        public BodyCircumferenceValue RightLeg { get; private set; } = null;

        /// <summary>
        /// Left Calf Circumference
        /// </summary>
        public BodyCircumferenceValue LeftCalf { get; private set; } = null;

        /// <summary>
        /// Right Calf Circumference
        /// </summary>
        public BodyCircumferenceValue RightCalf { get; private set; } = null;
        #endregion





        #region Ctors

        /// <summary>
        /// Requires all the values to be provided as params.
        /// The computation is not done here, hence the formula is not required
        /// </summary>
        private CircumferenceMeasureValue
        (
            BodyCircumferenceValue height = null,
            BodyWeightValue weight = null,
            GenderTypeEnum gender = null,
            BodyFatValue bf = null,
            BodyWeightValue ffm = null,
            BodyWeightValue fm = null,
            BodyCircumferenceValue leftForearm = null,
            BodyCircumferenceValue rightForearm = null,
            BodyCircumferenceValue leftArm = null,
            BodyCircumferenceValue rightArm = null,
            BodyCircumferenceValue neck = null,
            BodyCircumferenceValue shoulders = null,
            BodyCircumferenceValue chest = null,
            BodyCircumferenceValue abdomen = null,
            BodyCircumferenceValue waist = null,
            BodyCircumferenceValue hips = null,
            BodyCircumferenceValue leftLeg = null,
            BodyCircumferenceValue rightLeg = null,
            BodyCircumferenceValue leftCalf = null,
            BodyCircumferenceValue rightCalf = null
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

            if (!HasValidMeasUnits())
                throw new UnsupportedConversionException($"Incompatible meas units provided to {GetType().Name} Ctor");

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");
        }

        /// <summary>
        /// Requires the skinfolds to be provided; then it computes the derived values
        /// </summary>
        private CircumferenceMeasureValue
        (
            CircumferenceFormulaEnum formulaType,
            GenderTypeEnum gender = null,
            BodyWeightValue weight = null,
            BodyCircumferenceValue height = null,
            BodyCircumferenceValue leftForearm = null,
            BodyCircumferenceValue rightForearm = null,
            BodyCircumferenceValue leftArm = null,
            BodyCircumferenceValue rightArm = null,
            BodyCircumferenceValue neck = null,
            BodyCircumferenceValue shoulders = null,
            BodyCircumferenceValue chest = null,
            BodyCircumferenceValue abdomen = null,
            BodyCircumferenceValue waist = null,
            BodyCircumferenceValue hips = null,
            BodyCircumferenceValue leftLeg = null,
            BodyCircumferenceValue rightLeg = null,
            BodyCircumferenceValue leftCalf = null,
            BodyCircumferenceValue rightCalf = null
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

            if (!HasValidMeasUnits())
                throw new UnsupportedConversionException($"Incompatible meas units provided to {GetType().Name} Ctor");

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

            // Compute derivable measures
            BF = formulaType.ApplyFormula(Gender, HodgonAndBeckettMeasures());            // Null if invalid parameters
            FM = ComputeFatMass();                                                         // Null if invalid parameters
            FFM = ComputeFatFreeMass();                                                    // Null if invalid parameters
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory for as-is measures, no automatic computations performed here.
        /// </summary>
        /// <returns>A new CircumferenceMeasureValue instance</returns>
        public static CircumferenceMeasureValue TrackMeasures
        (
               BodyCircumferenceValue height = null,
               BodyWeightValue weight = null,
               GenderTypeEnum gender = null,
               BodyFatValue bf = null,
               BodyWeightValue ffm = null,
               BodyWeightValue fm = null,
               BodyCircumferenceValue leftForearm = null,
               BodyCircumferenceValue rightForearm = null,
               BodyCircumferenceValue leftArm = null,
               BodyCircumferenceValue rightArm = null,
               BodyCircumferenceValue neck = null,
               BodyCircumferenceValue shoulders = null,
               BodyCircumferenceValue chest = null,
               BodyCircumferenceValue abdomen = null,
               BodyCircumferenceValue waist = null,
               BodyCircumferenceValue hips = null,
               BodyCircumferenceValue leftLeg = null,
               BodyCircumferenceValue rightLeg = null,
               BodyCircumferenceValue leftCalf = null,
               BodyCircumferenceValue rightCalf = null
        )

            => new CircumferenceMeasureValue(height, weight, gender, bf, ffm, fm, leftForearm, rightForearm, leftArm, rightArm, neck, shoulders, chest, abdomen, waist, hips, leftLeg, rightLeg, leftCalf, rightCalf);




        /// <summary>
        /// Factory for tracking measures and applying the Hodgdon and Beckett formula for the derived values
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new CircumferenceMeasureValue instance</returns>
        public static CircumferenceMeasureValue ComputeHodgonAndBeckett
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            BodyCircumferenceValue leftForearm = null,
            BodyCircumferenceValue rightForearm = null,
            BodyCircumferenceValue leftArm = null,
            BodyCircumferenceValue rightArm = null,
            BodyCircumferenceValue neck = null,
            BodyCircumferenceValue shoulders = null,
            BodyCircumferenceValue chest = null,
            BodyCircumferenceValue abdomen = null,
            BodyCircumferenceValue waist = null,
            BodyCircumferenceValue hips = null,
            BodyCircumferenceValue leftLeg = null,
            BodyCircumferenceValue rightLeg = null,
            BodyCircumferenceValue leftCalf = null,
            BodyCircumferenceValue rightCalf = null
        )

            => new CircumferenceMeasureValue(CircumferenceFormulaEnum.HodgonAndBeckett, gender, weight, height, leftForearm, rightForearm, leftArm, rightArm, neck, shoulders, chest, abdomen, waist, hips, leftLeg, rightLeg, leftCalf, rightCalf);

        #endregion



        #region Business Methods

        /// <summary>
        /// Compute the Fat Mass weight according to the BIA values
        /// </summary>
        /// <returns>The FM weight according to the Weight measure unit, or null if the parameters for the computation has not been provided</returns>
        public BodyWeightValue ComputeFatMass()
        {
            if (Weight == null && BF == null)
                return null;

            return BodyWeightValue.Measure(Weight.Value * BF.Value, Weight.Unit);
        }


        /// <summary>
        /// Compute the Fat-Free Mass weight according to the BIA values
        /// </summary>
        /// <returns>The FFM weight according to the Weight and Height measure units, or null if the parameters for the computation has not been provided</returns>
        public BodyWeightValue ComputeFatFreeMass()
        {
            if (Weight == null && BF == null)
                return null;

            return BodyWeightValue.Measure(Weight.Value * (1 - BF.Value), Weight.Unit);
        }


        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
            => GetAtomicValues().All(x => x is null);


        /// <summary>
        /// Checks whether all the properties have coherent measure units
        /// </summary>
        /// <returns>True if ok, false if ko</returns>
        public bool HasValidMeasUnits()
        {

            // Check if weight and height have non-coherent meas units
            if (!Height.Unit.MeasureSystemType.Equals(Weight.Unit.MeasureSystemType))
                return false;

            // Check that all the skinfolds have coherent meas system, by comparing one of them with all the others
            MeasurmentSystemEnum rhs = Neck.Unit.MeasureSystemType;

            return GetMeasures().Where(x => !x.Unit.MeasureSystemType.Equals(rhs)).Count() == 0;
        }
        #endregion



        /// <summary>
        /// Checks if Meas Units are invalid
        /// </summary>
        /// <param name="left">LHS</param>
        /// <param name="right">RHS</param>
        /// <returns>Returns null if valid</returns>
        private BodyCircumferenceValue CheckInvalidMeasUnit(BodyCircumferenceValue left, BodyCircumferenceValue right) => left.Unit.MeasureSystemType.Equals(right.Unit.MeasureSystemType) ? null : left;

        /// <summary>
        /// Get the measures Values list
        /// </summary>
        /// <returns>The Skinfold Values list</returns>
        private IEnumerable<BodyCircumferenceValue> GetMeasures()
            => GetAtomicValues().Where(x => x?.GetType() == typeof(BodyCircumferenceValue)).Select(x => (BodyCircumferenceValue)x);

        /// <summary>
        /// Get the measures values which represent a valid measure
        /// </summary>
        /// <returns>The Skinfold Values list</returns>
        private IEnumerable<BodyCircumferenceValue> GetValidMeasures()
            => GetAtomicValues().Where(x => x != null && x?.GetType() == typeof(BodyCircumferenceValue)).Select(x => (BodyCircumferenceValue)x).Where(x => x.Value > 0);


        /// <summary>
        /// Get the measures needed by the Hodgon and Beckett formula, ordered as they must be used
        /// </summary>
        /// <returns>The measures to be provided to the formula</returns>
        private IList<BodyCircumferenceValue> HodgonAndBeckettMeasures()
        {
            if (Gender.IsMale())
            {
                return new List<BodyCircumferenceValue>()
                {
                    Abdomen,
                    Neck,
                    Height,
                };
            }
            else if (Gender.IsFemale())
            {
                return new List<BodyCircumferenceValue>()
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


    }
}
