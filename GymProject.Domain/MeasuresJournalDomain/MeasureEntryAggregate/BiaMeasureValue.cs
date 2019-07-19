using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class BiaMeasureValue : ValueObject
    {


        /// <summary>
        /// Body Weight
        /// </summary>
        public BodyWeightValue Weight { get; private set; } = null;

        /// <summary>
        /// Body Height
        /// </summary>
        public BodyMeasureValue Height { get; private set; } = null;

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
        public PercentageValue ECWPerc { get; private set; } = null;

        /// <summary>
        /// Intracellular Body Water Vs Total [%]
        /// </summary>
        public PercentageValue ICWPerc { get; private set; } = null;

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
        public PercentageValue BCMi { get; private set; } = null;

        /// <summary>
        /// Basal Metabolic Rate
        /// </summary>
        public CalorieValue BMR { get; private set; } = null;

        /// <summary>
        /// ExtraCellular Matrix [%]
        /// </summary>
        public PercentageValue ECMatrix { get; private set; } = null;

        /// <summary>
        /// Phase Angle [PA°]
        /// </summary>
        public GenericPureNumberValue HPA { get; private set; } = null;

        /// <summary>
        /// FK to the BIA device
        /// </summary>
        public IdType BiaDeviceId { get; private set; } 


        #region Ctors

        private BiaMeasureValue(
            BodyWeightValue Weight = null,
            BodyMeasureValue Height = null,
            float? BMI = null,
            BodyFatValue BF = null,
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
            this.BMI = BMI == null ? null : PercentageValue.MeasureRatio(BMI.Value, 1);
            this.BF = BF;
            this.FFM = FFM;
            this.FM = FM;
            this.TBW = TBW;
            this.ECW = ECW;
            this.ICW = ICW;
            this.ECWPerc = ECWPerc == null ? null : PercentageValue.MeasurePercentage(ECWPerc.Value, 1);
            this.ICWPerc = ICWPerc == null ? null : PercentageValue.MeasurePercentage(ICWPerc.Value, 1);
            this.BCM = BCM;
            this.ECM = ECM;
            this.BCMi = BCMi == null ? null : PercentageValue.MeasureRatio(BCMi.Value, 1);
            this.BMR = BMR;
            this.ECMatrix = ECMatrix == null ? null : PercentageValue.MeasurePercentage(ECMatrix.Value, 1);
            this.HPA = HPA == null ? null : GenericPureNumberValue.Measure(HPA.Value, 1);

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");
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
        public static BiaMeasureValue Track(
            BodyWeightValue weight = null,
            BodyMeasureValue height = null,
            float? bmi = null,
            BodyFatValue bf = null,
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

            => new BiaMeasureValue(weight, height, bmi, bf, ffm, fm, tbw, ecw, icw, ecwPerc, icwPerc, bcm, ecm, bcmi, bmr, ecmatrix, hpa);


        /// <summary>
        /// Factory methods with the non-derivable values only
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
        public static BiaMeasureValue Compute(
        BodyWeightValue weight = null,
        BodyMeasureValue height = null,
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

            BodyFatValue bf;
            float? ecwPerc = null;
            float? icwPerc = null;
            float? bmi = null;
            float? bcmi = null;

            if (fm == null)
            {
                bf = BodyMeasuresCalculator.ComputeBodyFatFFM(weight, ffm);
                fm = BodyMeasuresCalculator.ComputeFatMass(weight, bf);
            }
            else
            {
                bf = BodyMeasuresCalculator.ComputeBodyFat(weight, fm);
                ffm = ffm ?? BodyMeasuresCalculator.ComputeFatFreeMass(weight, fm);
            }

            if (tbw == null && icw != null && ecw != null)
                tbw = VolumeValue.Measure(icw.Value + ecw.Value, icw.Unit);

            if(tbw != null)
            {
                if (icw != null)
                {
                    ecw = ecw ?? VolumeValue.Measure(tbw.Value - icw.Value, tbw.Unit);
                    icwPerc = icw.Value / tbw.Value * 100f;
                    ecwPerc = ecw.Value / tbw.Value * 100f;
                }
                    
                if(ecw != null)
                {                    
                    icw = icw ?? VolumeValue.Measure(tbw.Value - ecw.Value, tbw.Unit);
                    icwPerc = icw.Value / tbw.Value * 100f;
                    ecwPerc = ecw.Value / tbw.Value * 100f;
                }
            }

            if(height != null && weight != null)
                bmi = BodyMeasuresCalculator.ComputeBodyMassIndex(height, weight).Value;

            if(ecm != null && bcm != null)
                bcmi = ecm.Value / bcm.Value;

            return Track(weight, height, bmi, bf, ffm, fm, tbw, ecw, icw, ecwPerc, icwPerc, bcm, ecm, bcmi, bmr, ecmatrix, hpa);
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
