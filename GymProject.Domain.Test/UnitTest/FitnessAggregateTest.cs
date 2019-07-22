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
        public void CreateEmptyFitnessDaySuccess()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
        }


        [Fact]
        public void CreateFitnessDayWithWeight()
        {
            float fakeWeight = 71.5f;
            float weight = 101.1f; 

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
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
        public void CreateFitnessDayWithWeight2()
        {
            float fakeWeight = 71.5f;
            float weight = 101.1f;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
            fd.TrackWeight(fakeWeight, WeightUnitMeasureEnum.Kilograms);
            fd.TrackWeight(weight, WeightUnitMeasureEnum.Kilograms);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
            Assert.Equal(weight, fd.DailyWeight.Value);
            Assert.Equal(WeightUnitMeasureEnum.Kilograms, fd.DailyWeight.Unit);
        }


        [Fact]
        public void CreateFitnessDayWithWeightPounds()
        {
            float fakeWeight = 100.5f;
            float weight = 170.1f;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
            fd.TrackWeightPounds(fakeWeight);
            fd.TrackWeightPounds(weight);

            Assert.NotNull(fd);
            Assert.Equal(DateTime.Today, fd.DayDate);
            Assert.Equal(rating, fd.Rating);
            Assert.Equal(weight, fd.DailyWeight.Value);
            Assert.Equal(WeightUnitMeasureEnum.Pounds, fd.DailyWeight.Unit);
        }


        [Fact]
        public void CreateFitnessDayWithDiet()
        {
            float carbs1 = 100, fats1 = 90, pro1 = 150, salt1 = 2.2f;
            bool isFreeMeal1 = true;
            DietDayTypeEnum dayType1 = DietDayTypeEnum.On;

            float carbs = 500, fats = 60f, pro = 150;
            bool isFreeMeal = false;
            DietDayTypeEnum dayType = DietDayTypeEnum.On;

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
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
        public void CreateFitnessDayWithDietFail()
        {

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
            Assert.Throws<GlobalDomainGenericException>(() => fd.TrackDiet());
        }


        [Fact]
        public void CreateFitnessDayWithActivity()
        {
            uint? steps = 1500;
            uint? stairs = 71;
            CalorieValue burned = CalorieValue.Measure(2222.5f);
            SleepDurationValue sleepMinutes = SleepDurationValue.MeasureMinutes(444f);
            RatingValue sleepRating = RatingValue.Rate(4);
            HeartRateValue restHr = HeartRateValue.Measure(100f);

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
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
        public void CreateFitnessDayWithWellnessNoMus()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);
            GlycemiaValue glycemia = GlycemiaValue.Measure(88.7f);

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
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
        public void CreateFitnessDayWithWellnessWithMus()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);

            List<MusReference> muses = new List<MusReference>()
            {
                MusReference.MusLink(new IdType(12), "insomnia"),
            };


            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
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
        public void CreateFitnessDayWithWellnessWithMus2()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);

            List<MusReference> muses = new List<MusReference>()
            {
                MusReference.MusLink(new IdType(12), "insomnia"),
            };

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
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


        [Fact]
        public void CreateFitnessDayWellnessUntrack()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);

            fd.TrackWellnessDay(celsius);
            fd.UntrackWellness();

            Assert.Null(fd.DailyWellness);
            Assert.Equal(3, fd.DomainEvents.Count);
        }


        [Fact]
        public void CreateFitnessDayDietUntrack()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);

            fd.TrackDiet(MacronutirentWeightValue.MeasureGrams(300));
            fd.UntrackDiet();

            Assert.Null(fd.DailyDiet);
            Assert.Equal(3, fd.DomainEvents.Count);
        }


        [Fact]
        public void CreateFitnessDayWeightUntrack()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);

            fd.TrackWeightKilograms(66);
            fd.UntrackWeight();

            Assert.Null(fd.DailyWeight);
            Assert.Equal(3, fd.DomainEvents.Count);
        }


        [Fact]
        public void CreateFitnessDayActivityUntrack()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);

            fd.TrackActivity(66);
            fd.UntrackActivity();

            Assert.Null(fd.DailyActivity);
            Assert.Equal(3, fd.DomainEvents.Count);
        }


        [Fact]
        public void CreateFitnessDayMultipleEntriesUntrack()
        {
            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);

            fd.TrackActivity(66);
            fd.TrackWeightKilograms(44);
            fd.UntrackActivity();                           // Don't raise the event for ClearAll

            Assert.Null(fd.DailyActivity);
            Assert.NotEqual(4, fd.DomainEvents.Count);      // ClearAll not raised
            Assert.Equal(3, fd.DomainEvents.Count);
        }


        [Fact]
        public void CreateFitnessDayWellnessRemoveMus()
        {
            TemperatureValue celsius = TemperatureValue.Measure(26.8f);

            List<MusReference> muses = new List<MusReference>()
            {
                MusReference.MusLink(new IdType(12), "insomnia"),           // One
            };

            RatingValue rating = RatingValue.Rate(4.2f);
            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, rating);
            fd.TrackWellnessDay(celsius, musList: muses);

            MusReference fakeMus = MusReference.MusLink(new IdType(1000), "WillBeRemoved");
            MusReference fakeMus2 = MusReference.MusLink(new IdType(1001), "WillBeRemoved2");

            fd.DiagnoseMus(fakeMus);
            fd.DiagnoseMus(MusReference.MusLink(new IdType(500), "Added1"));    // Two
            fd.UndiagnoseMus(fakeMus);

            fd.DiagnoseMus(MusReference.MusLink(new IdType(501), "Added2"));    // Three
            fd.DiagnoseMus(fakeMus2);
            fd.UndiagnoseMus(new IdType(1001));

            Assert.Equal(3, fd.DailyWellness.MusList.Count);
            Assert.DoesNotContain(fakeMus, fd.DailyWellness.MusList);
            Assert.DoesNotContain(fakeMus2, fd.DailyWellness.MusList);

            Assert.Equal(7, fd.DomainEvents.Count);
        }


        [Fact]
        public void ChangeFitnessDay
            ()
        {


            RatingValue fake = RatingValue.Rate(4.2f);
            RatingValue rating = RatingValue.Rate(4.2f);

            DateTime fakeDate = DateTime.MinValue;
            DateTime date = DateTime.Today;

            FitnessDay fd = FitnessDay.StartTrackingDay(fakeDate, fake);

            fd.ChangeRating(rating);
            fd.ChangeDate(date);


            Assert.Equal(rating, fd.Rating);
            Assert.Equal(date, fd.DayDate);
            Assert.Equal(2, fd.DomainEvents.Count);
        }


        [Fact]
        public void LinkFitnessDayToPost()
        {

            int postId = 100;
            RatingValue rating = RatingValue.Rate(4.2f);
            DateTime date = DateTime.Today;

            FitnessDay fd = FitnessDay.StartTrackingDay(date, rating);

            fd.LinkToPost(new IdType(postId));

            Assert.Equal(rating, fd.Rating);
            Assert.Equal(date, fd.DayDate);
            Assert.Equal(postId, fd.PostId.Id);
            Assert.Null(fd.DomainEvents);
        }


    }
}
