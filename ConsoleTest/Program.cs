﻿using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using ConsoleTest.DataSeed;
using System.Collections.Generic;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            // Check if seeding is needed
            if (TestDataSeed.IsSeedingRequired())
            {
                SeedData();
            }

            BuildTrainingPlanTestCase();
            //PerformWorkoutTestCase();
        }


        private static void PerformWorkoutTestCase()
        {
            uint excerciseProgressiveNumber = 0;
            uint excerciseId = 0;
            uint noteId = 0;

            using (GymContext context = new GymContext())
            {
                uint? workoutTemplateId = context.WorkoutTemplates.Select(x => x.Id).FirstOrDefault();

                if (!workoutTemplateId.HasValue)
                    throw new InvalidOperationException("No Workout Templates in the DB. Please populate them before trying again.");

                TestServiceLayer service = new TestServiceLayer(context);
                WorkoutSessionRoot workout = service.StartWorkoutSession(workoutTemplateId.Value);

                excerciseId = 1;
                service.StartExcercise(workout.Id.Value, excerciseId);
                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10), WeightPlatesValue.MeasureKilograms(110.2522f));
                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(9), WeightPlatesValue.MeasureKilograms(110.2522f));
                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), WeightPlatesValue.MeasureKilograms(110.25f));
                service.RateExcercisePerformance(workout.Id.Value, excerciseProgressiveNumber, 1f);

                excerciseId = 9;
                excerciseProgressiveNumber++;
                service.StartExcercise(workout.Id.Value, excerciseId);

                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(20), WeightPlatesValue.MeasureKilograms(32));
                noteId = service.WriteWorkingSetNote($"Brand new note0!!");
                service.AttachWorkingSetNote(workout.Id.Value, excerciseProgressiveNumber, 0, noteId);

                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(20), WeightPlatesValue.MeasureKilograms(30));
                noteId = service.WriteWorkingSetNote($"Brand new note1!!");
                service.AttachWorkingSetNote(workout.Id.Value, excerciseProgressiveNumber, 1, noteId);

                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(20), WeightPlatesValue.MeasureKilograms(28));
                noteId = service.WriteWorkingSetNote($"Brand new note2!!");
                service.AttachWorkingSetNote(workout.Id.Value, excerciseProgressiveNumber, 2, noteId);
                service.RateExcercisePerformance(workout.Id.Value, excerciseProgressiveNumber, 3.6f);

                excerciseId = 5;
                excerciseProgressiveNumber++;
                service.StartExcercise(workout.Id.Value, excerciseId);
                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(7), WeightPlatesValue.MeasureKilograms(10));
                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(7), WeightPlatesValue.MeasureKilograms(10));
                service.TrackWorkingSet(workout.Id.Value, excerciseProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(7), WeightPlatesValue.MeasureKilograms(10));

                service.FinishWorkoutSession(workout.Id.Value);
            }
        }


        private static void BuildTrainingPlanTestCase()
        {

            using (GymContext context = new GymContext())
            {
                uint weekProgressiveNumber = 0;
                uint excerciseId;
                uint wunitProgressiveNumber;

                TestServiceLayer service = new TestServiceLayer(context);

                UserRoot owner = context.Users.Single(x => x.Id == 2);

                TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan("Test plan", true, owner.Id);
                plan.LinkTargetProficiency(2);
                plan.TagAs(1);
                plan.TagAs(2);

                context.Update(plan);
                context.SaveChanges();


                WorkoutTemplateRoot wo = service.PlanWorkout(plan.Id.Value, weekProgressiveNumber);
                service.ModifyWorkoutAttributes(wo.Id.Value, "WO1", WeekdayEnum.Monday);

                // Work Unit 0
                excerciseId = 1;
                wo.PlanTransientExcercise(excerciseId, new List<WorkingSetTemplateEntity>(), ownerNoteId: 1);
                context.Update(wo);
                context.SaveChanges();

                wunitProgressiveNumber = 0;
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8));
                service.AddLinkedSet(wo.Id.Value, wunitProgressiveNumber, 3, WSRepetitionsValue.TrackRepetitionSerie(8));
                service.AddLinkedSet(wo.Id.Value, wunitProgressiveNumber, 3, WSRepetitionsValue.TrackRepetitionSerie(8));

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8));
                service.AddLinkedSet(wo.Id.Value, wunitProgressiveNumber, 3, WSRepetitionsValue.TrackRepetitionSerie(8));
                service.AddLinkedSet(wo.Id.Value, wunitProgressiveNumber, 3, WSRepetitionsValue.TrackRepetitionSerie(8));

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8));
                service.AddLinkedSet(wo.Id.Value, wunitProgressiveNumber, 3, WSRepetitionsValue.TrackRepetitionSerie(8));
                service.AddLinkedSet(wo.Id.Value, wunitProgressiveNumber, 3, WSRepetitionsValue.TrackRepetitionSerie(8));

                // Work Unit 1
                excerciseId = 10;
                wo.PlanTransientExcercise(excerciseId, new List<WorkingSetTemplateEntity>());
                context.Update(wo);
                context.SaveChanges();

                wunitProgressiveNumber++;
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(12), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(12), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(12), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(12), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));

                // Work Unit 2
                excerciseId = 2;
                wo.PlanTransientExcercise(excerciseId, new List<WorkingSetTemplateEntity>());
                context.Update(wo);
                context.SaveChanges();

                wunitProgressiveNumber++;
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(6), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(6), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(6), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(6), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));

                // Work Unit 3 - Empty
                wo.PlanTransientExcercise(excerciseId, new List<WorkingSetTemplateEntity>());
                context.Update(wo);
                context.SaveChanges();

                // WO2
                WorkoutTemplateRoot wo1 = service.PlanWorkout(plan.Id.Value, weekProgressiveNumber);
                service.ModifyWorkoutAttributes(wo1.Id.Value, "WO2");

                // Work Unit 0
                excerciseId = 1;
                wo1.PlanTransientExcercise(excerciseId, null);
                context.Update(wo1);
                context.SaveChanges();

                wunitProgressiveNumber = 0;
                service.AddWorkingSet(wo1.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(20));
                service.AddWorkingSet(wo1.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(15));
                service.AddWorkingSet(wo1.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10)
                    , intensityTechniques: new List<uint?>() { 5 });

                // Work Unit 1 - SS with WU0
                excerciseId = 2;
                service.AddLinkedExcercise(wo1.Id.Value, excerciseId, 1);

                wunitProgressiveNumber = 1;
                service.AddWorkingSet(wo1.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(20));
                service.AddWorkingSet(wo1.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(15));
                service.AddWorkingSet(wo1.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10)
                    , intensityTechniques: new List<uint?>() { 5 });
            }
        }

        private static void SeedData()
        {
            using (GymContext context = new GymContext())
            {
                TestDataSeed.UserDataSeed();
                TestDataSeed.WorkUnitNoteDataSeed();
                TestDataSeed.HashtagDataSeed();
                TestDataSeed.ProficiencyDataSeed();
                TestDataSeed.ExcerciseDataSeed();
                TestDataSeed.IntensityTechniqueDataSeed();
            }
        }




    }
}
