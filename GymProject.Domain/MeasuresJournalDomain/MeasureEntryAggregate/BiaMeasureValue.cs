using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    class BiaMeasureValue : ValueObject
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
        public GenericPureNumberValue BMI { get; private set; } = null;

        /// <summary>
        /// Body Fat
        /// </summary>
        public GenericPureNumberValue BF { get; private set; } = null;

        /// <summary>
        /// Free-Fat Mass
        /// </summary>
        public BodyWeightValue FFM { get; private set; } = null;

        /// <summary>
        /// Fat Mass
        /// </summary>
        public BodyWeightValue FM { get; private set; } = null;

        /// <summary>
        /// Total Body Water
        /// </summary>
        public VolumeValue TBW { get; private set; } = null;

        /// <summary>
        /// Extracellular Body Water
        /// </summary>
        public VolumeValue ECW { get; private set; } = null;

        /// <summary>
        /// Intracellular Body Water
        /// </summary>
        public VolumeValue ICW { get; private set; } = null;

        /// <summary>
        /// Extracellular Body Water Vs Total [%]
        /// </summary>
        public GenericPureNumberValue ECWPerc { get; private set; } = null;

        /// <summary>
        /// Intracellular Body Water Vs Total [%]
        /// </summary>
        public GenericPureNumberValue ICWPerc { get; private set; } = null;

        /// <summary>
        /// Body Cell Mass
        /// </summary>
        public BodyWeightValue BCM { get; private set; } = null;

        /// <summary>
        /// ExtraCellular Mass
        /// </summary>
        public BodyWeightValue ECM { get; private set; } = null;

        /// <summary>
        /// Body Cell Mass Index - ECM / BCM
        /// </summary>
        public GenericPureNumberValue BCMi { get; private set; } = null;

        /// <summary>
        /// Basal Metabolic Rate
        /// </summary>
        public CalorieValue BMR { get; private set; } = null;

        /// <summary>
        /// ExtraCellular Matrix [%]
        /// </summary>
        public GenericPureNumberValue ECMatrix { get; private set; } = null;

        /// <summary>
        /// Phase Angle [PA°]
        /// </summary>
        public GenericPureNumberValue HPA { get; private set; } = null;




        #region Ctors

        private BiaMeasureValue(
            BodyWeightValue Weight = null,
            BodyCircumferenceValue Height = null,
            float? BMI = null,
            float? BF = null,
            BodyWeightValue FFM = null,
            BodyWeightValue FM = null,
            VolumeValue TBW = null,
            VolumeValue ECW = null,
            VolumeValue ICW = null,
            float? ECWPerc = null,
            float? ICWPerc = null,
            BodyWeightValue BCM = null,
            BodyWeightValue ECM = null,
            float? BCMi = null,
            CalorieValue BMR = null,
            float? ECMatrix = null,
            float? HPA = null)
        {
            this.Weight = Weight;
            this.Height = Height;
            this.BMI = BMI;
            this.BF = BF;
            this.FFM = FFM;
            this.FM = FM;
            this.TBW = TBW;
            this.ECW = ECW;
            this.ICW = ICW;
            this.ECWPerc = ECWPerc;
            this.ICWPerc = ICWPerc;
            this.BCM = BCM;
            this.ECM = ECM;
            this.BCMi = BCMi;
            this.BMR = BMR;
            this.ECMatrix = ECMatrix;
            this.HPA = HPA;

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

            //// Here so it is excluded from the CheckNullState
            //DietDayType = dayType;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method with all the possible values
        /// </summary>
        /// <param name="Weight"></param>
        /// <param name="Height"></param>
        /// <param name="BMI"></param>
        /// <param name="BF"></param>
        /// <param name="FFM"></param>
        /// <param name="FM"></param>
        /// <param name="TBW"></param>
        /// <param name="ECW"></param>
        /// <param name="ICW"></param>
        /// <param name="ECWPerc"></param>
        /// <param name="ICWPerc"></param>
        /// <param name="BCM"></param>
        /// <param name="ECM"></param>
        /// <param name="BCMi"></param>
        /// <param name="BMR"></param>
        /// <param name="ECMatrix"></param>
        /// <param name="HPA"></param>
        /// <returns></returns>
        public static BiaMeasureValue Measure(
            BodyWeightValue weight = null,
            BodyCircumferenceValue height = null,
            float? bmi = null,
            float? bf = null,
            BodyWeightValue ffm = null,
            BodyWeightValue fm = null,
            VolumeValue tbw = null,
            VolumeValue ecw = null,
            VolumeValue icw = null,
            float? ecwPerc = null,
            float? icwPerc = null,
            BodyWeightValue bcm = null,
            BodyWeightValue ecm = null,
            float? bcmi = null,
            CalorieValue bmr = null,
            float? ecmatrix = null,
            float? hpa = null)
        {
            return new BiaMeasureValue(weight, height, bmi, bf, ffm, fm, tbw, ecw, icw, ecwPerc, icwPerc, bcm, ecm, bcmi, bmr, ecmatrix, hpa);
        }

        /// <summary>
        /// Factory methods with the non-derivble values only
        /// IE: BMI is derived from height / weight, BF from weight / FM etc.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        /// <param name="ffm"></param>
        /// <param name="fm"></param>
        /// <param name="tbw"></param>
        /// <param name="ecw"></param>
        /// <param name="icw"></param>
        /// <param name="bcm"></param>
        /// <param name="ecm"></param>
        /// <param name="bmr"></param>
        /// <param name="ecmatrix"></param>
        /// <param name="hpa"></param>
        /// <returns></returns>
        public static BiaMeasureValue Measure(
        BodyWeightValue weight = null,
        BodyCircumferenceValue height = null,
        BodyWeightValue ffm = null,
        BodyWeightValue fm = null,
        VolumeValue tbw = null,
        VolumeValue ecw = null,
        VolumeValue icw = null,
        BodyWeightValue bcm = null,
        BodyWeightValue ecm = null,
        CalorieValue bmr = null,
        float? ecmatrix = null,
        float? hpa = null)
        {
            float? bmi = height.Value / weight.Value;
            float? bf = weight.Value / fm.Value;
            float? ecwPerc = ecw.Value / tbw.Value;
            float? icwPerc = icw.Value / tbw.Value;
            float? bcmi = ecm.Value / bcm.Value;

            return Measure(weight, height, bmi, bf, ffm, fm, tbw, ecw, icw, ecwPerc, icwPerc, bcm, ecm, bcmi, bmr, ecmatrix, hpa);
        }

        #endregion



        #region Business Methods


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
