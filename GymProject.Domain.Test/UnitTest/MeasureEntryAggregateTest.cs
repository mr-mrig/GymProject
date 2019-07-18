using GymProject.Domain.SharedKernel;
using GymProject.Domain.FitnessJournalDomain.Common;
using GymProject.Domain.FitnessJournalDomain.Exceptions;
using GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate;
using GymProject.Domain.Test.Util;
using System;
using System.Collections;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate;

namespace GymProject.Domain.Test.UnitTest
{
    public class MeasureEntryAggregateTest
    {



        [Fact]
        public void CheckCaliperSkinfoldValue()
        {
            CaliperSkinfoldValue millimeters = CaliperSkinfoldValue.Measure(6.7f);
            CaliperSkinfoldValue millimeters2 = CaliperSkinfoldValue.MeasureMillimeters(6.91f);
            CaliperSkinfoldValue inches = CaliperSkinfoldValue.Measure(0.118f, LengthMeasureUnitEnum.Inches);
            CaliperSkinfoldValue inches2 = CaliperSkinfoldValue.MeasureInches(0.121f);

            Assert.NotNull(millimeters);
            Assert.Equal(millimeters, millimeters2);
            Assert.Equal(7f, millimeters.Value);
            Assert.NotNull(inches2);
            Assert.Equal(inches, inches2);
            Assert.Equal(0.12f, inches.Value);

            // Metric 2 Metric Conversions
            CaliperSkinfoldValue sourceMetric = CaliperSkinfoldValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            CaliperSkinfoldValue metersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Meters);
            CaliperSkinfoldValue centimetersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Centimeters);
            CaliperSkinfoldValue millimetersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Millimeters);

            Assert.Equal(sourceMetric.Value * 1000f, metersConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Meters, metersConv.Unit);
            Assert.Equal(sourceMetric.Value * 100000f, centimetersConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Centimeters, centimetersConv.Unit);
            Assert.Equal(sourceMetric.Value * 1000000f, millimetersConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Millimeters, millimetersConv.Unit);

            // Imperial 2 Imperial Conversions
            CaliperSkinfoldValue sourceImperial = CaliperSkinfoldValue.Measure(0.3f, LengthMeasureUnitEnum.Miles);
            CaliperSkinfoldValue yardsConv = sourceMetric.Convert(LengthMeasureUnitEnum.Meters);
            CaliperSkinfoldValue inchesConv = sourceMetric.Convert(LengthMeasureUnitEnum.Centimeters);
            

            Assert.Equal(sourceImperial.Value * 1760, yardsConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Yards, yardsConv.Unit);
            Assert.Equal(sourceImperial.Value * 63360f, inchesConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, inchesConv.Unit);


            // Metric 2 Imperial Conversions
            CaliperSkinfoldValue sourceMixed = CaliperSkinfoldValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            CaliperSkinfoldValue milesConv = sourceMixed.Convert(LengthMeasureUnitEnum.Miles);
            CaliperSkinfoldValue sourceMixed2 = CaliperSkinfoldValue.Measure(101.67f, LengthMeasureUnitEnum.Centimeters);
            CaliperSkinfoldValue cmToInches = sourceMixed2.Convert(LengthMeasureUnitEnum.Inches);
            CaliperSkinfoldValue sourceMixed3 = CaliperSkinfoldValue.Measure(101.67f, LengthMeasureUnitEnum.Millimeters);
            CaliperSkinfoldValue mmToInches = sourceMixed3.Convert(LengthMeasureUnitEnum.Inches);

            // km to miles
            Assert.Equal(sourceMixed.Value / 1.609f, milesConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Miles, milesConv.Unit);
            // cm to inches
            Assert.Equal(sourceMixed2.Value / 2.54f, cmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, cmToInches.Unit);
            // mm to inches
            Assert.Equal(sourceMixed3.Value / 25.4f, mmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, mmToInches.Unit);
        }

        [Fact]
        public void CheckCircumferenceSkinfoldValue()
        {
            BodyCircumferenceValue cm = BodyCircumferenceValue.Measure(66.72f);
            BodyCircumferenceValue cm2 = BodyCircumferenceValue.MeasureCentimeters(66.68f);
            BodyCircumferenceValue inches = BodyCircumferenceValue.Measure(0.118f, LengthMeasureUnitEnum.Inches);
            BodyCircumferenceValue inches2 = BodyCircumferenceValue.MeasureInches(0.121f);

            Assert.NotNull(cm);
            Assert.Equal(cm, cm2);
            Assert.Equal(66.7f, cm.Value);
            Assert.NotNull(inches2);
            Assert.Equal(inches, inches2);
            Assert.Equal(0.12f, inches.Value);

            // Metric 2 Metric Conversions
            BodyCircumferenceValue sourceMetric = BodyCircumferenceValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            BodyCircumferenceValue metersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Meters);
            BodyCircumferenceValue centimetersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Centimeters);
            BodyCircumferenceValue millimetersConv = sourceMetric.Convert(LengthMeasureUnitEnum.Millimeters);

            Assert.Equal(sourceMetric.Value * 1000f, metersConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Meters, metersConv.Unit);
            Assert.Equal(sourceMetric.Value * 100000f, centimetersConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Centimeters, centimetersConv.Unit);
            Assert.Equal(sourceMetric.Value * 1000000f, millimetersConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Millimeters, millimetersConv.Unit);

            // Imperial 2 Imperial Conversions
            BodyCircumferenceValue sourceImperial = BodyCircumferenceValue.Measure(0.3f, LengthMeasureUnitEnum.Miles);
            BodyCircumferenceValue yardsConv = sourceMetric.Convert(LengthMeasureUnitEnum.Meters);
            BodyCircumferenceValue inchesConv = sourceMetric.Convert(LengthMeasureUnitEnum.Centimeters);

            Assert.Equal(sourceImperial.Value * 1760, yardsConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Yards, yardsConv.Unit);
            Assert.Equal(sourceImperial.Value * 63360f, inchesConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, inchesConv.Unit);

            // Metric 2 Imperial Conversions
            BodyCircumferenceValue sourceMixed = BodyCircumferenceValue.Measure(0.3f, LengthMeasureUnitEnum.Kilometers);
            BodyCircumferenceValue milesConv = sourceMixed.Convert(LengthMeasureUnitEnum.Miles);
            BodyCircumferenceValue sourceMixed2 = BodyCircumferenceValue.Measure(101.67f, LengthMeasureUnitEnum.Centimeters);
            BodyCircumferenceValue cmToInches = sourceMixed2.Convert(LengthMeasureUnitEnum.Inches);
            BodyCircumferenceValue sourceMixed3 = BodyCircumferenceValue.Measure(101.67f, LengthMeasureUnitEnum.Millimeters);
            BodyCircumferenceValue mmToInches = sourceMixed3.Convert(LengthMeasureUnitEnum.Inches);

            // km to miles
            Assert.Equal(sourceMixed.Value / 1.609f, milesConv.Value);
            Assert.Equal(LengthMeasureUnitEnum.Miles, milesConv.Unit);
            // cm to inches
            Assert.Equal(sourceMixed2.Value / 2.54f, cmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, cmToInches.Unit);
            // mm to inches
            Assert.Equal(sourceMixed3.Value / 25.4f, mmToInches.Value);
            Assert.Equal(LengthMeasureUnitEnum.Inches, mmToInches.Unit);
        }


        [Fact]
        public void CheckPercentageValue()
        {
            PercentageValue perc = PercentageValue.MeasurePercentage(70.26f);
            PercentageValue perc2 = PercentageValue.MeasurePercentage(70.3f);
            PercentageValue perc3 = PercentageValue.MeasurePercentage(70.26f, 2);
            PercentageValue perc4 = PercentageValue.MeasurePercentage(50.1f, 0);

            Assert.NotNull(perc);
            Assert.Equal(perc, perc2);
            Assert.NotEqual(perc, perc3);
            Assert.Equal(70.3, perc.Value);
            Assert.Equal(PercentageMeasureUnitEnum.Percentage, perc.Unit);
            Assert.Equal(70.26f, perc3.Value);
            Assert.Equal(50f, perc4.Value);

            PercentageValue ratio = PercentageValue.MeasureRatio(perc.Value / 100);
            PercentageValue ratio2 = PercentageValue.MeasureRatio(perc.Value / 100 + 0.001f);
            PercentageValue ratio3 = PercentageValue.MeasureRatio(perc.Value / 100, 4);
            PercentageValue ratio4 = PercentageValue.MeasureRatio(perc4.Value / 100, 4);

            Assert.NotNull(perc);
            Assert.Equal(ratio, ratio2);
            Assert.NotEqual(ratio, ratio3);
            Assert.Equal(Math.Round(70.3 / 100, 1), ratio.Value);
            Assert.Equal(PercentageMeasureUnitEnum.Ratio, ratio.Unit);
            Assert.Equal(Math.Round(70.26f / 100, 4), ratio3.Value);
            Assert.Equal(Math.Round(50f/100, 4), ratio4.Value);

            Assert.Equal(perc4.Value / 100f, ratio4.Value);

            PercentageValue test = PercentageValue.MeasurePercentage(90f);
            PercentageValue percToRatio = test.Convert(PercentageMeasureUnitEnum.Ratio);

            Assert.Equal(test.Value, percToRatio.Convert(PercentageMeasureUnitEnum.Percentage).Value);
        }


        [Fact]
        public void CheckBodyFat()
        {
            BodyFatValue bf = BodyFatValue.MeasureBodyFat(7.811f);
            BodyFatValue bf2 = BodyFatValue.MeasureBodyFat(7.8f);

            Assert.NotNull(bf);
            Assert.Equal(bf, bf2);
            Assert.Equal(7.81f, bf.Value);
        }


        [Fact]
        public void PlicometryTrackMeasuresFail()
        {
            Assert.Throws<Exception>(() => PlicometryValue.TrackMeasures());  
        }


        [Fact]
        public void PlicometryTrackMeasuresMetric()
        {
            float bmiNum = 21.2f;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyCircumferenceValue height = BodyCircumferenceValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            BodyWeightValue ffm = BodyWeightValue.MeasureKilograms(40f);
            BodyFatValue bf = BodyFatValue.MeasureBodyFat(4.591f);
            PercentageValue bmi = PercentageValue.MeasureRatio(bmiNum);

            PlicometryValue plico = PlicometryValue.TrackMeasures(weight, height, chest: chest, abdomen: abdomen, thigh: thigh, ffm: ffm, bf: bf, bmi: bmiNum);

            Assert.NotNull(plico);
            Assert.Equal(PlicometryFormulaEnum.NotSet, plico.Formula);
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
            Assert.Null(plico.Gender);
            Assert.Null(plico.Armpit);
            Assert.Null(plico.FM);
            Assert.Equal(0, plico.Age);
            Assert.Null(plico.Armpit);
        }



        [Fact]
        public void PlicometryTrackMeasuresMetricMale()
        {
            ushort age = 34;
            GenderTypeEnum male = GenderTypeEnum.Male;
            BodyWeightValue weight = BodyWeightValue.MeasureKilograms(71.8f);
            BodyCircumferenceValue height = BodyCircumferenceValue.MeasureCentimeters(174f);
            CaliperSkinfoldValue chest = CaliperSkinfoldValue.MeasureMillimeters(5f);
            CaliperSkinfoldValue abdomen = CaliperSkinfoldValue.MeasureMillimeters(10f);
            CaliperSkinfoldValue thigh = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue subscapular = CaliperSkinfoldValue.MeasureMillimeters(7f);
            CaliperSkinfoldValue suprailiac = CaliperSkinfoldValue.MeasureMillimeters(9f);
            CaliperSkinfoldValue tricep = CaliperSkinfoldValue.MeasureMillimeters(3.5f);
            CaliperSkinfoldValue armpit = CaliperSkinfoldValue.MeasureMillimeters(4.46f);

            PlicometryValue plico = PlicometryValue.ComputeJacksonPollock7(male, age, weight, height, chest: chest, abdomen: abdomen, thigh: thigh, tricep: tricep, 
                armpit: armpit, suprailiac: suprailiac, subscapular: subscapular);

            Assert.NotNull(plico);
            Assert.Equal(PlicometryFormulaEnum.JacksonPollock7, plico.Formula);
            AssemblyTraitAttribute.Equals(age, plico.Age);
            Assert.Equal(weight, plico.Weight);
            Assert.Equal(height, plico.Height);
            Assert.Equal(chest, plico.Chest);
            Assert.Equal(abdomen, plico.Abdomen);
            Assert.Equal(thigh, plico.Thigh);
            Assert.Equal(subscapular, plico.Subscapular);
            Assert.Equal(suprailiac, plico.Suprailiac);
            Assert.Equal(tricep, plico.Tricep);
            Assert.Equal(male, plico.Gender);
            Assert.Equal(ffm, plico.FFM);
            Assert.Equal(bf, plico.BF);
            Assert.Equal(bmi, plico.BMI);

      
            Assert.Null(plico.FM);
            Assert.Equal(0, plico.Age);
            Assert.Null(plico.Armpit);
        }













        [Fact]
        public void CheckSleepDurationValue()
        {
            SleepDurationValue hours = SleepDurationValue.Measure(2.24f);
            SleepDurationValue hours2 = SleepDurationValue.Measure(2.24f, TimeMeasureUnitEnum.Hours);
            SleepDurationValue hours3 = SleepDurationValue.MeasureHours(2.24f);
            SleepDurationValue minutes = SleepDurationValue.MeasureMinutes(223.22f);
            SleepDurationValue minutesConv = SleepDurationValue.ToMinutes(hours.Value);
            SleepDurationValue hoursConv = SleepDurationValue.ToHours(minutesConv.Value);

            Assert.NotNull(hours);
            Assert.Equal(hours, hours2);
            Assert.Equal(hours, hours3);
            Assert.Equal(223f, minutes.Value);
            Assert.Equal(2.2f, hours.Value);
            Assert.Equal(minutesConv, SleepDurationValue.ToMinutes(hoursConv.Value));
        }


        [Fact]
        public void CheckHeartRateValue()
        {
            HeartRateValue pulses = HeartRateValue.Measure(121.34f, HeartRateMeasureUnitEnum.Pulses);
            HeartRateValue pulses2 = HeartRateValue.Measure(121f);

            Assert.NotNull(pulses);
            Assert.Equal(pulses, pulses2);
            Assert.Equal(121f, pulses.Value);
        }


        [Fact]
        public void CheckMacronutirentWeightValue()
        {
            MacronutirentWeightValue grams = MacronutirentWeightValue.Measure(100.27f);
            MacronutirentWeightValue grams2 = MacronutirentWeightValue.Measure(100.27f, WeightUnitMeasureEnum.Grams);
            MacronutirentWeightValue grams3 = MacronutirentWeightValue.MeasureGrams(99.81f);
            MacronutirentWeightValue ounces = MacronutirentWeightValue.Measure(55.688f, WeightUnitMeasureEnum.Ounces);
            MacronutirentWeightValue ounces2 = MacronutirentWeightValue.MeasureOunces(55.691f);
            MacronutirentWeightValue ouncesConv = grams.Convert(WeightUnitMeasureEnum.Ounces);
            MacronutirentWeightValue ouncesConv2 = ouncesConv.Convert(WeightUnitMeasureEnum.Ounces);
            MacronutirentWeightValue gramsConv = ounces.Convert(WeightUnitMeasureEnum.Grams);

            Assert.NotNull(grams);
            Assert.Equal(grams2, grams);
            Assert.Equal(grams3, grams);
            Assert.Equal(100f, grams.Value);
            Assert.Equal(ounces2, ounces);
            Assert.Equal(55.7f, ounces.Value);


            StaticUtils.CheckConversions(grams.Value, ouncesConv.Convert(WeightUnitMeasureEnum.Grams).Value, grams.Unit.Id, ouncesConv.Convert(WeightUnitMeasureEnum.Grams).Unit.Id);
            StaticUtils.CheckConversions(ouncesConv2.Value, ouncesConv.Convert(WeightUnitMeasureEnum.Ounces).Value, ouncesConv2.Unit.Id, ouncesConv.Convert(WeightUnitMeasureEnum.Ounces).Unit.Id);
            StaticUtils.CheckConversions(ounces.Value, gramsConv.Convert(WeightUnitMeasureEnum.Ounces).Value, ounces.Unit.Id, gramsConv.Convert(WeightUnitMeasureEnum.Ounces).Unit.Id);
        }



        [Fact]
        public void CheckMicronutirentWeightValue()
        {
            MicronutirentWeightValue _grams = MicronutirentWeightValue.Measure(100.27f);
            MicronutirentWeightValue _grams2 = MicronutirentWeightValue.Measure(100.27f, WeightUnitMeasureEnum.Grams);
            MicronutirentWeightValue _grams3 = MicronutirentWeightValue.MeasureGrams(100.32f);
            MicronutirentWeightValue _ounces = MicronutirentWeightValue.Measure(55.688f, WeightUnitMeasureEnum.Ounces);
            MicronutirentWeightValue _ounces2 = MicronutirentWeightValue.MeasureOunces(55.691f);
            MicronutirentWeightValue _ouncesConv = _grams.Convert(WeightUnitMeasureEnum.Ounces);
            MicronutirentWeightValue _ouncesConv2 = _ouncesConv.Convert(WeightUnitMeasureEnum.Ounces);
            MicronutirentWeightValue _gramsConv = _ounces.Convert(WeightUnitMeasureEnum.Grams);

            Assert.NotNull(_grams);
            Assert.Equal(_grams2, _grams);
            Assert.Equal(_grams3, _grams);
            Assert.Equal(100.3f, _grams.Value);
            Assert.Equal(_ounces2, _ounces);
            Assert.Equal(55.69f, _ounces.Value);
            StaticUtils.CheckConversions(_grams.Value, _ouncesConv.Convert(WeightUnitMeasureEnum.Grams).Value, _grams.Unit.Id, _ouncesConv.Convert(WeightUnitMeasureEnum.Grams).Unit.Id);
            StaticUtils.CheckConversions(_ouncesConv2.Value, _ouncesConv.Convert(WeightUnitMeasureEnum.Ounces).Value, _ouncesConv2.Unit.Id, _ouncesConv.Convert(WeightUnitMeasureEnum.Ounces).Unit.Id);
            StaticUtils.CheckConversions(_ounces.Value, _gramsConv.Convert(WeightUnitMeasureEnum.Ounces).Value, _ounces.Unit.Id, _gramsConv.Convert(WeightUnitMeasureEnum.Ounces).Unit.Id);
        }


        [Fact]
        public void CheckVolumeValue()
        {
            VolumeValue liters = VolumeValue.Measure(44.67f);
            VolumeValue liters2 = VolumeValue.Measure(44.72f, VolumeUnitMeasureEnum.Liters);
            VolumeValue liters3 = VolumeValue.MeasureLiters(44.7f);

            Assert.NotNull(liters);
            Assert.Equal(liters2, liters);
            Assert.Equal(liters3, liters);
            Assert.Equal(44.7f, liters.Value);
        }


        [Fact]
        public void CheckBodyWeightValue()
        {
            BodyWeightValue kilos = BodyWeightValue.Measure(71.88f);
            BodyWeightValue kilos2 = BodyWeightValue.Measure(71.93f, WeightUnitMeasureEnum.Kilograms);
            BodyWeightValue kilos3 = BodyWeightValue.MeasureKilograms(71.9f);
            BodyWeightValue pounds = BodyWeightValue.Measure(99f, WeightUnitMeasureEnum.Pounds);
            BodyWeightValue pounds2 = BodyWeightValue.MeasurePounds(99.02f);

            BodyWeightValue kilosConv = kilos.Convert(WeightUnitMeasureEnum.Kilograms);
            BodyWeightValue kilosConv2 = pounds.Convert(WeightUnitMeasureEnum.Kilograms);
            BodyWeightValue poundsConv = kilos.Convert(WeightUnitMeasureEnum.Pounds);

            Assert.NotNull(kilos);
            Assert.Equal(kilos2, kilos);
            Assert.Equal(kilos3, kilos);
            Assert.Equal(71.9f, kilos.Value);
            Assert.Equal(pounds2, pounds);
            Assert.Equal(kilosConv, kilos);
            Assert.Equal(kilosConv2, pounds.Convert(WeightUnitMeasureEnum.Kilograms));
            Assert.Equal(poundsConv, kilos.Convert(WeightUnitMeasureEnum.Pounds));
        }


        [Fact]
        public void CheckTemperatureValue()
        {
            TemperatureValue celsius = TemperatureValue.Measure(37.22f);
            TemperatureValue celsius2 = TemperatureValue.Measure(37.22f, TemperatureMeasureUnitEnum.Celsius);
            TemperatureValue celsius3 = TemperatureValue.MeasureCelsius(37.17f);
            TemperatureValue f = TemperatureValue.MeasureFahrenheit(88.02f);
            TemperatureValue f2 = TemperatureValue.Measure(88f, TemperatureMeasureUnitEnum.Fahrenheit);
            TemperatureValue celsiusConv = celsius.Convert(TemperatureMeasureUnitEnum.Celsius);
            TemperatureValue celsiusConv2 = f.Convert(TemperatureMeasureUnitEnum.Celsius);
            TemperatureValue fConv = celsius.Convert(TemperatureMeasureUnitEnum.Fahrenheit);

            Assert.NotNull(celsius);
            Assert.Equal(celsius2, celsius);
            Assert.Equal(celsius3, celsius);
            Assert.Equal(37.2f, celsius.Value);
            Assert.Equal(f2, f);
            Assert.Equal(celsiusConv, celsius);
            Assert.Equal(celsiusConv2, f.Convert(TemperatureMeasureUnitEnum.Celsius));
            Assert.Equal(fConv, celsius.Convert(TemperatureMeasureUnitEnum.Fahrenheit));
            Assert.Equal(31.1f, celsiusConv2.Value);
            Assert.Equal(99f, fConv.Value);
        }


        [Fact]
        public void CheckGlycemiaValue()
        {
            GlycemiaValue mg = GlycemiaValue.Measure(177.81f);
            GlycemiaValue mg2 = GlycemiaValue.Measure(177.83f, GlycemiaMeasureUnitEnum.Milligrams);
            GlycemiaValue mg3 = GlycemiaValue.MeasureMg(177.77f);

            Assert.NotNull(mg);
            Assert.Equal(mg2, mg);
            Assert.Equal(mg3, mg);
            Assert.Equal(178f, mg.Value);
        }



        [Fact]
        public void CheckRatingValue()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            Assert.Equal(4, rating.Value);

            RatingValue increased = rating.Increase();
            Assert.Equal(5, increased.Value);

            RatingValue decreased = rating.Decrease();
            Assert.Equal(3, decreased.Value);
        }



        [Fact]
        public void CreateEmptyFitnessDayEntrySuccess()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
        }


        [Fact]
        public void CreateFitnessDayEntryWithWeight()
        {
            float fakeWeight = 71.5f;
            float weight = 101.1f;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackWeightKilograms(fakeWeight);
            fd.TrackWeightKilograms(weight);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
            Assert.Equal(weight, fd.DailyWeight.Value);
            Assert.Equal(WeightUnitMeasureEnum.Kilograms, fd.DailyWeight.Unit);

            // Other entries are null
            Assert.Null(fd.DailyDiet);
            Assert.Null(fd.DailyWellness);
            Assert.Null(fd.DailyActivity);

            // Check for invariance preservation
            fd.DailyWeight.Convert(WeightUnitMeasureEnum.Pounds);
            Assert.Equal(WeightUnitMeasureEnum.Kilograms, fd.DailyWeight.Unit);
        }


        [Fact]
        public void CreateFitnessDayEntryWithWeight2()
        {
            float fakeWeight = 71.5f;
            float weight = 101.1f;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackWeight(fakeWeight, WeightUnitMeasureEnum.Kilograms);
            fd.TrackWeight(weight, WeightUnitMeasureEnum.Kilograms);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
            Assert.Equal(weight, fd.DailyWeight.Value);
            Assert.Equal(WeightUnitMeasureEnum.Kilograms, fd.DailyWeight.Unit);
        }


        [Fact]
        public void CreateFitnessDayEntryWithWeightPounds()
        {
            float fakeWeight = 100.5f;
            float weight = 170.1f;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackWeightPounds(fakeWeight);
            fd.TrackWeightPounds(weight);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
            Assert.Equal(weight, fd.DailyWeight.Value);
            Assert.Equal(WeightUnitMeasureEnum.Pounds, fd.DailyWeight.Unit);
        }


        [Fact]
        public void CreateFitnessDayEntryWithDiet()
        {
            float carbs1 = 100, fats1 = 90, pro1 = 150, salt1 = 2.2f;
            bool isFreeMeal1 = true;
            DietDayTypeEnum dayType1 = DietDayTypeEnum.On;

            float carbs = 500, fats = 60f, pro = 150;
            bool isFreeMeal = false;
            DietDayTypeEnum dayType = DietDayTypeEnum.On;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackDiet(MacronutirentWeightValue.MeasureGrams(carbs1), MacronutirentWeightValue.MeasureGrams(fats1), MacronutirentWeightValue.MeasureGrams(pro1), isFreeMeal: isFreeMeal1, dayType: dayType1);
            fd.TrackDiet(MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(pro), isFreeMeal: isFreeMeal, dayType: dayType);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);

            Assert.Equal(carbs, fd.DailyDiet.Carbs.Value);
            Assert.Equal(WeightUnitMeasureEnum.Grams, fd.DailyDiet.Carbs.Unit);

            Assert.Equal(fats, fd.DailyDiet.Fats.Value);
            Assert.Equal(WeightUnitMeasureEnum.Grams, fd.DailyDiet.Fats.Unit);

            Assert.Equal(pro, fd.DailyDiet.Proteins.Value);
            Assert.Equal(WeightUnitMeasureEnum.Grams, fd.DailyDiet.Proteins.Unit);

            Assert.Equal(isFreeMeal, fd.DailyDiet.IsFreeMeal);
            Assert.Equal(dayType, fd.DailyDiet.DietDayType);

            // Other properties are null
            Assert.Null(fd.DailyDiet.Water);
            Assert.Null(fd.DailyDiet.Salt);

            // Other entries are null
            Assert.Null(fd.DailyWeight);
            Assert.Null(fd.DailyWellness);
            Assert.Null(fd.DailyActivity);
        }


        [Fact]
        public void CreateFitnessDayEntryWithDietFail()
        {

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            Assert.Throws<GlobalDomainGenericException>(() => fd.TrackDiet());
        }


        [Fact]
        public void CreateFitnessDayEntryWithActivity()
        {
            uint? steps = 1500;
            uint? stairs = 71;
            CalorieValue burned = CalorieValue.Measure(2222.5f);
            SleepDurationValue sleepMinutes = SleepDurationValue.MeasureMinutes(444f);
            RatingValue sleepRating = RatingValue.Rate(4);
            HeartRateValue restHr = HeartRateValue.Measure(100f);

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackActivity(steps, stairs, burned, sleepMinutes, sleepRating, restHr);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);

            Assert.Equal(steps, fd.DailyActivity.Steps);
            Assert.Equal(sleepMinutes, fd.DailyActivity.SleepTime);
            Assert.Equal(sleepRating, fd.DailyActivity.SleepQuality);
            Assert.Equal(restHr, fd.DailyActivity.HeartRateRest);

            // Other properties are null
            Assert.Null(fd.DailyActivity.HeartRateMax);

            // Other entries are null
            Assert.Null(fd.DailyWeight);
            Assert.Null(fd.DailyWellness);
            Assert.Null(fd.DailyDiet);
        }


        [Fact]
        public void CreateFitnessDayEntryWithWellnessNoMus()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);
            GlycemiaValue glycemia = GlycemiaValue.Measure(88.7f);

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackWellnessDay(celsius, glycemia);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);

            Assert.Equal(celsius, fd.DailyWellness.Temperature);
            Assert.Equal(glycemia, fd.DailyWellness.Glycemia);
            Assert.Equal(0, fd.DailyWellness.MusList.Count);


            // Other properties are null
            //Assert.Null(fd.DailyActivity.HeartRateMax);

            // Other entries are null
            Assert.Null(fd.DailyWeight);
            Assert.Null(fd.DailyActivity);
            Assert.Null(fd.DailyDiet);
        }


        [Fact]
        public void CreateFitnessDayEntryWithWellnessWithMus()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);

            List<MusReference> muses = new List<MusReference>()
            {
                MusReference.MusLink(new IdType(12), "insomnia"),
            };


            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackWellnessDay(celsius, musList: muses);
            List<MusReference> malevolent = fd.DailyWellness.MusList.ToList();
            malevolent.RemoveAll(x => true);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);

            Assert.Equal(celsius, fd.DailyWellness.Temperature);
            Assert.Equal(1, fd.DailyWellness.MusList.Count);

            Assert.Null(fd.DailyWellness.Glycemia);

            // Other entries are null
            Assert.Null(fd.DailyWeight);
            Assert.Null(fd.DailyActivity);
            Assert.Null(fd.DailyDiet);
        }


        [Fact]
        public void CreateFitnessDayEntryWithWellnessWithMus2()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);

            List<MusReference> muses = new List<MusReference>()
            {
                MusReference.MusLink(new IdType(12), "insomnia"),
            };

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.TrackDay(DateTime.Today, rating);
            fd.TrackWellnessDay(celsius, musList: muses);
            fd.DiagnoseMus(MusReference.MusLink(new IdType(500), "Added"));

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);

            Assert.Equal(celsius, fd.DailyWellness.Temperature);
            Assert.Equal(2, fd.DailyWellness.MusList.Count);

            Assert.Null(fd.DailyWellness.Glycemia);

            // Other entries are null
            Assert.Null(fd.DailyWeight);
            Assert.Null(fd.DailyActivity);
            Assert.Null(fd.DailyDiet);
        }
    }
}
