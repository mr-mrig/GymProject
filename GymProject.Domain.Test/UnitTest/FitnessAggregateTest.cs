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
using System.Collections.ObjectModel;

namespace GymProject.Domain.Test.UnitTest
{
    public class FitnessAggregateTest
    {


        [Fact]
        public void CheckCalorieValue()
        {
            CalorieValue cal = CalorieValue.Measure(2300.77f);
            CalorieValue cal2 = CalorieValue.Measure(2301f, CaloriesMeasureUnitEnum.Kilocals);
            CalorieValue cal3 = CalorieValue.MeasureKcal(2301.2f);
            //CalorieValue calKj = CalorieValue.Measure(9623.2f, CaloriesMeasureUnitEnum.KiloJoules);
            CalorieValue calKj = cal.Convert(CaloriesMeasureUnitEnum.KiloJoules);

            Assert.NotNull(cal);
            Assert.Equal(cal, cal2);
            Assert.Equal(cal, cal3);
            Assert.Equal(2301f, cal.Value);
            Assert.Equal(cal, calKj.Convert(CaloriesMeasureUnitEnum.Kilocals));
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
