using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Test.Util;
using System;
using Xunit;
using GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate;

namespace GymProject.Domain.Test.UnitTest
{
    public class MeasureEntryAggregateTest
    {


        [Fact]
        public void PlicometryTrackMeasuresMeasUnitFail()
        {
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureInches(1740f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureInches(1f);
            Assert.Throws<UnsupportedMeasureException>(() => PlicometryValue.TrackMeasures(weight, height, thigh: thigh));  
        }


        [Fact]
        public void PlicometryTrackMeasuresMeasUnitFail2()
        {
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureInches(1f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(1f);
            Assert.Throws<UnsupportedMeasureException>(() => PlicometryValue.TrackMeasures(weight, height, thigh: thigh, abdomen: abdomen));
        }


        [Fact]
        public void PlicometryJAcksonPollockMeasUnitFail3()
        {
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureInches(1f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(1f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(1f);
            Assert.Throws<UnsupportedMeasureException>(() => PlicometryValue.ComputeJacksonPollockMale3(33, weight, height, thigh: thigh, abdomen: abdomen, chest: chest));
        }


        [Fact]
        public void PlicometryTrackMeasuresNullFail()
        {
            Assert.Throws<GlobalDomainGenericException>(() => PlicometryValue.TrackMeasures());
        }


        [Fact]
        public void PlicometryTrackMeasuresMetric()
        {
            float bmiNum = 21.2f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(40f);
            BodyFatValue bf = BodyFatValue.MeasureBodyFat(4.591f);
            PercentageValue bmi = PercentageValue.MeasureRatio(bmiNum);

            PlicometryValue plico = PlicometryValue.TrackMeasures(weight, height, chest: chest, abdomen: abdomen, thigh: thigh, ffm: ffm, bf: bf, bmi: bmiNum);

            Assert.NotNull(plico);
            Assert.Equal(weight, plico.Weight);
            Assert.Equal(height, plico.Height);
            Assert.Equal(chest, plico.Chest);
            Assert.Equal(abdomen, plico.Abdomen);
            Assert.Equal(thigh, plico.Thigh);
            Assert.Equal(ffm, plico.FFM);
            Assert.Equal(bf, plico.BF);
            Assert.Equal(bmi, plico.BMI);

            Assert.Null(plico.Subscapular);
            Assert.Null(plico.Suprailiac);
            Assert.Null(plico.Tricep);
            Assert.Null(plico.Midaxillary);
            Assert.Null(plico.FM);

            Assert.Equal(0, plico.Age);
            Assert.Equal(GenderTypeEnum.NotSet, plico.Gender);
            Assert.Equal(PlicometryFormulaEnum.NotSet, plico.Formula);
        }



        [Fact]
        public void PlicometryComputeMaleMetricJP7()
        {
            ushort age = 34;
            GenderTypeEnum male = GenderTypeEnum.Male;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue tricep = CaliperSkinfoldValue.MeasureMillimeters(3.5f);
            CaliperSkinfoldValue subscapular = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue suprailiac = CaliperSkinfoldValue.MeasureMillimeters(9f);
            CaliperSkinfoldValue armpit = CaliperSkinfoldValue.MeasureMillimeters(4.46f);

            PlicometryValue plico = PlicometryValue.ComputeJacksonPollock7(male, age, weight, height, chest: chest, abdomen: abdomen, thigh: thigh, tricep: tricep, 
                armpit: armpit, suprailiac: suprailiac, subscapular: subscapular);

            Assert.NotNull(plico);
            Assert.Equal(PlicometryFormulaEnum.JacksonPollock7, plico.Formula);
            Assert.Equal(age, plico.Age);
            Assert.Equal(weight, plico.Weight);
            Assert.Equal(height, plico.Height);
            Assert.Equal(chest, plico.Chest);
            Assert.Equal(abdomen, plico.Abdomen);
            Assert.Equal(thigh, plico.Thigh);
            Assert.Equal(subscapular, plico.Subscapular);
            Assert.Equal(suprailiac, plico.Suprailiac);
            Assert.Equal(tricep, plico.Tricep);
            Assert.Equal(armpit, plico.Midaxillary);
            Assert.Equal(male, plico.Gender);

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(6.91f);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(4.96f);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(66.84f);
            PercentageValue bmi = PercentageValue.MeasureRatio(weight.Value / height.Value / height.Value * 10000);

            Assert.Equal(bf, plico.BF);
            Assert.Equal(ffm, plico.FFM);
            Assert.Equal(fm, plico.FM);
            Assert.Equal(bmi, plico.BMI);
        }



        [Fact]
        public void PlicometryComputeFemaleMetricJP7()
        {
            ushort age = 34;
            GenderTypeEnum male = GenderTypeEnum.Female;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue tricep = CaliperSkinfoldValue.MeasureMillimeters(3.5f);
            CaliperSkinfoldValue subscapular = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue suprailiac = CaliperSkinfoldValue.MeasureMillimeters(9f);
            CaliperSkinfoldValue armpit = CaliperSkinfoldValue.MeasureMillimeters(4.46f);

            PlicometryValue plico = PlicometryValue.ComputeJacksonPollock7(male, age, weight, height, chest: chest, abdomen: abdomen, thigh: thigh, tricep: tricep,
                armpit: armpit, suprailiac: suprailiac, subscapular: subscapular);

            Assert.NotNull(plico);
            Assert.Equal(PlicometryFormulaEnum.JacksonPollock7, plico.Formula);
            Assert.Equal(age, plico.Age);
            Assert.Equal(weight, plico.Weight);
            Assert.Equal(height, plico.Height);
            Assert.Equal(chest, plico.Chest);
            Assert.Equal(abdomen, plico.Abdomen);
            Assert.Equal(thigh, plico.Thigh);
            Assert.Equal(subscapular, plico.Subscapular);
            Assert.Equal(suprailiac, plico.Suprailiac);
            Assert.Equal(tricep, plico.Tricep);
            Assert.Equal(armpit, plico.Midaxillary);
            Assert.Equal(male, plico.Gender);

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(11.66f);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(8.37f);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(63.43f);
            PercentageValue bmi = PercentageValue.MeasureRatio(weight.Value / height.Value / height.Value * 10000);

            Assert.Equal(ffm, plico.FFM);
            Assert.Equal(bf, plico.BF);
            Assert.Equal(fm, plico.FM);
            Assert.Equal(bmi, plico.BMI);
        }



        [Fact]
        public void PlicometryComputeMaleImperialJP7()
        {
            ushort age = 34;
            GenderTypeEnum male = GenderTypeEnum.Male;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue tricep = CaliperSkinfoldValue.MeasureMillimeters(3.5f);
            CaliperSkinfoldValue subscapular = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue suprailiac = CaliperSkinfoldValue.MeasureMillimeters(9f);
            CaliperSkinfoldValue armpit = CaliperSkinfoldValue.MeasureMillimeters(4.46f);

            PlicometryValue plico = PlicometryValue.ComputeJacksonPollock7(male, age, weight, height, chest: chest, abdomen: abdomen, thigh: thigh, tricep: tricep,
                armpit: armpit, suprailiac: suprailiac, subscapular: subscapular);

            PlicometryValue plico2 = PlicometryValue.ComputeJacksonPollock7(male, age
                , weight.Convert(WeightUnitMeasureEnum.Pounds)
                , height.Convert(LengthMeasureUnitEnum.Inches)
                , chest: chest.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , abdomen: abdomen.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , thigh: thigh.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , tricep: tricep.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , armpit: armpit.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , suprailiac: suprailiac.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , subscapular: subscapular.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                );

            Assert.NotEqual(plico, plico2);
            //Assert.Equal(plico.BF, plico2.BF);
            StaticUtils.CheckConversions(plico.FFM.Value, plico2.FFM.Convert(plico.FFM.Unit).Value);
            StaticUtils.CheckConversions(plico.FM.Value, plico2.FM.Convert(plico.FM.Unit).Value);
            Assert.Equal(plico.FFM.Value, plico2.FFM.Convert(plico.FFM.Unit).Value);
            Assert.Equal(plico.FM.Value, plico2.FM.Convert(plico.FM.Unit).Value);
            Assert.Equal(plico.BMI, plico2.BMI);
        }



        [Fact]
        public void PlicometryComputeFemaleImperialJP7()
        {
            ushort age = 34;
            GenderTypeEnum male = GenderTypeEnum.Female;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue tricep = CaliperSkinfoldValue.MeasureMillimeters(3.5f);
            CaliperSkinfoldValue subscapular = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue suprailiac = CaliperSkinfoldValue.MeasureMillimeters(9f);
            CaliperSkinfoldValue armpit = CaliperSkinfoldValue.MeasureMillimeters(4.46f);

            PlicometryValue plico = PlicometryValue.ComputeJacksonPollock7(male, age, weight, height, chest: chest, abdomen: abdomen, thigh: thigh, tricep: tricep,
                armpit: armpit, suprailiac: suprailiac, subscapular: subscapular);

            PlicometryValue plico2 = PlicometryValue.ComputeJacksonPollock7(male, age
                , weight.Convert(WeightUnitMeasureEnum.Pounds)
                , height.Convert(LengthMeasureUnitEnum.Inches)
                , chest: chest.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , abdomen: abdomen.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , thigh: thigh.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , tricep: tricep.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , armpit: armpit.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , suprailiac: suprailiac.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                , subscapular: subscapular.ConvertMetricVsImperial(LengthMeasureUnitEnum.Inches)
                );

            Assert.NotEqual(plico, plico2);
            Assert.Equal(Math.Round(plico.BF.Value, 1), Math.Round(plico2.BF.Value, 1));
            //StaticUtils.CheckConversions(plico.FFM.Value, plico2.FFM.Convert(plico.FFM.Unit).Value);
            //StaticUtils.CheckConversions(plico.FM.Value, plico2.FM.Convert(plico.FM.Unit).Value);
            Assert.Equal(plico.FFM.Value, plico2.FFM.Convert(plico.FFM.Unit).Value);
            Assert.Equal(plico.FM.Value, plico2.FM.Convert(plico.FM.Unit).Value);
            Assert.Equal(plico.BMI, plico2.BMI);
        }



        [Fact]
        public void BiaTrackMeasure()
        {
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(174f);
            CalorieValue bmr = CalorieValue.MeasureKcal(2700.33f);
            BodyFatValue bf = BodyFatValue.MeasureBodyFat(11.491f);
            VolumeValue tbw = VolumeValue.MeasureLiters(51.3f);
            VolumeValue ecw = VolumeValue.MeasureLiters(11.3f);
            VolumeValue icw = VolumeValue.MeasureLiters(40f);
            float? hpa = 4.71f;



            BiaMeasureValue bia = BiaMeasureValue.Track(weight: weight, height: height, bmr: bmr, bf: bf, tbw: tbw, ecw: ecw, icw: icw, hpa: hpa);

            Assert.NotNull(bia);
            Assert.Equal(weight, bia.Weight);
            Assert.Equal(height, bia.Height);
            Assert.Equal(bmr, bia.BMR);
            Assert.Equal(bf, bia.BF);
            Assert.Equal(tbw, bia.TBW);
            Assert.Equal(ecw, bia.ECW);
            Assert.Equal(icw, bia.ICW);
            Assert.Equal(GenericPureNumberValue.Measure(hpa.Value, 1), bia.HPA);

            Assert.Null(bia.BCM);
            Assert.Null(bia.BCMi);
            Assert.Null(bia.BMI);
            Assert.Null(bia.ECM);
            Assert.Null(bia.ECMatrix);
            Assert.Null(bia.ECWPerc);
            Assert.Null(bia.FFM);
            Assert.Null(bia.FM);
            Assert.Null(bia.ICWPerc);
        }



        [Fact]
        public void BiaCompute()
        {
            float w = 71.8f, h = 174f, bfval = 0.1151f; ;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            CalorieValue bmr = CalorieValue.MeasureKcal(2700.33f);
            VolumeValue tbw = VolumeValue.MeasureLiters(51.3f);
            VolumeValue ecw = VolumeValue.MeasureLiters(11.3f);
            float? hpa = 4.71f;

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfval * 100);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(w * bfval);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(w - fm.Value);


            BiaMeasureValue bia = BiaMeasureValue.Compute(weight: weight, height: height, bmr: bmr, 
                tbw: tbw, ecw: ecw, hpa: hpa, fm: fm, ffm: ffm);

            Assert.NotNull(bia);
            Assert.Equal(weight, bia.Weight);
            Assert.Equal(height, bia.Height);
            Assert.Equal(bmr, bia.BMR);
            Assert.Equal(tbw, bia.TBW);
            Assert.Equal(ecw, bia.ECW);
            Assert.Equal(GenericPureNumberValue.Measure(hpa.Value, 1), bia.HPA);

            // Derived values
            VolumeValue icw = VolumeValue.MeasureLiters(40f);
            PercentageValue bmi = PercentageValue.MeasureRatio(w / h / h * 10000);

            Assert.Equal(icw, bia.ICW);
            Assert.Equal(ecw, bia.ECW);
            StaticUtils.CheckConversions(bf.Value, bia.BF.Value);
            Assert.Equal(bmi, bia.BMI);
        }



        [Fact]
        public void BiaCompute2()
        {
            float w = 71.8f, h = 174f, bfval = 0.1191f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            CalorieValue bmr = CalorieValue.MeasureKcal(2700.33f);
            VolumeValue tbw = VolumeValue.MeasureLiters(51.3f);
            VolumeValue ecw = VolumeValue.MeasureLiters(11.3f);
            float? hpa = 4.71f;

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfval * 100);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(w * bfval);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(w - fm.Value);


            BiaMeasureValue bia = BiaMeasureValue.Compute(weight: weight, height: height, bmr: bmr,
                tbw: tbw, ecw: ecw, hpa: hpa, ffm: ffm);

            Assert.NotNull(bia);
            Assert.Equal(weight, bia.Weight);
            Assert.Equal(height, bia.Height);
            Assert.Equal(bmr, bia.BMR);
            Assert.Equal(tbw, bia.TBW);
            Assert.Equal(ecw, bia.ECW);
            Assert.Equal(GenericPureNumberValue.Measure(hpa.Value, 1), bia.HPA);

            // Derived values
            VolumeValue icw = VolumeValue.MeasureLiters(40f);
            PercentageValue bmi = PercentageValue.MeasureRatio(w / h / h * 10000);

            Assert.Equal(icw, bia.ICW);
            Assert.Equal(ecw, bia.ECW);
            StaticUtils.CheckConversions(bf.Value, bia.BF.Value, tolerance: 0.055f);
            Assert.Equal(bmi, bia.BMI);
        }



        [Fact]
        public void BiaCompute3()
        {
            float w = 71.8f, h = 174f, bfval = 0.1191f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            CalorieValue bmr = CalorieValue.MeasureKcal(2700.33f);
            VolumeValue icw = VolumeValue.MeasureLiters(40f);
            VolumeValue ecw = VolumeValue.MeasureLiters(11.3f);
            float? hpa = 4.71f;

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfval * 100);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(w * bfval);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(w - fm.Value);


            BiaMeasureValue bia = BiaMeasureValue.Compute(weight: weight, height: height, bmr: bmr,
                icw: icw, ecw: ecw, hpa: hpa, ffm: ffm);

            Assert.NotNull(bia);
            Assert.Equal(weight, bia.Weight);
            Assert.Equal(height, bia.Height);
            Assert.Equal(bmr, bia.BMR);
            Assert.Equal(ecw, bia.ECW);
            Assert.Equal(GenericPureNumberValue.Measure(hpa.Value, 1), bia.HPA);

            // Derived values
            VolumeValue tbw = VolumeValue.MeasureLiters(51.3f);
            PercentageValue icwperc = PercentageValue.MeasurePercentage(icw.Value / tbw.Value * 100f);
            PercentageValue bmi = PercentageValue.MeasureRatio(w / h / h * 10000);

            Assert.Equal(icw, bia.ICW);
            Assert.Equal(ecw, bia.ECW);
            Assert.Equal(tbw, bia.TBW);
            Assert.Equal(icwperc, bia.ICWPerc);
            StaticUtils.CheckConversions(bf.Value, bia.BF.Value, tolerance: 0.055f);
            Assert.Equal(bmi, bia.BMI);
        }



        [Fact]
        public void BiaCompute4()
        {
            float w = 71.8f, h = 174f, bfval = 0.1191f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyWeightValue ecm = BodyWeightValue.MeasureKilograms(4);
            BodyWeightValue bcm = BodyWeightValue.MeasureKilograms(5);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            CalorieValue bmr = CalorieValue.MeasureKcal(2700.33f);
            VolumeValue tbw = VolumeValue.MeasureLiters(51.3f);
            VolumeValue ecw = VolumeValue.MeasureLiters(11.3f);
            float? hpa = 4.71f;

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfval * 100);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(w * bfval);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(w - fm.Value);


            BiaMeasureValue bia = BiaMeasureValue.Compute(weight: weight, height: height, bmr: bmr,
                tbw: tbw, ecw: ecw, hpa: hpa, ffm: ffm, ecm: ecm, bcm: bcm);


            // Derived values
            VolumeValue icw = VolumeValue.MeasureLiters(40f);
            PercentageValue icwperc = PercentageValue.MeasurePercentage(icw.Value / tbw.Value * 100f);
            PercentageValue bmi = PercentageValue.MeasureRatio(w / h / h * 10000);

            Assert.Equal(icw, bia.ICW);
            Assert.Equal(ecw, bia.ECW);
            Assert.Equal(tbw, bia.TBW);
            Assert.Equal(PercentageValue.MeasureRatio(ecm.Value / bcm.Value), bia.BCMi);
            Assert.Equal(icwperc, bia.ICWPerc);
            StaticUtils.CheckConversions(bf.Value, bia.BF.Value, tolerance: 0.055f);
            Assert.Equal(bmi, bia.BMI);
        }


        [Fact]
        public void CircumferenceTrackMeasuresNullFail()
        {
            Assert.Throws<GlobalDomainGenericException>(() => CircumferenceMeasureValue.TrackMeasures());
        }


        [Fact]
        public void CircumferenceTrackMeasuresUnitFail()
        {
            float w = 71.8f, h = 174f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureInches(h * 2.2f);
            BodyMeasureValue neck = BodyMeasureValue.MeasureCentimeters(44f);

            Assert.Throws<UnsupportedMeasureException>(() => CircumferenceMeasureValue.TrackMeasures(weight: weight, height: height, neck: neck));
        }


        [Fact]
        public void CircumferenceTrackMeasuresUnitFail2()
        {
            float w = 71.8f, h = 174f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            BodyMeasureValue neck = BodyMeasureValue.MeasureInches(44f);

            Assert.Throws<UnsupportedMeasureException>(() => CircumferenceMeasureValue.TrackMeasures(weight: weight, height: height, neck: neck));
        }


        [Fact]
        public void CircumferenceTrackMeasures()
        {
            float w = 71.8f, h = 174f, bfnum = 8.7f;
            GenderTypeEnum gender = GenderTypeEnum.Male;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            BodyMeasureValue neck = BodyMeasureValue.MeasureCentimeters(44f);
            BodyMeasureValue abdomen = BodyMeasureValue.MeasureCentimeters(79.5f);
            BodyMeasureValue chest = BodyMeasureValue.MeasureCentimeters(106f);
            BodyMeasureValue larm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue rarm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue lleg = BodyMeasureValue.MeasureCentimeters(57f);
            BodyMeasureValue rleg = BodyMeasureValue.MeasureCentimeters(57f);

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfnum);

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.TrackMeasures(weight: weight, height: height, gender: gender, 
                neck: neck, abdomen: abdomen, chest: chest, leftArm: larm, rightArm: rarm, leftLeg: lleg, rightLeg: rleg, bf: bf);

            Assert.NotNull(circ);
            Assert.Equal(gender, circ.Gender);
            Assert.Equal(weight, circ.Weight);
            Assert.Equal(height, circ.Height);
            Assert.Equal(gender, circ.Gender);
            Assert.Equal(neck, circ.Neck);
            Assert.Equal(abdomen, circ.Abdomen);
            Assert.Equal(chest, circ.Chest);
            Assert.Equal(larm, circ.LeftArm);
            Assert.Equal(rarm, circ.RightArm);
            Assert.Equal(lleg, circ.LeftLeg);
            Assert.Equal(rleg, circ.RightLeg);
            Assert.Equal(bf, circ.BF);
            Assert.Equal(CircumferenceFormulaEnum.NotSet, circ.Formula);

            Assert.Null(circ.LeftForearm);
            Assert.Null(circ.RightForearm);
            Assert.Null(circ.LeftCalf);
            Assert.Null(circ.RightCalf);
            Assert.Null(circ.Waist);
            Assert.Null(circ.Shoulders);
            Assert.Null(circ.Hips);
            Assert.Null(circ.FM);
            Assert.Null(circ.FFM);
        }


        [Fact]
        public void CircumferenceComputeMaleMetric()
        {
            float w = 71.8f, h = 174f, bfnum = 6.7f;
            GenderTypeEnum gender = GenderTypeEnum.Male;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            BodyMeasureValue neck = BodyMeasureValue.MeasureCentimeters(44f);
            BodyMeasureValue abdomen = BodyMeasureValue.MeasureCentimeters(79.5f);
            BodyMeasureValue chest = BodyMeasureValue.MeasureCentimeters(106f);
            BodyMeasureValue larm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue rarm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue lleg = BodyMeasureValue.MeasureCentimeters(57f);
            BodyMeasureValue rleg = BodyMeasureValue.MeasureCentimeters(57f);

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfnum);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(4.8f);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(67f);

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.ComputeHodgonAndBeckett(weight: weight, height: height, gender: gender,
                neck: neck, abdomen: abdomen, chest: chest, leftArm: larm, rightArm: rarm, leftLeg: lleg, rightLeg: rleg);


            Assert.NotNull(circ);
            Assert.Equal(gender, circ.Gender);
            Assert.Equal(weight, circ.Weight);
            Assert.Equal(height, circ.Height);
            Assert.Equal(gender, circ.Gender);
            Assert.Equal(neck, circ.Neck);
            Assert.Equal(abdomen, circ.Abdomen);
            Assert.Equal(chest, circ.Chest);
            Assert.Equal(larm, circ.LeftArm);
            Assert.Equal(rarm, circ.RightArm);
            Assert.Equal(lleg, circ.LeftLeg);
            Assert.Equal(rleg, circ.RightLeg);

            Assert.Equal(bf, circ.BF);
            Assert.Equal(fm, circ.FM);
            Assert.Equal(ffm, circ.FFM);
            Assert.Equal(CircumferenceFormulaEnum.HodgonAndBeckett, circ.Formula);

            Assert.Null(circ.LeftForearm);
            Assert.Null(circ.RightForearm);
            Assert.Null(circ.LeftCalf);
            Assert.Null(circ.RightCalf);
            Assert.Null(circ.Waist);
            Assert.Null(circ.Shoulders);
            Assert.Null(circ.Hips);
        }


        [Fact]
        public void CircumferenceComputeFemaleMetric()
        {
            float w = 71.8f, h = 174f, bfnum = 15.85f;
            GenderTypeEnum gender = GenderTypeEnum.Female;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            BodyMeasureValue neck = BodyMeasureValue.MeasureCentimeters(35);
            BodyMeasureValue abdomen = BodyMeasureValue.MeasureCentimeters(79.5f);
            BodyMeasureValue chest = BodyMeasureValue.MeasureCentimeters(106f);
            BodyMeasureValue larm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue rarm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue lleg = BodyMeasureValue.MeasureCentimeters(57f);
            BodyMeasureValue rleg = BodyMeasureValue.MeasureCentimeters(57f);
            BodyMeasureValue hips = BodyMeasureValue.MeasureCentimeters(90.5f);
            BodyMeasureValue waist = BodyMeasureValue.MeasureCentimeters(65f);
            BodyMeasureValue lforearm = BodyMeasureValue.MeasureCentimeters(7f);
            BodyMeasureValue rforearm = BodyMeasureValue.MeasureCentimeters(7f);
            BodyMeasureValue lcalf = BodyMeasureValue.MeasureCentimeters(17f);
            BodyMeasureValue rcalf = BodyMeasureValue.MeasureCentimeters(17f);
            BodyMeasureValue shoulders = BodyMeasureValue.MeasureCentimeters(44f);

            BodyFatValue bf = BodyFatValue.MeasureBodyFat(bfnum);
            BodyWeightValue fm = BodyWeightValue.MeasureKilograms(11.38f);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(60.42f);

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.ComputeHodgonAndBeckett(weight: weight, height: height, gender: gender,
                neck: neck, abdomen: abdomen, chest: chest, leftArm: larm, rightArm: rarm, leftLeg: lleg, rightLeg: rleg,
                hips: hips, waist: waist, leftForearm: lforearm, rightForearm: rforearm, leftCalf: lcalf, rightCalf: rcalf, shoulders: shoulders);


            Assert.NotNull(circ);
            Assert.Equal(gender, circ.Gender);
            Assert.Equal(weight, circ.Weight);
            Assert.Equal(height, circ.Height);
            Assert.Equal(gender, circ.Gender);
            Assert.Equal(neck, circ.Neck);
            Assert.Equal(abdomen, circ.Abdomen);
            Assert.Equal(chest, circ.Chest);
            Assert.Equal(larm, circ.LeftArm);
            Assert.Equal(rarm, circ.RightArm);
            Assert.Equal(lleg, circ.LeftLeg);
            Assert.Equal(rleg, circ.RightLeg);
            Assert.Equal(waist, circ.Waist);
            Assert.Equal(hips, circ.Hips);
            Assert.Equal(lforearm, circ.LeftForearm);
            Assert.Equal(rforearm, circ.RightForearm);
            Assert.Equal(lcalf, circ.LeftCalf);
            Assert.Equal(rcalf, circ.RightCalf);
            Assert.Equal(shoulders, circ.Shoulders);

            Assert.Equal(bf, circ.BF);
            Assert.Equal(fm, circ.FM);
            Assert.Equal(ffm, circ.FFM);
            Assert.Equal(CircumferenceFormulaEnum.HodgonAndBeckett, circ.Formula);
        }


        [Fact]
        public void CircumferenceComputeMaleImperial()
        {
            float w = 71.8f, h = 174f;
            GenderTypeEnum gender = GenderTypeEnum.Male;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            BodyMeasureValue neck = BodyMeasureValue.MeasureCentimeters(44f);
            BodyMeasureValue abdomen = BodyMeasureValue.MeasureCentimeters(79.5f);
            BodyMeasureValue chest = BodyMeasureValue.MeasureCentimeters(106f);
            BodyMeasureValue larm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue rarm = BodyMeasureValue.MeasureCentimeters(37.5f);
            BodyMeasureValue lleg = BodyMeasureValue.MeasureCentimeters(57f);
            BodyMeasureValue rleg = BodyMeasureValue.MeasureCentimeters(57f);


            CircumferenceMeasureValue circ = CircumferenceMeasureValue.ComputeHodgonAndBeckett(weight: weight, height: height, gender: gender,
                neck: neck, abdomen: abdomen, chest: chest, leftArm: larm, rightArm: rarm, leftLeg: lleg, rightLeg: rleg);


            CircumferenceMeasureValue circ2 = CircumferenceMeasureValue.ComputeHodgonAndBeckett(
                weight: weight.Convert(WeightUnitMeasureEnum.Pounds), 
                height: height.Convert(LengthMeasureUnitEnum.Inches), 
                gender: gender,
                neck: neck.Convert(LengthMeasureUnitEnum.Inches), 
                abdomen: abdomen.Convert(LengthMeasureUnitEnum.Inches), 
                chest: chest.Convert(LengthMeasureUnitEnum.Inches));


            Assert.NotEqual(circ, circ2);
            StaticUtils.CheckConversions(circ.BF.Value, circ2.BF.Value, tolerance: 0.2f);
            StaticUtils.CheckConversions(circ.FFM.Value, circ2.FFM.Convert(WeightUnitMeasureEnum.Kilograms).Value, tolerance: 0.2f);
            StaticUtils.CheckConversions(circ.FM.Value, circ2.FM.Convert(WeightUnitMeasureEnum.Kilograms).Value, tolerance: 0.2f);
        }


        [Fact]
        public void CircumferenceComputeFemaleImperial()
        {
            float w = 71.8f, h = 174f;
            GenderTypeEnum gender = GenderTypeEnum.Female;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(w);
            BodyMeasureValue height = BodyMeasureValue.MeasureCentimeters(h);
            BodyMeasureValue neck = BodyMeasureValue.MeasureCentimeters(34f);
            BodyMeasureValue waist = BodyMeasureValue.MeasureCentimeters(62f);
            BodyMeasureValue hips = BodyMeasureValue.MeasureCentimeters(91.5f);


            CircumferenceMeasureValue circ = CircumferenceMeasureValue.ComputeHodgonAndBeckett(weight: weight, height: height, gender: gender, waist:waist, hips: hips, neck: neck);


            CircumferenceMeasureValue circ2 = CircumferenceMeasureValue.ComputeHodgonAndBeckett(
                weight: weight.Convert(WeightUnitMeasureEnum.Pounds),
                height: height.Convert(LengthMeasureUnitEnum.Inches),
                gender: gender,
                neck: neck.Convert(LengthMeasureUnitEnum.Inches),
                waist: waist.Convert(LengthMeasureUnitEnum.Inches),
                hips: hips.Convert(LengthMeasureUnitEnum.Inches));


            Assert.NotEqual(circ, circ2);
            StaticUtils.CheckConversions(circ.BF.Value, circ2.BF.Value, tolerance: 0.2f);
            StaticUtils.CheckConversions(circ.FFM.Value, circ2.FFM.Convert(WeightUnitMeasureEnum.Kilograms).Value, tolerance: 0.2f);
            StaticUtils.CheckConversions(circ.FM.Value, circ2.FM.Convert(WeightUnitMeasureEnum.Kilograms).Value, tolerance: 0.2f);
        }


        [Fact]
        public void CreateMeasuresEntry()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);

            Assert.NotNull(entry);
            Assert.Equal(rating, entry.Rating);
            Assert.Equal(date, entry.EntryDate);
            Assert.Null(entry.DomainEvents);
            Assert.Null(entry.BodyFat);
        }


        [Fact]
        public void CreateMeasuresEntry2()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date);

            Assert.NotNull(entry);
            Assert.Null(entry.Rating);
            Assert.Null(entry.OwnerNote);
            Assert.Equal(date, entry.EntryDate);
            Assert.Null(entry.DomainEvents);
            Assert.Null(entry.Owner);
            Assert.Null(entry.BodyFat);
        }


        [Fact]
        public void ChangeMeasuresEntry()
        {
            DateTime fakedate = DateTime.MinValue;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;
            string note = "new note";
            OwnerEntity owner = OwnerEntity.Register("owner1", "ciao.img");

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(fakedate);
            entry.ChangeDate(date);
            entry.ChangeRating(rating);
            entry.UpdateNote(note);
            entry.RegisterOwner(owner);

            Assert.NotNull(entry);
            Assert.Null(entry.BodyFat);
            Assert.Equal(rating, entry.Rating);
            Assert.Equal(date, entry.EntryDate);
            Assert.Equal(note, entry.OwnerNote);
            Assert.Equal(owner, entry.Owner);
            Assert.Equal(4, entry.DomainEvents.Count);
        }


        [Fact]
        public void AttachPlicometry()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            PlicometryValue plicometry = PlicometryValue.ComputeJacksonPollockFemale3(
                33,
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                CaliperSkinfoldValue.MeasureMillimeters(4),
                CaliperSkinfoldValue.MeasureMillimeters(8),
                CaliperSkinfoldValue.MeasureMillimeters(5.5f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);
            entry.AttachPlicometryCheck(plicometry);
            eventsCounter++;

            Assert.NotNull(entry);
            Assert.Equal(plicometry.BF, entry.BodyFat);
            Assert.Equal(plicometry, entry.Plicometry);
            Assert.Null(entry.BiaMeasure);
            Assert.Null(entry.CircumferencesMeasure);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void RemovePlicometry()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            PlicometryValue plicometry = PlicometryValue.ComputeJacksonPollockFemale3(
                33,
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                CaliperSkinfoldValue.MeasureMillimeters(4),
                CaliperSkinfoldValue.MeasureMillimeters(8),
                CaliperSkinfoldValue.MeasureMillimeters(5.5f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);
            entry.AttachPlicometryCheck(plicometry);
            eventsCounter++;
            entry.RemovePlicometry();
            eventsCounter++;
            eventsCounter++;

            Assert.NotNull(entry);
            Assert.Null(entry.BodyFat);
            Assert.Null(entry.Plicometry);
            Assert.Null(entry.BiaMeasure);
            Assert.Null(entry.CircumferencesMeasure);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void AttachBIA()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            BiaMeasureValue bia = BiaMeasureValue.Track(
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                bf: BodyFatValue.MeasureBodyFat(5.6f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);
            entry.AttachBiaCheck(bia);
            eventsCounter++;

            Assert.NotNull(entry);
            Assert.Equal(bia.BF, entry.BodyFat);
            Assert.Equal(bia, entry.BiaMeasure);
            Assert.Null(entry.Plicometry);
            Assert.Null(entry.CircumferencesMeasure);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void RemoveBIA()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            BiaMeasureValue bia = BiaMeasureValue.Track(
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                bf: BodyFatValue.MeasureBodyFat(5.6f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);

            entry.AttachBiaCheck(bia);
            eventsCounter++;
            entry.RemoveBia();
            eventsCounter++;
            eventsCounter++;

            Assert.NotNull(entry);
            Assert.Null(entry.BodyFat);
            Assert.Null(entry.Plicometry);
            Assert.Null(entry.BiaMeasure);
            Assert.Null(entry.CircumferencesMeasure);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void AttachCircumferences()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.TrackMeasures(
                BodyMeasureValue.MeasureCentimeters(174),
                BodyWeightValue.MeasureKilograms(72),
                GenderTypeEnum.Male,
                bf: BodyFatValue.MeasureBodyFat(12.6f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);
            entry.AttachCircumferenceCheck(circ);
            eventsCounter++;

            Assert.NotNull(entry);
            Assert.Equal(circ.BF, entry.BodyFat);
            Assert.Equal(circ, entry.CircumferencesMeasure);
            Assert.Null(entry.Plicometry);
            Assert.Null(entry.BiaMeasure);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void RemoveCircumferences()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.TrackMeasures(
                BodyMeasureValue.MeasureCentimeters(174),
                BodyWeightValue.MeasureKilograms(72),
                GenderTypeEnum.Male,
                bf: BodyFatValue.MeasureBodyFat(12.6f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);

            entry.AttachCircumferenceCheck(circ);
            eventsCounter++;
            entry.RemoveCircumferencesCheck();
            eventsCounter++;
            eventsCounter++;

            Assert.NotNull(entry);
            Assert.Null(entry.BodyFat);
            Assert.Null(entry.Plicometry);
            Assert.Null(entry.BiaMeasure);
            Assert.Null(entry.CircumferencesMeasure);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void CheckBodyFatValueBiaWins()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            PlicometryValue plicometry = PlicometryValue.ComputeJacksonPollockFemale3(
                33,
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                CaliperSkinfoldValue.MeasureMillimeters(4),
                CaliperSkinfoldValue.MeasureMillimeters(8),
                CaliperSkinfoldValue.MeasureMillimeters(5.5f));

            BiaMeasureValue bia = BiaMeasureValue.Track(
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                bf: BodyFatValue.MeasureBodyFat(5.6f));

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.TrackMeasures(
                BodyMeasureValue.MeasureCentimeters(174),
                BodyWeightValue.MeasureKilograms(72),
                GenderTypeEnum.Male,
                bf: BodyFatValue.MeasureBodyFat(12.6f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);

            entry.AttachCircumferenceCheck(circ);
            eventsCounter++;

            entry.AttachBiaCheck(bia);
            eventsCounter++;

            entry.AttachPlicometryCheck(plicometry);
            eventsCounter++;

            Assert.Equal(bia.BF, entry.BodyFat);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }


        [Fact]
        public void CheckBodyFatValuePlicometryWins()
        {
            int eventsCounter = 0;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            PlicometryValue plicometry = PlicometryValue.ComputeJacksonPollockFemale3(
                33,
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                CaliperSkinfoldValue.MeasureMillimeters(4),
                CaliperSkinfoldValue.MeasureMillimeters(8),
                CaliperSkinfoldValue.MeasureMillimeters(5.5f));

            BiaMeasureValue bia = BiaMeasureValue.Track(
                BodyWeightValue.MeasureKilograms(72),
                BodyMeasureValue.MeasureCentimeters(174),
                bf: BodyFatValue.MeasureBodyFat(5.6f));

            CircumferenceMeasureValue circ = CircumferenceMeasureValue.TrackMeasures(
                BodyMeasureValue.MeasureCentimeters(174),
                BodyWeightValue.MeasureKilograms(72),
                GenderTypeEnum.Male,
                bf: BodyFatValue.MeasureBodyFat(12.6f));

            DailyMeasuresEntity entry = DailyMeasuresEntity.StartTrackingMeasures(date, rating);

            entry.AttachCircumferenceCheck(circ);
            eventsCounter++;

            entry.AttachBiaCheck(bia);
            eventsCounter++;

            entry.AttachPlicometryCheck(plicometry);
            eventsCounter++;

            entry.RemoveBia();
            eventsCounter++;
            //eventsCounter++;

            Assert.Equal(plicometry.BF, entry.BodyFat);
            Assert.Equal(eventsCounter, entry.DomainEvents.Count);
        }
    }
}
