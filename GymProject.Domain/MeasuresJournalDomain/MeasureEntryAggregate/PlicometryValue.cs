using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class PlicometryValue : ValueObject
    {



        /// <summary>
        /// The formula to be applied for the BF computation
        /// </summary>
        public PlicometryFormulaEnum Formula { get; private set; } = null;

        /// <summary>
        /// The gender
        /// </summary>
        public GenderTypeEnum Gender { get; private set; } = null;

        /// <summary>
        /// The age
        /// </summary>
        public ushort Age { get; private set; } = 0;

        /// <summary>
        /// Body Weight
        /// </summary>
        public BodyWeightValue Weight { get; private set; } = null;

        /// <summary>
        /// Body Height
        /// </summary>
        public BodyCircumferenceValue Height { get; private set; } = null;

        /// <summary>
        /// Body Mass Index
        /// </summary>
        public PercentageValue BMI { get; private set; } = null;

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
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Tricep { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Chest { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Armpit { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Subscapular { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Suprailiac { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Abdomen { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue Thigh { get; private set; } = null;






        #region Ctors

        /// <summary>
        /// Requires all the values to be provided as params.
        /// The computation is not done here, hence the formula is not required
        /// </summary>
        private PlicometryValue
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            float? bmi,
            BodyFatValue bf,
            BodyWeightValue fm,
            BodyWeightValue ffm,
            CaliperSkinfoldValue tricep = null,
            CaliperSkinfoldValue chest = null,
            CaliperSkinfoldValue armpit = null,
            CaliperSkinfoldValue subscapular = null,
            CaliperSkinfoldValue suprailiac = null,
            CaliperSkinfoldValue abdomen = null,
            CaliperSkinfoldValue thigh = null)
        {
            // Unknown formula
            Formula = PlicometryFormulaEnum.NotSet;

            Gender = gender;
            Weight = weight;
            Height = height;
            BMI = bmi == null ? null : PercentageValue.MeasureRatio(bmi.Value, 1);
            BF = bf;
            FFM = ffm;
            FM = fm;
            Tricep = tricep;
            Chest = chest;
            Armpit = armpit;
            Subscapular = subscapular;
            Suprailiac = suprailiac;
            Abdomen = abdomen;
            Thigh = thigh;

            if (!HasValidMeasUnits())
                throw new UnsupportedConversionException($"Incompatible meas units provided to {GetType().Name} Ctor");

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");
        }

        /// <summary>
        /// Requires the skinfolds to be provided, then it computes the derived values
        /// </summary>
        private PlicometryValue
        (
            PlicometryFormulaEnum formulaType,
            GenderTypeEnum gender,
            ushort age,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue tricep = null,
            CaliperSkinfoldValue chest = null,
            CaliperSkinfoldValue armpit = null,
            CaliperSkinfoldValue subscapular = null,
            CaliperSkinfoldValue suprailiac = null,
            CaliperSkinfoldValue abdomen = null,
            CaliperSkinfoldValue thigh = null)
        {
            Age = age;
            Gender = gender;
            Weight = weight;
            Height = height;
            Tricep = tricep;
            Chest = chest;
            Armpit = armpit;
            Subscapular = subscapular;
            Suprailiac = suprailiac;
            Abdomen = abdomen;
            Thigh = thigh;

            if (!HasValidMeasUnits())
                throw new UnsupportedConversionException($"Incompatible meas units provided to {GetType().Name} Ctor");

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

            // Compute derivable measures
            BMI = ComputeBodyMassIndex();                                               // Null if invalid parameters
            BF = formulaType.ApplyFormula(Gender, Age, GetValidSkinfoldValues());       // Null if invalid parameters
            FM = ComputeFatMass();                                                      // Null if invalid parameters
            FFM = ComputeFatFreeMass();                                                 // Null if invalid parameters
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory for as-is measures, no automatic computations performed here.
        /// </summary>
        /// <param name="gender">The gender</param>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="bmi">The Body Mass Index</param>
        /// <param name="bf">The BodyFat</param>
        /// <param name="fm">The Fat Mass</param>
        /// <param name="ffm">The Free-Fat Mass</param>
        /// <param name="tricep">The tricep skinfold</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="armpit">The armpit skinfold</param>
        /// <param name="subscapular">The subscapular skinfold</param>
        /// <param name="suprailiac">The suprailiac skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue TrackMeasures
        (
            BodyWeightValue weight = null,
            BodyCircumferenceValue height = null,
            float? bmi = null,
            BodyFatValue bf = null,
            BodyWeightValue fm = null,
            BodyWeightValue ffm = null,
            GenderTypeEnum gender = null,
            CaliperSkinfoldValue tricep = null,
            CaliperSkinfoldValue chest = null,
            CaliperSkinfoldValue armpit = null,
            CaliperSkinfoldValue subscapular = null,
            CaliperSkinfoldValue suprailiac = null,
            CaliperSkinfoldValue abdomen = null,
            CaliperSkinfoldValue thigh = null
        )

            => new PlicometryValue(gender ?? GenderTypeEnum.NotSet, weight, height, bmi, bf, fm, ffm, tricep, chest, armpit, subscapular, suprailiac, abdomen, thigh);




        /// <summary>
        /// Factory for applying the Jackson-Pollock 3-skinfolds formula for Males
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue ComputeJacksonPollockMale3
        (
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue chest,
            CaliperSkinfoldValue abdomen,
            CaliperSkinfoldValue thigh
        )

            => new PlicometryValue(PlicometryFormulaEnum.JacksonPollock3, GenderTypeEnum.Male, weight, height, null, chest, null, null, null, abdomen, thigh);



        /// <summary>
        /// Factory for applying the Jackson-Pollock 3-skinfolds formula for Females
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="tricep">The tricep skinfold</param>
        /// <param name="suprailiac">The suprailiac skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue ComputeJacksonPollockFemale3
        (
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue tricep,
            CaliperSkinfoldValue suprailiac,
            CaliperSkinfoldValue thigh
        )

            => new PlicometryValue(PlicometryFormulaEnum.JacksonPollock3, GenderTypeEnum.Female, weight, height, tricep, null, null, null, suprailiac, null, thigh);




        /// <summary>
        /// Factory for applying the Jackson-Pollock 4-skinfolds formula
        /// </summary>
        /// <param name="formulaType">The formula to be applied</param>
        /// <param name="gender">The gender</param>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="tricep">The tricep skinfold</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="armpit">The armpit skinfold</param>
        /// <param name="subscapular">The subscapular skinfold</param>
        /// <param name="suprailiac">The suprailiac skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue ComputeJacksonPollock4
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue tricep,
            CaliperSkinfoldValue suprailiac,
            CaliperSkinfoldValue abdomen,
            CaliperSkinfoldValue thigh
        )

            => new PlicometryValue(PlicometryFormulaEnum.JacksonPollock4, gender, weight, height, tricep, null, null, null, suprailiac, abdomen, thigh);



        /// <summary>
        /// Factory for applying the Jackson-Pollock 7-skinfolds formula
        /// </summary>
        /// <param name="gender">The gender</param>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="tricep">The tricep skinfold</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="armpit">The armpit skinfold</param>
        /// <param name="subscapular">The subscapular skinfold</param>
        /// <param name="suprailiac">The suprailiac skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue ComputeJacksonPollock7
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue tricep,
            CaliperSkinfoldValue chest,
            CaliperSkinfoldValue armpit,
            CaliperSkinfoldValue subscapular,
            CaliperSkinfoldValue suprailiac,
            CaliperSkinfoldValue abdomen,
            CaliperSkinfoldValue thigh 
        )

            => new PlicometryValue(PlicometryFormulaEnum.JacksonPollock7, gender, weight, height, tricep, chest, armpit, subscapular, suprailiac, abdomen, thigh);

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
        /// Compute the BMI index according to the BIA values
        /// </summary>
        /// <returns>The BMI index [pure number], or null if invalid input properties</returns>
        public PercentageValue ComputeBodyMassIndex()
        {
            // Input not provided
            if (Weight == null && Height == null)
                return null;

            if (!Weight.Unit.MeasureSystemType.Equals(Height.Unit.MeasureSystemType))
                return null;

            return PercentageValue.MeasureRatio(Height.Value / Weight.Value);
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
            MeasurmentSystemEnum rhs = Tricep.Unit.MeasureSystemType;

            return GetSkinfoldValues().Where(x => !x.Unit.MeasureSystemType.Equals(rhs)).Count() == 0;
        }
        #endregion


        /// <summary>
        /// Checks if Meas Units are invalid
        /// </summary>
        /// <param name="left">LHS</param>
        /// <param name="right">RHS</param>
        /// <returns>Returns null if valid</returns>
        private CaliperSkinfoldValue CheckInvalidMeasUnit(CaliperSkinfoldValue left, CaliperSkinfoldValue right) => left.Unit.MeasureSystemType.Equals(right.Unit.MeasureSystemType) ? null : left;

        /// <summary>
        /// Get the Skinfold Values list
        /// </summary>
        /// <returns>The Skinfold Values list</returns>
        private IEnumerable<CaliperSkinfoldValue> GetSkinfoldValues() 
            => GetAtomicValues().Where(x => x?.GetType() == typeof(CaliperSkinfoldValue)).Select(x => (CaliperSkinfoldValue)x);

        /// <summary>
        /// Get the Skinfold values which represent a valid measure
        /// </summary>
        /// <returns>The Skinfold Values list</returns>
        private IEnumerable<CaliperSkinfoldValue> GetValidSkinfoldValues() 
            => GetAtomicValues().Where(x => x != null && x?.GetType() == typeof(CaliperSkinfoldValue)).Select(x => (CaliperSkinfoldValue)x).Where(x => x.Value > 0);




        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Formula;
            yield return Gender;
            yield return Age;
            yield return Weight;
            yield return Height;
            yield return BMI;
            yield return BF;
            yield return FFM;
            yield return FM;
            yield return Tricep;
            yield return Chest;
            yield return Armpit;
            yield return Subscapular;
            yield return Suprailiac;
            yield return Abdomen;
            yield return Thigh;
        }


    }
}
