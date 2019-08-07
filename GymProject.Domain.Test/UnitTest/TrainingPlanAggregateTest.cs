using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using Xunit;
using GymProject.Domain.Base;
using GymProject.Domain.Test.Util;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingPlanAggregateTest
    {


        [Fact]
        public void WorkingSetTemplateFail()
        {
            uint pNum = 0;
            WSRepetitionValue reps1 = null;
            WSRepetitionValue reps2 = WSRepetitionValue.TrackNotSetRepetitions();
            WSRepetitionValue reps3 = WSRepetitionValue.TrackNotSetTime();
            WSRepetitionValue amrap = WSRepetitionValue.TrackAMRAP();
            WSRepetitionValue timed0 = WSRepetitionValue.TrackTimedSerie(0);
            WSRepetitionValue validReps = WSRepetitionValue.TrackRepetitionSerie(10);

            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, reps1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, reps2));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, reps3));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, amrap));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, timed0));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, amrap, effort: TrainingEffortValue.AsRPE(10)));

            // Duplicate Intensity Techniques
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(10), new IdType(1) }));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(10), new IdType(10), new IdType(1) }));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(10), new IdType(1), new IdType(10) }));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(1) }));
        }


        [Fact]
        public void WorkingSetTemplateIntensityEffortAdd()
        {
            uint pNum = 0;
            uint restValue = 90, newRestValue = 60;
            uint repsVal10 = 10;

            RestPeriodValue rest = RestPeriodValue.SetRestSeconds(restValue);
            RestPeriodValue newRest = RestPeriodValue.SetRestSeconds(newRestValue);
            TUTValue tut = TUTValue.PlanTUT("1010");
            TUTValue newTut = TUTValue.PlanTUT("1030");

            TrainingEffortValue effort1 = TrainingEffortValue.AsIntensityPerc(67f);
            WSRepetitionValue amrap = WSRepetitionValue.TrackAMRAP();
            WSRepetitionValue reps10 = WSRepetitionValue.TrackRepetitionSerie(repsVal10);

            // WS
            WorkingSetTemplate amrapSet = WorkingSetTemplate.AddWorkingSet(pNum, amrap, rest: rest, effort: effort1, tempo: tut);

            Assert.Equal(pNum, amrapSet.ProgressiveNumber);
            Assert.Equal(rest, amrapSet.Rest);
            Assert.Equal(effort1, amrapSet.Effort);
            Assert.Equal(tut, amrapSet.Tempo);
            Assert.Equal(WSRepetitionValue.AMRAPValue, amrapSet.Repetitions.Value);
            Assert.Empty(amrapSet.IntensityTechniqueIds);
            Assert.Equal(0, amrapSet.ToWorkload().Value);

            // Check functions
            Assert.True(amrapSet.IsEffortVsRepetitionsConsistent());
            Assert.Equal(14, amrapSet.ToRepetitions());
            Assert.Equal(14 * 2, amrapSet.ToSecondsUnderTension());
            Assert.Equal(14 * 2 + (int)restValue, amrapSet.ToTotalSeconds());

            // Change it
            amrapSet.ChangeRestPeriod(newRest);
            amrapSet.ChangeLiftingTempo(newTut);

            Assert.Equal(14, amrapSet.ToRepetitions());
            Assert.Equal(14 * 4, amrapSet.ToSecondsUnderTension());
            Assert.Equal(14 * 4 + (int)newRestValue, amrapSet.ToTotalSeconds());

            // Change it
            amrapSet.ChangeRepetitions(reps10);

            Assert.Equal((int)repsVal10, amrapSet.ToRepetitions());
            Assert.Equal((int)repsVal10 * 4, amrapSet.ToSecondsUnderTension());
            Assert.Equal(repsVal10 * 4 + (int)newRestValue, amrapSet.ToTotalSeconds());
            Assert.Equal(0, amrapSet.ToWorkload().Value);

            // Change it
            amrapSet.ToNewEffortType(TrainingEffortTypeEnum.RPE);

            Assert.Equal(5.5f, amrapSet.Effort.Value);     // 10 reps @ 67% -> 15RM

            // Intensity Techniques
            IdType itId1 = new IdType(1);
            IdType itId2 = new IdType(2);
            IdType itId10 = new IdType(10);

            amrapSet.AddIntensityTechnique(itId1);
            amrapSet.AddIntensityTechnique(itId2);
            amrapSet.AddIntensityTechnique(itId10);
            amrapSet.RemoveIntensityTechnique(itId2);

            Assert.Equal(2, amrapSet.IntensityTechniqueIds.Count);
            Assert.Contains(itId1, amrapSet.IntensityTechniqueIds);
            Assert.Contains(itId10, amrapSet.IntensityTechniqueIds);
            Assert.DoesNotContain(itId2, amrapSet.IntensityTechniqueIds);
        }


        [Fact]
        public void WorkingSetTemplateIntensityEffortTimedAdd()
        {
            uint pNum = 0;
            float intPerc = 67f;
            uint restValue = 90, newRestValue = 60;
            int reps10 = 10, reps100 = 100;
            uint time10 = (uint)reps10 * 3;    // Defualt tempo = 3TUT
            uint newTime100 = (uint)reps100 * 3;    // Defualt tempo = 3TUT

            RestPeriodValue rest = RestPeriodValue.SetRestSeconds(restValue);
            RestPeriodValue newRest = RestPeriodValue.SetRestSeconds(newRestValue);
            TUTValue newTut = TUTValue.PlanTUT("60x0");

            TrainingEffortValue effort1 = TrainingEffortValue.AsIntensityPerc(intPerc);
            WSRepetitionValue timed1 = WSRepetitionValue.TrackTimedSerie(time10);
            WSRepetitionValue timed2 = WSRepetitionValue.TrackTimedSerie(newTime100);

            // WS
            WorkingSetTemplate timedSet = WorkingSetTemplate.AddWorkingSet(pNum, timed1, rest: rest, effort: effort1, 
                intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(11) });

            Assert.Equal(pNum, timedSet.ProgressiveNumber);
            Assert.Equal(rest, timedSet.Rest);
            Assert.Equal(effort1, timedSet.Effort);
            Assert.Equal(TUTValue.GenericTempo, timedSet.Tempo.TUT);
            Assert.Equal((int)time10, timedSet.Repetitions.Value);
            Assert.Equal(2, timedSet.IntensityTechniqueIds.Count);
            Assert.Contains(new IdType(1), timedSet.IntensityTechniqueIds);
            Assert.Contains(new IdType(11), timedSet.IntensityTechniqueIds);

            Assert.True(timedSet.IsEffortVsRepetitionsConsistent());
            Assert.Equal(reps10, timedSet.ToRepetitions());
            Assert.Equal((int)time10, timedSet.ToSecondsUnderTension());
            Assert.Equal((int)time10 + (int)restValue, timedSet.ToTotalSeconds());

            // Change it
            timedSet.ChangeRestPeriod(newRest);
            timedSet.ChangeLiftingTempo(newTut);

            Assert.Equal(reps10 / 2, timedSet.ToRepetitions());     // TUT has been doubled up
            Assert.Equal((int)time10, timedSet.ToSecondsUnderTension());
            Assert.Equal((int)time10 + (int)newRestValue, timedSet.ToTotalSeconds());

            // Change it
            timedSet.ChangeRepetitions(timed2);

            Assert.Equal(reps100 / 2, timedSet.ToRepetitions());    // TUT has been doubled up
            Assert.Equal((int)newTime100, timedSet.ToSecondsUnderTension());
            Assert.Equal((int)newTime100 + (int)newRestValue, timedSet.ToTotalSeconds());

            // To RM
            timedSet.ChangeRepetitions(timed1);
            timedSet.ToNewEffortType(TrainingEffortTypeEnum.RM);
            Assert.Equal(14, timedSet.Effort.Value);

            // To RPE
            timedSet.ToNewEffortType(TrainingEffortTypeEnum.RPE);
            Assert.Equal(TrainingEffortTypeEnum.MinRPE, timedSet.Effort.Value);     // 5 reps @ 67% -> 14RM

            // To Int % again
            timedSet.ToNewEffortType(TrainingEffortTypeEnum.IntensityPerc);
            StaticUtils.CheckConversions(intPerc, timedSet.Effort.Value);
        }


        [Fact]
        public void WorkingSetTemplateRMAdd()
        {
            float rm = 10;
            uint pNum = 0;
            uint newRestValue = 60;
            uint repsVal10 = 10;

            RestPeriodValue newRest = RestPeriodValue.SetRestSeconds(newRestValue);
            TUTValue tut = TUTValue.PlanTUT("1010");
            TUTValue newTut = TUTValue.PlanTUT("1030");

            TrainingEffortValue effort1 = TrainingEffortValue.AsRM(rm);
            WSRepetitionValue amrap = WSRepetitionValue.TrackAMRAP();
            WSRepetitionValue reps10 = WSRepetitionValue.TrackRepetitionSerie(repsVal10);

            // WS
            WorkingSetTemplate amrapSet = WorkingSetTemplate.AddWorkingSet(pNum, amrap, effort: effort1, tempo: tut,
                intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(100), });

            Assert.Equal(pNum, amrapSet.ProgressiveNumber);
            Assert.Equal(effort1, amrapSet.Effort);
            Assert.Equal(tut, amrapSet.Tempo);
            Assert.Equal(WSRepetitionValue.AMRAPValue, amrapSet.Repetitions.Value);
            Assert.Equal(2, amrapSet.IntensityTechniqueIds.Count);

            // Check functions
            Assert.True(amrapSet.IsEffortVsRepetitionsConsistent());
            Assert.Equal(rm, amrapSet.ToRepetitions());
            Assert.Equal(rm * 2, amrapSet.ToSecondsUnderTension());
            Assert.Equal(rm * 2 + RestPeriodValue.DefaultRestValue, amrapSet.ToTotalSeconds());

            // Change it
            amrapSet.ChangeRestPeriod(newRest);
            amrapSet.ChangeLiftingTempo(newTut);

            Assert.Equal(rm, amrapSet.ToRepetitions());
            Assert.Equal(rm * 4, amrapSet.ToSecondsUnderTension());
            Assert.Equal(rm * 4 + (int)newRestValue, amrapSet.ToTotalSeconds());

            // Change it
            amrapSet.ChangeRepetitions(reps10);

            Assert.Equal((int)repsVal10, amrapSet.ToRepetitions());
            Assert.Equal((int)repsVal10 * 4, amrapSet.ToSecondsUnderTension());
            Assert.Equal(repsVal10 * 4 + (int)newRestValue, amrapSet.ToTotalSeconds());

            // To RPE
            amrapSet.ToNewEffortType(TrainingEffortTypeEnum.RPE);
            Assert.Equal(10, amrapSet.Effort.Value);     // 10 reps @ 10RM

            // To Intensity %
            amrapSet.ToNewEffortType(TrainingEffortTypeEnum.IntensityPerc);
            StaticUtils.CheckConversions(74f, amrapSet.Effort.Value);

            // Intensity Techniques
            IdType itId1 = new IdType(1);   // Already added
            IdType itId2 = new IdType(2);
            IdType itId10 = new IdType(10);

            amrapSet.AddIntensityTechnique(itId1);
            amrapSet.AddIntensityTechnique(itId2);
            amrapSet.AddIntensityTechnique(itId10);
            amrapSet.RemoveIntensityTechnique(itId2);

            Assert.Equal(3, amrapSet.IntensityTechniqueIds.Count);
            Assert.Contains(itId1, amrapSet.IntensityTechniqueIds);
            Assert.Contains(itId10, amrapSet.IntensityTechniqueIds);
            Assert.Contains(new IdType(100), amrapSet.IntensityTechniqueIds);
            Assert.DoesNotContain(itId2, amrapSet.IntensityTechniqueIds);
        }


        [Fact]
        public void WorkingSetWithDefaultProperties()
        {
            float rm = 10;
            uint pNum = 0;
            WSRepetitionValue reps10 = WSRepetitionValue.TrackRepetitionSerie((uint)rm);
            TrainingEffortValue expectedRm = TrainingEffortValue.AsRM(rm + (TrainingEffortTypeEnum.AMRAPAsRPE - TrainingEffortValue.DefaultEffort.Value));

            // WS with some fields missing -> Check defaults
            WorkingSetTemplate reps = WorkingSetTemplate.AddWorkingSet(pNum, reps10);

            Assert.Equal(TrainingEffortValue.DefaultEffort, reps.Effort);
            Assert.Equal(RestPeriodValue.SetRestNotSpecified(), reps.Rest);
            Assert.Equal(TUTValue.SetGenericTUT(), reps.Tempo);

            Assert.Equal(RestPeriodValue.DefaultRestValue + TUTValue.SetGenericTUT().ToSeconds() * reps.Repetitions.Value, reps.ToTotalSeconds());
            Assert.Equal(WeightPlatesValue.MeasureKilograms(0), reps.ToWorkload());

            reps.ToNewEffortType(TrainingEffortTypeEnum.RM);
            Assert.Equal(expectedRm, reps.Effort);
        }


        [Fact]
        public void WorkingSetTemplateRPEAdd()
        {
            float rm = 10, rm7 = 7;
            float rpe = 9.5f;
            uint pNum = 0;
            uint restValue = 5000, newRestValue = 60;

            RestPeriodValue rest = RestPeriodValue.SetRestSeconds(restValue);
            RestPeriodValue newRest = RestPeriodValue.SetRestSeconds(newRestValue);
            TUTValue tut = TUTValue.PlanTUT("1010");
            TUTValue newTut = TUTValue.PlanTUT("1030");

            TrainingEffortValue effort1 = TrainingEffortValue.AsRPE(rpe);
            WSRepetitionValue reps10 = WSRepetitionValue.TrackRepetitionSerie((uint)rm);
            WSRepetitionValue reps7 = WSRepetitionValue.TrackRepetitionSerie((uint)rm7);

            // WS
            WorkingSetTemplate reps = WorkingSetTemplate.AddWorkingSet(pNum, reps10, rest: rest, effort: effort1, tempo: tut,
                intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(100), });

            Assert.Equal(pNum, reps.ProgressiveNumber);
            Assert.Equal(effort1, reps.Effort);
            Assert.Equal(tut, reps.Tempo);
            Assert.Equal(rm, reps.Repetitions.Value);
            Assert.Equal(2, reps.IntensityTechniqueIds.Count);

            // Check functions
            Assert.True(reps.IsEffortVsRepetitionsConsistent());
            Assert.Equal(rm, reps.ToRepetitions());
            Assert.Equal(rm * 2, reps.ToSecondsUnderTension());
            Assert.Equal(rm * 2 + restValue, reps.ToTotalSeconds());

            // Change it
            reps.ChangeRestPeriod(newRest);

            Assert.Equal(rm, reps.ToRepetitions());
            Assert.Equal(rm * 2, reps.ToSecondsUnderTension());
            Assert.Equal(rm * 2 + (int)newRestValue, reps.ToTotalSeconds());

            // Change it
            reps.ChangeRepetitions(reps7);
            reps.ChangeLiftingTempo(newTut);

            Assert.Equal((int)rm7, reps.ToRepetitions());
            Assert.Equal((int)rm7 * 4, reps.ToSecondsUnderTension());
            Assert.Equal(rm7 * 4 + (int)newRestValue, reps.ToTotalSeconds());

            // To Intensity %
            reps.ToNewEffortType(TrainingEffortTypeEnum.IntensityPerc);
            StaticUtils.CheckConversions(82f, reps.Effort.Value);

            // To RM
            reps.ToNewEffortType(TrainingEffortTypeEnum.RM);
            StaticUtils.CheckConversions(rm7, reps.Effort.Value);

            // To RPE again
            reps.ToNewEffortType(TrainingEffortTypeEnum.RPE);
            Assert.Equal(10, reps.Effort.Value);     // 10 reps @ 10RM

            // Intensity Techniques
            IdType itId1 = new IdType(1);   // Already added
            IdType itId2 = new IdType(2);
            IdType itId10 = new IdType(10);

            reps.AddIntensityTechnique(itId1);
            reps.AddIntensityTechnique(itId2);
            reps.AddIntensityTechnique(itId10);
            reps.RemoveIntensityTechnique(itId2);

            Assert.Equal(3, reps.IntensityTechniqueIds.Count);
            Assert.Contains(itId1, reps.IntensityTechniqueIds);
            Assert.Contains(itId10, reps.IntensityTechniqueIds);
            Assert.Contains(new IdType(100), reps.IntensityTechniqueIds);
            Assert.DoesNotContain(itId2, reps.IntensityTechniqueIds);
        }


        [Fact]
        public void WorkUnitCreateFail()
        {
            List<WorkingSetTemplate> fake1 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(2, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake2 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(4, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake3 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(2, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake4 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake5 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(2, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake6 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(2, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(10)),
                null,
            };

            List<WorkingSetTemplate> valid = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), new List<WorkingSetTemplate>()));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), fake1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), fake2));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), fake3));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), fake4));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), fake5));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(1), fake6));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(0, null, valid));
            Assert.Throws<ArgumentException>(() => WorkUnitTemplate.PlanWorkUnit(0, new IdType(0), valid));
        }


        [Fact]
        public void WorkUnitCreate()
        {
            uint reps10 = 10, reps8 = 8, reps6 = 6;
            uint progn1 = 0, progn2 = 1;

            IdType excerciseId = new IdType(19);
            IdType excerciseId2 = new IdType(29);

            RestPeriodValue rest = RestPeriodValue.SetRestSeconds(45);
            RestPeriodValue fullRest = RestPeriodValue.SetFullRecoveryRest();
            TrainingEffortValue effort = TrainingEffortValue.AsRM(12);
            TrainingEffortValue effort2 = TrainingEffortValue.AsRM(8);
            TUTValue tut9 = TUTValue.PlanTUT("3030");
            TUTValue tut4 = TUTValue.PlanTUT("1030");


            List<WorkingSetTemplate> wsFull = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(reps10),rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(reps10),rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(2, WSRepetitionValue.TrackRepetitionSerie(reps10), rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(reps8), rest, effort, tut4),
                WorkingSetTemplate.AddWorkingSet(4, WSRepetitionValue.TrackRepetitionSerie(reps8), rest, effort, tut4),
            };

            List<WorkingSetTemplate> wsShuffled = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(1, WSRepetitionValue.TrackRepetitionSerie(reps10), fullRest),
                WorkingSetTemplate.AddWorkingSet(3, WSRepetitionValue.TrackRepetitionSerie(reps10), fullRest),
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(reps10), fullRest),
                WorkingSetTemplate.AddWorkingSet(2, WSRepetitionValue.TrackRepetitionSerie(reps10)),
            };

            List<WorkingSetTemplate> oneWs = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(0, WSRepetitionValue.TrackRepetitionSerie(reps6), effort: effort2),
            };

            PersonalNoteValue note = PersonalNoteValue.Write("My Note.");

            WorkUnitTemplate wu1 = WorkUnitTemplate.PlanWorkUnit(progn1, excerciseId, wsFull, note);
            WorkUnitTemplate wu2 = WorkUnitTemplate.PlanWorkUnit(progn2, excerciseId2, wsShuffled);
            WorkUnitTemplate wu3 = WorkUnitTemplate.PlanWorkUnit(progn1, excerciseId, oneWs);

            Assert.Equal(excerciseId, wu1.ExcerciseId);
            Assert.Equal(excerciseId2, wu2.ExcerciseId);
            Assert.Equal(excerciseId, wu3.ExcerciseId);

            Assert.Equal(progn1, wu1.ProgressiveNumber);
            Assert.Equal(progn2, wu2.ProgressiveNumber);
            Assert.Equal(progn1, wu3.ProgressiveNumber);

            Assert.Equal(wsFull.Count, wu1.WorkingSets.Count);
            Assert.Equal(wsShuffled.Count, wu2.WorkingSets.Count);
            Assert.Equal(oneWs.Count, wu3.WorkingSets.Count);

            Assert.Equal(note, wu1.OwnerNote);
            Assert.Null(wu2.OwnerNote);
            Assert.Null(wu3.OwnerNote);

            // Volume check
            TrainingVolumeParametersValue vol1 = TrainingVolumeParametersValue.SetTrainingVolume(3 * reps10 + 2 * reps8, (uint)wsFull.Count, null);
            TrainingVolumeParametersValue vol2 = TrainingVolumeParametersValue.SetTrainingVolume(4 * reps10, (uint)wsShuffled.Count, null);
            TrainingVolumeParametersValue vol3 = TrainingVolumeParametersValue.SetTrainingVolume(6, (uint)oneWs.Count, null);

            Assert.Equal(vol1, wu1.TrainingVolume);
            Assert.Equal(vol2, wu2.TrainingVolume);
            Assert.Equal(vol3, wu3.TrainingVolume);

            // Density Check
            TrainingDensityParametersValue dens1 = TrainingDensityParametersValue.SetTrainingDensity
                (3 * (int)reps10 * tut9.ToSeconds() + 2 * (int)reps8 * tut4.ToSeconds(), rest.Value * wsFull.Count, wsFull.Count);

            TrainingDensityParametersValue dens2 = TrainingDensityParametersValue.SetTrainingDensity
                (wsShuffled.Count * (int)reps10 * TUTValue.SetGenericTUT().ToSeconds(), 3 * fullRest.Value + RestPeriodValue.SetRestNotSpecified().Value, wsFull.Count);

            TrainingDensityParametersValue dens3 = TrainingDensityParametersValue.SetTrainingDensity
                (TUTValue.SetGenericTUT().ToSeconds() * (int)reps6 * oneWs.Count, RestPeriodValue.SetRestNotSpecified().Value * oneWs.Count, wsFull.Count);

            Assert.Equal(dens1, wu1.TrainingDensity);
            Assert.Equal(dens2, wu2.TrainingDensity);
            Assert.Equal(dens3, wu3.TrainingDensity);

            throw new NotImplementedException();

            //// Intensity Check [%]
            //TrainingEffortValue intEffort1 = TrainingEffortValue.AsIntensityPerc((71 + 71 + 71 + 71 + 71) / 5);
            //TrainingEffortValue intEffort2 = TrainingEffortValue.DefaultEffort.ToIntensityPercentage(
            //    WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions()));

            //TrainingEffortValue intEffort3 = TrainingEffortValue.AsIntensityPerc(79);

            //TrainingIntensityParametersValue int1 = TrainingIntensityParametersValue.SetTrainingIntensity(intEffort1, wsFull.Count);
            //TrainingIntensityParametersValue int2 = TrainingIntensityParametersValue.SetTrainingIntensity(intEffort2, wsShuffled.Count);
            //TrainingIntensityParametersValue int3 = TrainingIntensityParametersValue.SetTrainingIntensity(intEffort3, oneWs.Count);

            //StaticUtils.CheckConversions(int1.AverageIntensity.Value, wu1.TrainingIntensity.AverageIntensity.Value);
            //StaticUtils.CheckConversions(int2.AverageIntensity.Value, wu2.TrainingIntensity.AverageIntensity.Value);
            //StaticUtils.CheckConversions(int3.AverageIntensity.Value, wu3.TrainingIntensity.AverageIntensity.Value);

            //// Intensity Check [RM]
            //TrainingEffortValue rmEffort1 = TrainingEffortValue.AsRM((5* effort.Value) / 5);
            //TrainingEffortValue rmEffort2 = TrainingEffortValue.DefaultEffort.ToRm(
            //    WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions()));
            //TrainingEffortValue rmEffort3 = TrainingEffortValue.AsRM(effort2.Value);

            //TrainingIntensityParametersValue rm1 = TrainingIntensityParametersValue.SetTrainingIntensity(rmEffort1, wsFull.Count);
            //TrainingIntensityParametersValue rm2 = TrainingIntensityParametersValue.SetTrainingIntensity(rmEffort2, wsShuffled.Count);
            //TrainingIntensityParametersValue rm3 = TrainingIntensityParametersValue.SetTrainingIntensity(rmEffort3, oneWs.Count);

            //Assert.Equal(rm1.AverageIntensity, wu1.TrainingIntensity.AverageIntensity.ToRm());
            //Assert.Equal(rm2.AverageIntensity, wu2.TrainingIntensity.AverageIntensity.ToRm());
            //Assert.Equal(rm3.AverageIntensity, wu3.TrainingIntensity.AverageIntensity.ToRm());

            //// Intensity Check [RPE]
            //TrainingEffortValue rpeffort1 = TrainingEffortValue.AsRPE((8 + 8 + 8 + 6 + 6) / 5);
            //TrainingEffortValue rpeffort2 = TrainingEffortValue.DefaultEffort;
            //TrainingEffortValue rpeffort3 = TrainingEffortValue.AsRPE(8);

            //TrainingIntensityParametersValue rpe1 = TrainingIntensityParametersValue.SetTrainingIntensity(rpeffort1, wsFull.Count);
            //TrainingIntensityParametersValue rpe2 = TrainingIntensityParametersValue.SetTrainingIntensity(rpeffort2, wsShuffled.Count);
            //TrainingIntensityParametersValue rpe3 = TrainingIntensityParametersValue.SetTrainingIntensity(rpeffort3, oneWs.Count);

            //Assert.Equal(rpe1.AverageIntensity, wu1.TrainingIntensity.AverageIntensity.ToRPE(WSRepetitionValue.TrackRepetitionSerie((uint)wu1.TrainingVolume.GetAverageRepetitions())));
            //Assert.Equal(rpe2.AverageIntensity, wu2.TrainingIntensity.AverageIntensity.ToRPE(WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions())));
            //Assert.Equal(rpe3.AverageIntensity, wu3.TrainingIntensity.AverageIntensity.ToRPE(WSRepetitionValue.TrackRepetitionSerie((uint)wu3.TrainingVolume.GetAverageRepetitions())));
        }


        [Fact]
        public void WorkUnitChange()
        {
            uint reps10 = 10, reps8 = 8;

            IdType excerciseId = new IdType(19);
            RestPeriodValue rest = RestPeriodValue.SetRestSeconds(45);
            TrainingEffortValue effort = TrainingEffortValue.AsRM(12);
            TUTValue tut9 = TUTValue.PlanTUT("3030");
            TUTValue tut4 = TUTValue.PlanTUT("1030");

            IdType excercise = new IdType(10);
            IdType newExcercise = new IdType(1);

            throw new NotImplementedException();
        }
        }
}
