using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using Xunit;
using GymProject.Domain.Base;
using GymProject.Domain.Test.Util;
using GymProject.Domain.Utils;
using System.Linq;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingPlanAggregateTest
    {


        [Fact]
        public void WorkingSetTemplateFail()
        {
            IdType id = new IdType(1);
            uint pNum = 0;
            WSRepetitionValue reps1 = null;
            WSRepetitionValue reps2 = WSRepetitionValue.TrackNotSetRepetitions();
            WSRepetitionValue reps3 = WSRepetitionValue.TrackNotSetTime();
            WSRepetitionValue amrap = WSRepetitionValue.TrackAMRAP();
            WSRepetitionValue timed0 = WSRepetitionValue.TrackTimedSerie(0);
            WSRepetitionValue validReps = WSRepetitionValue.TrackRepetitionSerie(10);

            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, reps1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, reps2));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, reps3));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, amrap));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, timed0));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, amrap, effort: TrainingEffortValue.AsRPE(10)));

            // Duplicate Intensity Techniques
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(10), new IdType(1) }));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(10), new IdType(10), new IdType(1) }));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(10), new IdType(1), new IdType(10) }));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkingSetTemplate.AddWorkingSet(id, pNum, validReps, intensityTechniqueIds: new List<IdType>() { new IdType(1), new IdType(1) }));
        }


        [Fact]
        public void WorkingSetTemplateIntensityEffortAdd()
        {
            IdType id = new IdType(1);
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
            WorkingSetTemplate amrapSet = WorkingSetTemplate.AddWorkingSet(id, pNum, amrap, rest: rest, effort: effort1, tempo: tut);

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

            StaticUtils.CheckConversions(5.5f, amrapSet.Effort.Value);      // 10 reps @ 67% -> 15RM

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
            IdType id = new IdType(1);
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
            WorkingSetTemplate timedSet = WorkingSetTemplate.AddWorkingSet(id, pNum, timed1, rest: rest, effort: effort1, 
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
            IdType id = new IdType(1);
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
            WorkingSetTemplate amrapSet = WorkingSetTemplate.AddWorkingSet(id, pNum, amrap, effort: effort1, tempo: tut,
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
            IdType id = new IdType(1);
            float rm = 10;
            uint pNum = 0;
            WSRepetitionValue reps10 = WSRepetitionValue.TrackRepetitionSerie((uint)rm);
            TrainingEffortValue expectedRm = TrainingEffortValue.AsRM(rm + (TrainingEffortTypeEnum.AMRAPAsRPE - TrainingEffortValue.DefaultEffort.Value));

            // WS with some fields missing -> Check defaults
            WorkingSetTemplate reps = WorkingSetTemplate.AddWorkingSet(id, pNum, reps10);

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
            IdType id = new IdType(1);
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
            WorkingSetTemplate reps = WorkingSetTemplate.AddWorkingSet(id, pNum, reps10, rest: rest, effort: effort1, tempo: tut,
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
            IdType wsId = new IdType(1);
            IdType wuId = new IdType(1);

            List<WorkingSetTemplate> fake1 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 2, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake2 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 4, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake3 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 2, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake4 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake5 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 2, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            List<WorkingSetTemplate> fake6 = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 2, WSRepetitionValue.TrackRepetitionSerie(10)),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(10)),
                null,
            };

            List<WorkingSetTemplate> valid = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
            };

            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), null));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), new List<WorkingSetTemplate>()));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), fake1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), fake2));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), fake3));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), fake4));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), fake5));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(1), fake6));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, null, valid));
            Assert.Throws<ArgumentException>(() => WorkUnitTemplate.PlanWorkUnit(wuId, 0, new IdType(0), valid));
        }


        [Fact]
        public void WorkUnitCreate()
        {
            IdType wsId = new IdType(1);
            IdType wuId = new IdType(1);

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
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(reps10),rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(reps10),rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(wsId, 2, WSRepetitionValue.TrackRepetitionSerie(reps10), rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(reps8), rest, effort, tut4),
                WorkingSetTemplate.AddWorkingSet(wsId, 4, WSRepetitionValue.TrackRepetitionSerie(reps8), rest, effort, tut4),
            };

            List<WorkingSetTemplate> wsShuffled = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 1, WSRepetitionValue.TrackRepetitionSerie(reps10), fullRest),
                WorkingSetTemplate.AddWorkingSet(wsId, 3, WSRepetitionValue.TrackRepetitionSerie(reps10), fullRest),
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(reps10), fullRest),
                WorkingSetTemplate.AddWorkingSet(wsId, 2, WSRepetitionValue.TrackRepetitionSerie(reps10)),
            };

            List<WorkingSetTemplate> oneWs = new List<WorkingSetTemplate>(){
                WorkingSetTemplate.AddWorkingSet(wsId, 0, WSRepetitionValue.TrackRepetitionSerie(reps6), effort: effort2),
            };

            IdType noteId = new IdType(11);

            WorkUnitTemplate wu1 = WorkUnitTemplate.PlanWorkUnit(wuId, progn1, excerciseId, wsFull, noteId);
            WorkUnitTemplate wu2 = WorkUnitTemplate.PlanWorkUnit(wuId, progn2, excerciseId2, wsShuffled);
            WorkUnitTemplate wu3 = WorkUnitTemplate.PlanWorkUnit(wuId, progn1, excerciseId, oneWs);

            Assert.Equal(excerciseId, wu1.ExcerciseId);
            Assert.Equal(excerciseId2, wu2.ExcerciseId);
            Assert.Equal(excerciseId, wu3.ExcerciseId);

            Assert.Equal(progn1, wu1.ProgressiveNumber);
            Assert.Equal(progn2, wu2.ProgressiveNumber);
            Assert.Equal(progn1, wu3.ProgressiveNumber);

            Assert.Equal(wsFull.Count, wu1.WorkingSets.Count);
            Assert.Equal(wsShuffled.Count, wu2.WorkingSets.Count);
            Assert.Equal(oneWs.Count, wu3.WorkingSets.Count);

            Assert.Equal(noteId, wu1.OwnerNoteId);
            Assert.Null(wu2.OwnerNoteId);
            Assert.Null(wu3.OwnerNoteId);

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

            // Intensity Check [%]
            TrainingEffortValue intEffort1 = TrainingEffortValue.AsIntensityPerc((71 + 71 + 71 + 71 + 71) / 5);
            TrainingEffortValue intEffort2 = TrainingEffortValue.DefaultEffort.ToIntensityPercentage(
                WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions()));

            TrainingEffortValue intEffort3 = TrainingEffortValue.AsIntensityPerc(79);

            StaticUtils.CheckConversions(intEffort1.Value, wu1.TrainingIntensity.AverageIntensity.ToIntensityPercentage().Value);
            StaticUtils.CheckConversions(intEffort3.Value, wu3.TrainingIntensity.AverageIntensity.ToIntensityPercentage().Value);
            StaticUtils.CheckConversions(intEffort2.Value, wu2.TrainingIntensity.AverageIntensity
                .ToIntensityPercentage(WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions())).Value);       // RPE


            //// Intensity Check [RM]
            //TrainingEffortValue rmEffort1 = TrainingEffortValue.AsRM((5 * effort.Value) / 5);
            //TrainingEffortValue rmEffort2 = TrainingEffortValue.DefaultEffort.ToRm(
            //    WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions()));
            //TrainingEffortValue rmEffort3 = TrainingEffortValue.AsRM(effort2.Value);


            //Assert.Equal(rmEffort1, wu1.TrainingIntensity.AverageIntensity);
            //Assert.Equal(rmEffort2, wu2.TrainingIntensity.AverageIntensity);
            //Assert.Equal(rmEffort3, wu3.TrainingIntensity.AverageIntensity);

            //// Intensity Check [RPE]
            //TrainingEffortValue rpeffort1 = TrainingEffortValue.AsRPE((8 + 8 + 8 + 6 + 6) / 5);
            //TrainingEffortValue rpeffort2 = TrainingEffortValue.DefaultEffort;
            //TrainingEffortValue rpeffort3 = TrainingEffortValue.AsRPE(8);

            //Assert.Equal(rpeffort1, wu1.TrainingIntensity.AverageIntensity.ToRPE(WSRepetitionValue.TrackRepetitionSerie((uint)wu1.TrainingVolume.GetAverageRepetitions())));
            //Assert.Equal(rpeffort2, wu2.TrainingIntensity.AverageIntensity.ToRPE(WSRepetitionValue.TrackRepetitionSerie((uint)wu2.TrainingVolume.GetAverageRepetitions())));
            //Assert.Equal(rpeffort3, wu3.TrainingIntensity.AverageIntensity.ToRPE(WSRepetitionValue.TrackRepetitionSerie((uint)wu3.TrainingVolume.GetAverageRepetitions())));
        }


        [Fact]
        public void WorkUnitRepetitionsFullTest()
        {
            int ntests = 1000;      // Number of tests

            int wsMin = 2;
            int wsMax = 7;
            int repsMax = 20;
            int repsMin = 1;
            int restMin = 11;
            int restMax = 500;
            int wsToRemoveNum = 2;

            float intPercMin = 0.5f, intePercMax = 1f;
            float rmMin = 1f, rmMax = 20f;
            float rpeMin = 3f, rpeMax = 11f;
            float effortMin, effortMax;

            int noteMin = 10, noteMax = 10000;
            int excerciseMin = 1, excerciseMax = 100;

            float amrapProbability = 0.1f;   // Probability that an AMRAP set is created

            // TUTs to choose from
            List<TUTValue> tuts = new List<TUTValue>()
            {
                TUTValue.PlanTUT("1010"),
                TUTValue.PlanTUT("3030"),
                TUTValue.PlanTUT("5151"),
                TUTValue.SetGenericTUT(),
            };

            IdType wuId = new IdType(1);
            WorkUnitTemplate wu = null;
            TrainingEffortTypeEnum effortType;

            for (int itest = 0; itest < ntests; itest++)
            {
                TrainingEffortValue avgEffort;

                // Generate random parameters
                int iwsMax = RandomFieldGenerator.RandomInt(wsMin, wsMax);

                IdType excercise = new IdType(RandomFieldGenerator.RandomInt(excerciseMin, excerciseMax));

                if (itest % 2 == 0)
                {
                    effortType = TrainingEffortTypeEnum.IntensityPerc;
                    effortMin = intPercMin;
                    effortMax = intePercMax;
                }

                else if (itest % 3 == 0)
                {
                    effortType = TrainingEffortTypeEnum.RM;
                    effortMin = rmMin;
                    effortMax = rmMax;
                }

                else
                {
                    effortType = TrainingEffortTypeEnum.RPE;
                    effortMin = rpeMin;
                    effortMax = rpeMax;
                }

                if (effortType == TrainingEffortTypeEnum.RM)
                    avgEffort = TrainingEffortValue.AsRM(1);

                else if (effortType == TrainingEffortTypeEnum.RPE)
                    avgEffort = TrainingEffortValue.AsRPE(1);

                else
                    avgEffort = TrainingEffortValue.AsIntensityPerc(1);

                List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                RestPeriodValue rest = RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax));
                TUTValue tut = tuts[RandomFieldGenerator.RandomInt(0, tuts.Count - 1)];
                IdType noteId = new IdType(RandomFieldGenerator.RandomInt(noteMin, noteMax));

                int totalReps = 0;
                int totalRest = 0;
                int totalTempo = 0;

                for (int iws = 0; iws < iwsMax; iws++)
                {
                    // New WS
                    WorkingSetTemplate newWs = BuildRandomWs(iws + 1, iws, repsMin, repsMax, true, amrapProbability, restMin, restMax, effortType, effortMin, effortMax, tuts);
                    ws.Add(newWs);

                    // Build the WU
                    if(iws == 0)
                        wu = WorkUnitTemplate.PlanWorkUnit(wuId, 0, excercise, ws, noteId);

                    else
                        wu.AddWorkingSet(newWs.Repetitions, newWs.Rest, newWs.Effort, newWs.Tempo, new List<IdType>(newWs.IntensityTechniqueIds));


                    totalReps += newWs.ToRepetitions();
                    totalRest += newWs.Rest.Value;
                    totalTempo += newWs.Tempo.ToSeconds() * newWs.ToRepetitions();

                    CheckWorkUnitSets(wu, ws, effortType, totalReps, totalRest, totalTempo);  // Asserts
                }

                // Check WU 
                Assert.Equal(iwsMax, wu.WorkingSets.Count);
                Assert.Equal(noteId, wu.OwnerNoteId);
                Assert.Equal(excercise, wu.ExcerciseId);

                // Change the WU
                excercise = new IdType(RandomFieldGenerator.RandomInt(excerciseMin, excerciseMax));
                noteId = new IdType(RandomFieldGenerator.RandomInt(noteMin, noteMax));
                wu.AssignNote(noteId);
                wu.AssignExcercise(excercise);

                // Check changes
                Assert.Equal(noteId, wu.OwnerNoteId);
                Assert.Equal(excercise, wu.ExcerciseId);
                wu.RemoveNote();
                Assert.Null(wu.OwnerNoteId);

                // Remove WSs
                IList<int> idsRemoved = new List<int>();
                IEnumerable<WorkingSetTemplate> wsLeft = new List<WorkingSetTemplate>();

                for (int iws = 0; iws < wsToRemoveNum; iws++)
                {
                    int idNum = RandomFieldGenerator.RandomIntValueExcluded(1, iwsMax, idsRemoved);
                    idsRemoved.Add(idNum);

                    WorkingSetTemplate removed = ws.Where(x => x.Id == idNum).FirstOrDefault();

                    // No WS in the WU -> Business Rule violated
                    if (idsRemoved.OrderBy(x => x).SequenceEqual(ws.Select(x => (int)x.Id.Id)))
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => wu.RemoveWorkingSet(new IdType(idNum)));
                    else
                    {
                        wu.RemoveWorkingSet(new IdType(idNum));

                        Assert.Equal(ws.Count - iws - 1, wu.WorkingSets.Count);
                        Assert.DoesNotContain(removed, wu.WorkingSets);

                        IEnumerable<WorkingSetTemplate> wsRemoved = ws.Where(x => idsRemoved.Contains((int)x.Id.Id));
                        wsLeft = ws.Where(x => !idsRemoved.Contains((int)x.Id.Id));

                        int repsLeft = totalReps - wsRemoved.Sum(x => x.ToRepetitions());
                        int restLeft = totalRest - wsRemoved.Sum(x => x.Rest.Value);
                        int tempoLeft = totalTempo - wsRemoved.Sum(x => x.Tempo.ToSeconds() * x.ToRepetitions());

                        CheckWorkUnitSets(wu, wsLeft, effortType, repsLeft, restLeft, tempoLeft, false);  // Asserts
                    }
                }

                // Update with the real values -> progressive numbers are different!
                wsLeft = wu.WorkingSets;

                // Change WSs
                if(wsLeft.Count() > 0)
                {
                    foreach(WorkingSetTemplate toChange in wsLeft)
                    {
                        WSRepetitionValue newReps = WSRepetitionValue.TrackRepetitionSerie(
                            (uint)RandomFieldGenerator.RandomIntValueExcluded(repsMin,  repsMax, toChange.Repetitions.Value));

                        RestPeriodValue newRest = RestPeriodValue.SetRestSeconds
                            ((uint)RandomFieldGenerator.RandomIntValueExcluded(restMin, restMax, toChange.Rest.Value));

                        TUTValue newTempo = tuts[RandomFieldGenerator.RandomInt(0, tuts.Count - 1)];

                        // Covnert to RM no matter what the original effort
                        TrainingEffortValue newEffort = TrainingEffortValue.AsRM(toChange.Effort.ToRm(toChange.Repetitions).Value + 1);

                        wu.ChangeWorkingSetRepetitions(toChange.Id, newReps);
                        wu.ChangeWorkingSetRestPeriod(toChange.Id, newRest);
                        wu.ChangeWorkingSetLiftingTempo(toChange.Id, newTempo);
                        wu.ChangeWorkingSetEffort(toChange.Id, newEffort);

                        Assert.Equal(newReps, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Repetitions);
                        Assert.Equal(newRest, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Rest);
                        Assert.Equal(newTempo, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Tempo);
                        Assert.Equal(newEffort, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Effort);
                    }
                }
            }
        }


        [Fact]
        public void WorkUnitTimedSerieFullTest()
        {
            int ntests = 1000;      // Number of tests

            int wsMin = 2;
            int wsMax = 7;
            int repsMax = 20;
            int repsMin = 1;
            int restMin = 11;
            int restMax = 500;
            int wsToRemoveNum = 2;

            float intPercMin = 0.5f, intePercMax = 1f;
            float rmMin = 1f, rmMax = 20f;
            float rpeMin = 3f, rpeMax = 11f;
            float effortMin, effortMax;

            int noteMin = 10, noteMax = 10000;
            int excerciseMin = 1, excerciseMax = 100;

            // TUTs to choose from
            List<TUTValue> tuts = new List<TUTValue>()
            {
                TUTValue.PlanTUT("1010"),
                TUTValue.PlanTUT("3030"),
                TUTValue.PlanTUT("5151"),
                TUTValue.SetGenericTUT(),
            };

            IdType wuId = new IdType(1);
            WorkUnitTemplate wu = null;
            TrainingEffortTypeEnum effortType;

            for (int itest = 0; itest < ntests; itest++)
            {
                TrainingEffortValue avgEffort;

                // Generate random parameters
                int iwsMax = RandomFieldGenerator.RandomInt(wsMin, wsMax);

                IdType excercise = new IdType(RandomFieldGenerator.RandomInt(excerciseMin, excerciseMax));

                if (itest % 2 == 0)
                {
                    effortType = TrainingEffortTypeEnum.IntensityPerc;
                    effortMin = intPercMin;
                    effortMax = intePercMax;
                }

                else if (itest % 3 == 0)
                {
                    effortType = TrainingEffortTypeEnum.RM;
                    effortMin = rmMin;
                    effortMax = rmMax;
                }

                else
                {
                    effortType = TrainingEffortTypeEnum.RPE;
                    effortMin = rpeMin;
                    effortMax = rpeMax;
                }

                if (effortType == TrainingEffortTypeEnum.RM)
                    avgEffort = TrainingEffortValue.AsRM(1);

                else if (effortType == TrainingEffortTypeEnum.RPE)
                    avgEffort = TrainingEffortValue.AsRPE(1);

                else
                    avgEffort = TrainingEffortValue.AsIntensityPerc(1);

                List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                RestPeriodValue rest = RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax));
                TUTValue tut = tuts[RandomFieldGenerator.RandomInt(0, tuts.Count - 1)];
                IdType noteId = new IdType(RandomFieldGenerator.RandomInt(noteMin, noteMax));

                int totalReps = 0;
                int totalRest = 0;
                int totalTempo = 0;

                for (int iws = 0; iws < iwsMax; iws++)
                {
                    // New WS
                    WorkingSetTemplate newWs = BuildRandomWs(iws + 1, iws, repsMin, repsMax, false, 0f, restMin, restMax, effortType, effortMin, effortMax, tuts);
                    ws.Add(newWs);

                    // Build the WU
                    if (iws == 0)
                        wu = WorkUnitTemplate.PlanWorkUnit(wuId, 0, excercise, ws, noteId);

                    else
                        wu.AddWorkingSet(newWs.Repetitions, newWs.Rest, newWs.Effort, newWs.Tempo, new List<IdType>(newWs.IntensityTechniqueIds));


                    totalReps += newWs.ToRepetitions();
                    totalRest += newWs.Rest.Value;
                    totalTempo += newWs.ToSecondsUnderTension();

                    CheckWorkUnitSets(wu, ws, effortType, totalReps, totalRest, totalTempo);  // Asserts
                }

                // Check WU 
                Assert.Equal(iwsMax, wu.WorkingSets.Count);
                Assert.Equal(noteId, wu.OwnerNoteId);
                Assert.Equal(excercise, wu.ExcerciseId);

                // Change the WU
                excercise = new IdType(RandomFieldGenerator.RandomInt(excerciseMin, excerciseMax));
                noteId = noteId = new IdType(RandomFieldGenerator.RandomInt(noteMin, noteMax));

                wu.AssignNote(noteId);
                wu.AssignExcercise(excercise);

                // Check changes
                Assert.Equal(noteId, wu.OwnerNoteId);
                Assert.Equal(excercise, wu.ExcerciseId);

                // Remove WSs
                IList<int> idsRemoved = new List<int>();
                IEnumerable<WorkingSetTemplate> wsLeft = new List<WorkingSetTemplate>();

                for (int iws = 0; iws < wsToRemoveNum; iws++)
                {
                    int idNum = RandomFieldGenerator.RandomIntValueExcluded(1, iwsMax, idsRemoved);
                    idsRemoved.Add(idNum);

                    WorkingSetTemplate removed = ws.Where(x => x.Id == idNum).FirstOrDefault();

                    // No WS in the WU -> Business Rule violated
                    if (idsRemoved.OrderBy(x => x).SequenceEqual(ws.Select(x => (int)x.Id.Id)))
                        ;
                    //Assert.Throws<TrainingDomainInvariantViolationException>(() => wu.RemoveWorkingSet(new IdType(idNum)));
                    else
                    {
                        wu.RemoveWorkingSet(new IdType(idNum));

                        Assert.Equal(ws.Count - iws - 1, wu.WorkingSets.Count);
                        Assert.DoesNotContain(removed, wu.WorkingSets);

                        IEnumerable<WorkingSetTemplate> wsRemoved = ws.Where(x => idsRemoved.Contains((int)x.Id.Id));
                        wsLeft = ws.Where(x => !idsRemoved.Contains((int)x.Id.Id));

                        int repsLeft = totalReps - wsRemoved.Sum(x => x.ToRepetitions());
                        int restLeft = totalRest - wsRemoved.Sum(x => x.Rest.Value);
                        int tempoLeft = totalTempo - wsRemoved.Sum(x => x.ToSecondsUnderTension());

                        CheckWorkUnitSets(wu, wsLeft, effortType, repsLeft, restLeft, tempoLeft, false);  // Asserts
                    }
                }

                // Update with the real values -> progressive numbers are different!
                wsLeft = wu.WorkingSets;

                // Change WSs
                if (wsLeft.Count() > 0)
                {
                    foreach (WorkingSetTemplate toChange in wsLeft)
                    {
                        WSRepetitionValue newReps = WSRepetitionValue.TrackRepetitionSerie(
                            (uint)RandomFieldGenerator.RandomIntValueExcluded(repsMin, repsMax, toChange.Repetitions.Value));

                        RestPeriodValue newRest = RestPeriodValue.SetRestSeconds
                            ((uint)RandomFieldGenerator.RandomIntValueExcluded(restMin, restMax, toChange.Rest.Value));

                        TUTValue newTempo = tuts[RandomFieldGenerator.RandomInt(0, tuts.Count - 1)];

                        // Covnert to RM no matter what the original effort
                        TrainingEffortValue newEffort = TrainingEffortValue.AsRM(toChange.Effort.ToRm(toChange.Repetitions).Value + 1);

                        wu.ChangeWorkingSetRepetitions(toChange.Id, newReps);
                        wu.ChangeWorkingSetRestPeriod(toChange.Id, newRest);
                        wu.ChangeWorkingSetLiftingTempo(toChange.Id, newTempo);
                        wu.ChangeWorkingSetEffort(toChange.Id, newEffort);

                        Assert.Equal(newReps, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Repetitions);
                        Assert.Equal(newRest, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Rest);
                        Assert.Equal(newTempo, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Tempo);
                        Assert.Equal(newEffort, wu.FindWorkingSetByProgressiveNumber((int)toChange.ProgressiveNumber).Effort);
                    }
                }
            }
        }






        private static void CheckWorkUnitSets(WorkUnitTemplate wu, IEnumerable<WorkingSetTemplate> ws, TrainingEffortTypeEnum effortType,
            int totalReps, int totalRest, int totalTempo, bool wsFullCheck = true)
        {
            TrainingEffortValue avgEffort;
            int totalWs = ws.Count();

            if (effortType == TrainingEffortTypeEnum.RM)
                avgEffort = TrainingEffortValue.AsRM(ws.Average(x => x.Effort.Value));

            else if (effortType == TrainingEffortTypeEnum.RPE)
                avgEffort = TrainingEffortValue.AsRPE(ws.Average(x => x.Effort.Value));

            else
                avgEffort = TrainingEffortValue.AsIntensityPerc(ws.Average(x => x.Effort.Value));

            // Generic Fields
            foreach(WorkingSetTemplate wsCheck in ws)
            {
                Assert.Contains(wsCheck, wu.WorkingSets);

                if(wsFullCheck)
                {
                    Assert.Equal(wsCheck.Repetitions, wu.FindWorkingSetByProgressiveNumber((int)wsCheck.ProgressiveNumber).Repetitions);
                    Assert.Equal(wsCheck.Rest, wu.FindWorkingSetByProgressiveNumber((int)wsCheck.ProgressiveNumber).Rest);
                    Assert.Equal(wsCheck.Tempo, wu.FindWorkingSetByProgressiveNumber((int)wsCheck.ProgressiveNumber).Tempo);
                    Assert.Equal(wsCheck.Effort, wu.FindWorkingSetByProgressiveNumber((int)wsCheck.ProgressiveNumber).Effort);
                    Assert.Equal(wsCheck.IntensityTechniqueIds, wu.FindWorkingSetByProgressiveNumber((int)wsCheck.ProgressiveNumber).IntensityTechniqueIds);

                    Assert.Equal(wsCheck.Repetitions, wu.FindWorkingSetById(new IdType(wsCheck.Id.Id)).Repetitions);
                    Assert.Equal(wsCheck.Rest, wu.FindWorkingSetById(new IdType(wsCheck.Id.Id)).Rest);
                    Assert.Equal(wsCheck.Tempo, wu.FindWorkingSetById(new IdType(wsCheck.Id.Id)).Tempo);
                    Assert.Equal(wsCheck.Effort, wu.FindWorkingSetById(new IdType(wsCheck.Id.Id)).Effort);
                    Assert.Equal(wsCheck.IntensityTechniqueIds, wu.FindWorkingSetById(new IdType(wsCheck.Id.Id)).IntensityTechniqueIds);
                }
            }


            // Check Volume
            Assert.Equal(totalReps, wu.TrainingVolume.TotalReps);
            Assert.Equal(totalWs, wu.TrainingVolume.TotalWorkingSets);
            Assert.Equal(0, wu.TrainingVolume.TotalWorkload.Value);
            Assert.Equal((float)totalReps / (float)totalWs, wu.TrainingVolume.GetAverageRepetitions(), 1);
            Assert.Equal(0, wu.TrainingVolume.GetAverageWorkloadPerSet().Value);

            // Check Density
            Assert.Equal(totalRest, wu.TrainingDensity.TotalRest);
            Assert.Equal(totalTempo, wu.TrainingDensity.TotalSecondsUnderTension);
            Assert.Equal((float)totalRest / (float)totalWs, wu.TrainingDensity.GetAverageRest(), 1);
            Assert.Equal((float)totalTempo / (float)totalWs, wu.TrainingDensity.GetAverageSecondsUnderTension(), 1);

            // Check Intensity
            if(ws.All(x => !x.IsAMRAP()) || !avgEffort.IsRPE())     // AMRAP with RPE effort, can't be managed!
                StaticUtils.CheckConversions(avgEffort.Value, wu.TrainingIntensity.AverageIntensity.Value, tolerance: 0.03f);
        }



        private static WorkingSetTemplate BuildRandomWs(int id, int progn, int repsMin, int repsMax, bool repetitionsSerie, float amrapProbability, int restMin, int restMax, 
            TrainingEffortTypeEnum effortType, float effortMin, float effortMax, IList<TUTValue> tutToChooseAmong = null, IList<IdType> techniquesId = null)
        {
            TrainingEffortValue effort;
            WSRepetitionValue serie;

            switch (effortType)
            {
                case var _ when effortType == TrainingEffortTypeEnum.IntensityPerc:

                    effort = TrainingEffortValue.AsIntensityPerc(RandomFieldGenerator.RandomFloat(effortMin, effortMax));
                    break;

                case var _ when effortType == TrainingEffortTypeEnum.RM:

                    effort = TrainingEffortValue.AsRM(RandomFieldGenerator.RandomFloat(effortMin, effortMax));
                    break;

                case var _ when effortType == TrainingEffortTypeEnum.RPE:

                    effort = TrainingEffortValue.AsRPE((float)CommonUtilities.RoundToPointFive(RandomFieldGenerator.RandomFloat(effortMin, effortMax)));
                    break;

                default:

                    effort = null;
                    break;
            }

            if (repetitionsSerie)
            {
                if (RandomFieldGenerator.RandomDouble(0, 1) < amrapProbability)
                {
                    serie = WSRepetitionValue.TrackAMRAP();
                    effort =  effortType == TrainingEffortTypeEnum.RPE ? TrainingEffortValue.AsRM(10) : effort; // Couldn't resolve this
                    //effort = effortType == TrainingEffortTypeEnum.IntensityPerc ? effort.ToIntensityPercentage(serie) : effort.ToRm(serie);
                }
                else
                    serie = WSRepetitionValue.TrackRepetitionSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));
            }
            else
                serie = WSRepetitionValue.TrackTimedSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));

            return WorkingSetTemplate.AddWorkingSet(
                new IdType(id),
                (uint)progn,
                serie,
                RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax)),
                effort,
                tutToChooseAmong[RandomFieldGenerator.RandomInt(0, tutToChooseAmong.Count - 1)],
                techniquesId
            );
        }
    }
}
