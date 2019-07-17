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
        public PercentageValue BF { get; private set; } = null;

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
        public CaliperSkinfoldValue LeftForearm { get; private set; } = null;

        /// <summary>
        /// Caliper Skinfold Measure
        /// </summary>
        public CaliperSkinfoldValue RightForearm { get; private set; } = null;

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

        private PlicometryValue
        (
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            PercentageValue bmi,
            PercentageValue bf,
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
            Weight = weight;
            Height = height;
            BMI = bmi;
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

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");
        }

        private PlicometryValue
        (
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            float bmi,
            float bf,
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
            Weight = weight;
            Height = height;
            BMI = PercentageValue.MeasureRatio(bmi);
            BF = PercentageValue.MeasurePercentage(bf);
            FFM = ffm;
            FM = fm;
            Tricep = tricep;
            Chest = chest;
            Armpit = armpit;
            Subscapular = subscapular;
            Suprailiac = suprailiac;
            Abdomen = abdomen;
            Thigh = thigh;

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for as-is measures, no automatic computations performed here.
        /// </summary>
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
        public static PlicometryValue Measure
        (
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            float bmi,
            float bf,
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
            return new PlicometryValue(weight, height, bmi, bf, fm, ffm, tricep, chest, armpit, subscapular, suprailiac, abdomen, thigh);
        }


        /// <summary>
        /// Factory for the Jackson Pollock 7 caliper method
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
        public static PlicometryValue JacksonPollockMeasures7
        (
            GenderTypeEnum gender,
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
            float bmi = height.Value / weight.Value;

            float bf = ComputeJacksonPollock7(gender);

            BodyWeightValue fm = BodyWeightValue.Measure(weight.Value * bf);
            BodyWeightValue ffm = BodyWeightValue.Measure(weight.Value * (1 - bf));


            return Measure(weight, height, bmi, bf, fm, ffm, tricep, chest, armpit, subscapular, suprailiac, abdomen, thigh);   
        }

        /// <summary>
        /// Factory for the Jackson Pollock 4 caliper method
        /// </summary>
        /// <param name="gender">The gender</param>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="tricep">The tricep skinfold</param>
        /// <param name="suprailiac">The suprailiac skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue JacksonPollockMeasures4
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue tricep ,
            CaliperSkinfoldValue suprailiac,
            CaliperSkinfoldValue abdomen,
            CaliperSkinfoldValue thigh)
        {
            float bmi = height.Value / weight.Value;

            float bf = ComputeJacksonPollock4(gender);

            BodyWeightValue fm = BodyWeightValue.Measure(weight.Value * bf);
            BodyWeightValue ffm = BodyWeightValue.Measure(weight.Value * (1 - bf));


            return Measure(weight, height, bmi, bf, fm, ffm, tricep, null, null, null, suprailiac, abdomen, thigh);
        }

        /// <summary>
        /// Factory for the Jackson Pollock 3 caliper method
        /// </summary>
        /// <param name="gender">The gender</param>
        /// <param name="weight">The weight</param>
        /// <param name="height">The height</param>
        /// <param name="chest">The chest skinfold</param>
        /// <param name="abdomen">The abdomen skinfold</param>
        /// <param name="thigh">The thigh skinfold</param>
        /// <returns>A new PlicometryValue instance</returns>
        public static PlicometryValue JacksonPollockMeasures3
        (
            GenderTypeEnum gender,
            BodyWeightValue weight,
            BodyCircumferenceValue height,
            CaliperSkinfoldValue chest,
            CaliperSkinfoldValue abdomen,
            CaliperSkinfoldValue thigh)
        {
            float bmi = height.Value / weight.Value;

            float bf = ComputeJacksonPollock3(gender);

            BodyWeightValue fm = BodyWeightValue.Measure(weight.Value * bf);
            BodyWeightValue ffm = BodyWeightValue.Measure(weight.Value * (1 - bf));


            return Measure(weight, height, bmi, bf, fm, ffm, null, chest, null, null, null, abdomen, thigh);
        }
        #endregion



        #region Business Methods

        /// <summary>
        /// Computes the BF with the Jackson Pollock 7 Caliper method
        /// </summary>
        /// <returns>The BF percentage</returns>
        public static float ComputeJacksonPollock7
        (
            GenderTypeEnum gender)
        {

        }

        /// <summary>
        /// Computes the BF with the Jackson Pollock 4 Caliper method
        /// </summary>
        /// <returns>The BF percentage</returns>
        public static float ComputeJacksonPollock4
        (
            GenderTypeEnum gender)
        {

        }

        /// <summary>
        /// Computes the BF with the Jackson Pollock 3 Caliper method
        /// </summary>
        /// <returns>The BF percentage</returns>
        public static float ComputeJacksonPollock3
        (
            GenderTypeEnum gender)
        {

        }

        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
            => GetAtomicValues().All(x => x is null);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Weight;
            yield return Height;
            yield return BMI;
            yield return BF;
            yield return FFM;
            yield return FM;
            yield return TBW;
            yield return ECW;
            yield return ICW;
            yield return ECWPerc;
            yield return ICWPerc;
            yield return BCM;
            yield return ECM;
            yield return BCMi;
            yield return BMR;
            yield return ECMatrix;
            yield return HPA;
        }


    }
}
