using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using Xunit;
using GymProject.Domain.Base;
using GymProject.Domain.Test.Util;
using System.Linq;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingPlanAggregateTest
    {




        private const int ntests = 500;    // Number of test cycles per Fact




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
                WorkingSetTemplate.AddWorkingSet(wsId + 1, 1, WSRepetitionValue.TrackRepetitionSerie(reps10),rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(wsId + 2, 2, WSRepetitionValue.TrackRepetitionSerie(reps10), rest, effort, tut9),
                WorkingSetTemplate.AddWorkingSet(wsId + 3, 3, WSRepetitionValue.TrackRepetitionSerie(reps8), rest, effort, tut4),
                WorkingSetTemplate.AddWorkingSet(wsId + 4, 4, WSRepetitionValue.TrackRepetitionSerie(reps8), rest, effort, tut4),
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

            List<IdType> wuIntId1 = new List<IdType>() { null };
            List<IdType> wuIntId2 = new List<IdType>() { new IdType(1), new IdType(88), };

            IdType noteId = new IdType(11);

            WorkUnitTemplate wu1 = WorkUnitTemplate.PlanWorkUnit(wuId, progn1, excerciseId, wsFull, null, noteId);
            WorkUnitTemplate wu2 = WorkUnitTemplate.PlanWorkUnit(wuId, progn2, excerciseId2, wsShuffled, wuIntId1);
            WorkUnitTemplate wu3 = WorkUnitTemplate.PlanWorkUnit(wuId, progn1, excerciseId, oneWs, wuIntId2);

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


            // Check PNums
            IdType toMoveId = new IdType(4);
            IdType toMoveId2 = new IdType(5);
            uint toMovePnum = 0;
            uint toMovePnum2 = 1;

            WorkUnitTemplate wu = WorkUnitTemplate.PlanWorkUnit(wuId, progn1, excerciseId, wsFull, null, noteId);
            wu.MoveWorkingSetToNewProgressiveNumber(toMoveId, toMovePnum);  
            wu.MoveWorkingSetToNewProgressiveNumber(toMoveId2, toMovePnum2);
            Assert.Equal(toMovePnum, wu.FindWorkingSetById(toMoveId).ProgressiveNumber);
            Assert.Equal(toMovePnum2, wu.FindWorkingSetById(toMoveId2).ProgressiveNumber);
            Assert.Equal(wu.WorkingSets.OrderBy(x => x.ProgressiveNumber), wu.WorkingSets);
        }


        [Fact]
        public void WorkUnitRepetitionsFullTest()
        {
            int wsMin = 2;
            int wsMax = 7;
            int repsMax = 20;
            int repsMin = 1;
            int restMin = 11;
            int restMax = 500;
            int wsToRemoveNum = 2;

            int noteMin = 10, noteMax = 10000;
            int excerciseMin = 1, excerciseMax = 100;
            int wuIntTechniquesMin = 0, wuIntTechniquesMax = 3;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 100;

            float amrapProbability = 0.1f;   // Probability that an AMRAP set is created

            float intPercMin = 0.5f, intePercMax = 1f;
            float rmMin = 1f, rmMax = 20f;
            float rpeMin = 3f, rpeMax = 11f;
            float effortMin, effortMax;

            uint newPnum;

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

                int wuIntTechniquesNum = RandomFieldGenerator.RandomInt(wuIntTechniquesMin, wuIntTechniquesMax);
                List <IdType> wuIntTechniques = new List<IdType>();
                for (int i = 0; i < wuIntTechniquesNum; i++)
                    wuIntTechniques.Add(new IdType(RandomFieldGenerator.RandomIntValueExcluded(intTechniqueIdMin, intTechniqueIdMax, wuIntTechniques.Select(x => (int)x.Id))));

                for (int iws = 0; iws < iwsMax; iws++)
                {
                    // New WS
                    WorkingSetTemplate newWs = StaticUtils.BuildRandomWorkingSet(iws + 1, iws, effortType, effortMin, effortMax, repsMin, repsMax, true, amrapProbability, restMin, restMax, tuts);
                    ws.Add(newWs);

                    // Build the WU
                    if(iws == 0)
                        wu = WorkUnitTemplate.PlanWorkUnit(wuId, 0, excercise, ws, wuIntTechniques, noteId);

                    else
                        wu.AddWorkingSet(newWs.Repetitions, newWs.Rest, newWs.Effort, newWs.Tempo, new List<IdType>(newWs.IntensityTechniqueIds));


                    //totalReps += newWs.ToRepetitions();
                    //totalRest += newWs.Rest.Value;
                    //totalTempo += newWs.Tempo.ToSeconds() * newWs.ToRepetitions();

                    CheckWorkUnitSets(wu, ws, effortType);  // Asserts
                }

                // Check WU 
                Assert.Equal(iwsMax, wu.WorkingSets.Count);
                Assert.Equal(noteId, wu.OwnerNoteId);
                Assert.Equal(excercise, wu.ExcerciseId);

                // Change the WU
                CheckWorkUnitChanges(wu);

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
                        //;
                    else
                    {
                        wu.RemoveWorkingSet(new IdType(idNum));

                        Assert.Equal(ws.Count - iws - 1, wu.WorkingSets.Count);
                        Assert.DoesNotContain(removed, wu.WorkingSets);

                        IEnumerable<WorkingSetTemplate> wsRemoved = ws.Where(x => idsRemoved.Contains((int)x.Id.Id));
                        wsLeft = ws.Where(x => !idsRemoved.Contains((int)x.Id.Id));

                        //int repsLeft = totalReps - wsRemoved.Sum(x => x.ToRepetitions());
                        //int restLeft = totalRest - wsRemoved.Sum(x => x.Rest.Value);
                        //int tempoLeft = totalTempo - wsRemoved.Sum(x => x.Tempo.ToSeconds() * x.ToRepetitions());

                        CheckWorkUnitSets(wu, wsLeft, effortType, false);  // Asserts
                    }
                }

                // Update with the real values -> progressive numbers are different!
                wsLeft = wu.WorkingSets;

                // Change WSs
                if (wsLeft.Count() > 0)
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

                        IdType newIntTechnique2 = new IdType(RandomFieldGenerator.RandomIntValueExcluded(
                            2 * wuIntTechniquesMax + 1, 3 * intTechniqueIdMax, toChange.IntensityTechniqueIds.Select(x => (int)x.Id)));

                        wu.AddWorkingSetIntensityTechnique(toChange.Id, newIntTechnique2);
                        Assert.Equal(toChange.IntensityTechniqueIds.Count + 1, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds.Count);
                        Assert.Contains(newIntTechnique2, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds);

                        IdType remIntTechnique2 = new IdType(RandomFieldGenerator.ChooseAmong(
                            wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds.Select(x => (int)x.Id).ToList()));

                        if (wu.IntensityTechniquesIds.Contains(remIntTechnique2))
                        {
                            Assert.Throws<TrainingDomainInvariantViolationException>(() => wu.RemoveWorkingSetIntensityTechnique(toChange.Id, remIntTechnique2));
                            wu.AddWorkingSetIntensityTechnique(toChange.Id, remIntTechnique2);  // Restore from invalid state
                        }
                        else
                        {
                            wu.RemoveWorkingSetIntensityTechnique(toChange.Id, remIntTechnique2);
                            Assert.Equal(toChange.IntensityTechniqueIds.Count, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds.Count);
                            Assert.DoesNotContain(remIntTechnique2, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds);
                        }
                    }

                    // Check PNums
                    uint destPnum = (uint)RandomFieldGenerator.ChooseAmong<int>(wu.WorkingSets.Select(x => (int)x.ProgressiveNumber).ToList());
                    IdType toMoveWsId = new IdType(RandomFieldGenerator.ChooseAmong<int>(wu.WorkingSets.Select(x => (int)x.Id.Id).ToList()));
                    IdType srcWsId = wu.FindWorkingSetByProgressiveNumber((int)destPnum).Id;
                    uint srcPnum = wu.FindWorkingSetById(toMoveWsId).ProgressiveNumber;

                    wu.MoveWorkingSetToNewProgressiveNumber(toMoveWsId, destPnum);
                    Assert.Equal(destPnum, wu.FindWorkingSetById(toMoveWsId).ProgressiveNumber);
                    Assert.Equal(srcPnum, wu.FindWorkingSetById(srcWsId).ProgressiveNumber);
                    Assert.Equal(wu.WorkingSets.OrderBy(x => x.ProgressiveNumber), wu.WorkingSets);
                }
            }
        }


        [Fact]
        public void WorkUnitTimedSerieFullTest()
        {
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
            int wuIntTechniquesMin = 0, wuIntTechniquesMax = 3;
            int intTechniqueIdMax = 100;

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

                int wuIntTechniquesNum = RandomFieldGenerator.RandomInt(wuIntTechniquesMin, wuIntTechniquesMax);
                List<IdType> wuIntTechniques = new List<IdType>();
                for (int i = 0; i < wuIntTechniquesNum; i++)
                    wuIntTechniques.Add(new IdType(RandomFieldGenerator.RandomIntValueExcluded(1, 100, wuIntTechniques.Select(x => (int)x.Id))));

                for (int iws = 0; iws < iwsMax; iws++)
                {
                    // New WS
                    WorkingSetTemplate newWs = StaticUtils.BuildRandomWorkingSet(iws + 1, iws, effortType, effortMin, effortMax, repsMin, repsMax, false, 0f, restMin, restMax, tuts);
                    ws.Add(newWs);

                    // Build the WU
                    if (iws == 0)
                        wu = WorkUnitTemplate.PlanWorkUnit(wuId, 0, excercise, ws, wuIntTechniques, noteId);

                    else
                        wu.AddWorkingSet(newWs.Repetitions, newWs.Rest, newWs.Effort, newWs.Tempo, new List<IdType>(newWs.IntensityTechniqueIds));


                    //totalReps += newWs.ToRepetitions();
                    //totalRest += newWs.Rest.Value;
                    //totalTempo += newWs.ToSecondsUnderTension();

                    CheckWorkUnitSets(wu, ws, effortType);  // Asserts
                }

                // Check WU 
                Assert.Equal(iwsMax, wu.WorkingSets.Count);
                Assert.Equal(noteId, wu.OwnerNoteId);
                Assert.Equal(excercise, wu.ExcerciseId);

                // Change the WU
                CheckWorkUnitChanges(wu);

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
                        //;
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => wu.RemoveWorkingSet(new IdType(idNum)));
                    else
                    {
                        wu.RemoveWorkingSet(new IdType(idNum));

                        Assert.Equal(ws.Count - iws - 1, wu.WorkingSets.Count);
                        Assert.DoesNotContain(removed, wu.WorkingSets);

                        IEnumerable<WorkingSetTemplate> wsRemoved = ws.Where(x => idsRemoved.Contains((int)x.Id.Id));
                        wsLeft = ws.Where(x => !idsRemoved.Contains((int)x.Id.Id));

                        //int repsLeft = totalReps - wsRemoved.Sum(x => x.ToRepetitions());
                        //int restLeft = totalRest - wsRemoved.Sum(x => x.Rest.Value);
                        //int tempoLeft = totalTempo - wsRemoved.Sum(x => x.ToSecondsUnderTension());

                        CheckWorkUnitSets(wu, wsLeft, effortType, false);  // Asserts
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

                        IdType newIntTechnique2 = new IdType(RandomFieldGenerator.RandomIntValueExcluded(
                            2 * intTechniqueIdMax + 1, 3 * intTechniqueIdMax, toChange.IntensityTechniqueIds.Select(x => (int)x.Id)));

                        wu.AddWorkingSetIntensityTechnique(toChange.Id, newIntTechnique2);
                        Assert.Equal(toChange.IntensityTechniqueIds.Count + 1, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds.Count);
                        Assert.Contains(newIntTechnique2, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds);

                        IdType remIntTechnique2 = new IdType(RandomFieldGenerator.ChooseAmong(
                            wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds.Select(x => (int)x.Id).ToList()));

                        if (wu.IntensityTechniquesIds.Contains(remIntTechnique2))
                        {
                            Assert.Throws<TrainingDomainInvariantViolationException>(() => wu.RemoveWorkingSetIntensityTechnique(toChange.Id, remIntTechnique2));
                            wu.AddWorkingSetIntensityTechnique(toChange.Id, remIntTechnique2);  // Restore from invalid state
                        }
                        else
                        {
                            wu.RemoveWorkingSetIntensityTechnique(toChange.Id, remIntTechnique2);
                            Assert.Equal(toChange.IntensityTechniqueIds.Count, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds.Count);
                            Assert.DoesNotContain(remIntTechnique2, wu.FindWorkingSetById(toChange.Id).IntensityTechniqueIds);
                        }
                    }
                    // Check PNums
                    uint destPnum = (uint)RandomFieldGenerator.ChooseAmong<int>(wu.WorkingSets.Select(x => (int)x.ProgressiveNumber).ToList());
                    IdType toMoveWsId = new IdType(RandomFieldGenerator.ChooseAmong<int>(wu.WorkingSets.Select(x => (int)x.Id.Id).ToList()));
                    IdType srcWsId = wu.FindWorkingSetByProgressiveNumber((int)destPnum).Id;
                    uint srcPnum = wu.FindWorkingSetById(toMoveWsId).ProgressiveNumber;

                    wu.MoveWorkingSetToNewProgressiveNumber(toMoveWsId, destPnum);
                    Assert.Equal(destPnum, wu.FindWorkingSetById(toMoveWsId).ProgressiveNumber);
                    Assert.Equal(srcPnum, wu.FindWorkingSetById(srcWsId).ProgressiveNumber);
                    Assert.Equal(wu.WorkingSets.OrderBy(x => x.ProgressiveNumber), wu.WorkingSets);
                }
            }
        }



        [Fact]
        public void WorkoutFail()
        {
            uint pnum = 0;
            IdType id = new IdType(1);
            IList<WorkUnitTemplate> wusFirstNull = new List<WorkUnitTemplate>();
            IList<WorkUnitTemplate> wusLastNull = new List<WorkUnitTemplate>();
            IList<WorkUnitTemplate> wusMiddleNull = new List<WorkUnitTemplate>();

            wusFirstNull.Add(null);

            for (int i = 0; i < 5; i++)
            {
                wusFirstNull.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i));
                wusLastNull.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i));
                if(i == 1)
                    wusMiddleNull.Add(null);
                else
                    wusMiddleNull.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i));
            }

            wusLastNull.Add(null);


            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, pnum + 1, wusFirstNull, string.Empty));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, pnum + 1, wusMiddleNull, string.Empty));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, pnum + 1, wusLastNull, string.Empty));

            IList<WorkUnitTemplate> wusPnumStarts1 = new List<WorkUnitTemplate>();
            IList<WorkUnitTemplate> wusPnumGap = new List<WorkUnitTemplate>();

            for (int i = 0; i < 5; i++)
            {
                wusPnumStarts1.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i + 1));

                if(i == 1)
                    wusPnumGap.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i + 100));
                else
                    wusPnumStarts1.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i + 1));
            }

            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, pnum + 1, wusPnumStarts1, string.Empty));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, pnum + 1, wusPnumGap, string.Empty));
        }



        [Fact]
        public void WorkoutFullTest()
        {
            int wuMin = 2, wuMax = 7;
            int initialWuMin = 0, initialWuMax = 2;
            int woNameLengthMin = 4, woNameLengthMax = 50;
            int pnumMin = 0, pnumMax = 50;
            int wuRemoveNum = wuMin;
            int wuAddNum = 2;

            IdType woId = new IdType(1);
            WorkoutTemplate wo;
            WorkUnitTemplate wu;

            for (int itest = 0; itest < ntests; itest++)
            {
                // Generate random parameters
                int iwuMax = RandomFieldGenerator.RandomInt(wuMin, wuMax);
                string woName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax);
                WeekdayEnum specificDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.Sunday.Id));
                List<WorkUnitTemplate> initialWus = new List<WorkUnitTemplate>();
                List<WorkUnitTemplate> wus = new List<WorkUnitTemplate>();
                List<IdType> wuIds = new List<IdType>();

                // Initial WUs
                int initialWuNum = RandomFieldGenerator.RandomInt(initialWuMin, initialWuMax);
                int idnum = itest * ntests;
                int pnum = 0;

                for (int iwu = 0; iwu < initialWuNum; iwu++)
                {
                    // Act as the ID has been retrieved from the DB
                    idnum++;
                    wuIds.Add(new IdType(idnum));
                    wu = StaticUtils.BuildRandomWorkUnit(wuIds.Last().Id, pnum++);

                    initialWus.Add(wu);
                    wus.Add(wu);
                }

                // WO with initial WUs
                wo = WorkoutTemplate.PlanWorkout(woId, 0, initialWus, woName, specificDay);

                // No WUs loaded from the DB -> start the sequence
                if (initialWuNum == 0)
                    idnum = 0;

                // Add WUs
                for (int iwu = 0; iwu < iwuMax; iwu++)
                {
                    idnum++;
                    wuIds.Add(new IdType(idnum));
                    wu = StaticUtils.BuildRandomWorkUnit(wuIds.Last().Id, pnum++);

                    wo.AddWorkUnit(wu.ExcerciseId, wu.WorkingSets.ToList(), wu.IntensityTechniquesIds.ToList(), wu.OwnerNoteId);
                    wus.Add(wu);
                }

                // Check WO
                Assert.Equal(woName, wo.Name);
                Assert.Equal((uint)0, wo.ProgressiveNumber);
                Assert.Equal(specificDay, wo.SpecificWeekday);
                Assert.Equal(initialWuNum + iwuMax, wo.WorkUnits.Count);

                // Check WUs
                foreach (WorkUnitTemplate iwu in wus)
                    CheckWorkUnit(iwu, wo);

                // Modify WO
                string newName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax, 0.05f);
                uint newPnum = (uint)RandomFieldGenerator.RandomInt(pnumMin, pnumMax);
                WeekdayEnum newDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.AllTheWeek));

                wo.GiveName(newName);
                wo.MoveToNewProgressiveNumber(newPnum);
                wo.ScheduleToSpecificDay(newDay);

                Assert.Equal(newName, wo.Name);
                Assert.Equal(newPnum, wo.ProgressiveNumber);
                Assert.Equal(newDay, wo.SpecificWeekday);

                wo.UnscheduleSpecificDay();
                Assert.Equal(WeekdayEnum.Generic, wo.SpecificWeekday);

                // Modify WUs
                foreach (WorkUnitTemplate iwu in wo.WorkUnits)
                    CheckWorkUnitChanges(wo, iwu);


                // Remove WUs
                int nWorkUnits = wo.WorkUnits.Count;

                for (int i = 0; i < wuRemoveNum; i++)
                {
                    IdType toRemoveId = RandomFieldGenerator.ChooseAmong<IdType>(wo.WorkUnits.Select(x => x.Id).ToList());
                    WorkUnitTemplate removed = wo.FindWorkUnitById(toRemoveId);

                    wo.RemoveWorkUnit(toRemoveId);

                    Assert.Equal(nWorkUnits - i - 1, wo.WorkUnits.Count);

                    if(wo.WorkUnits.Count > 0)
                    {
                        Assert.DoesNotContain(removed, wo.WorkUnits);
                        // Check pnums
                        Assert.Equal(0, (int)wo.WorkUnits.OrderBy(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);
                        Assert.Equal(wo.WorkUnits.Count - 1, (int)wo.WorkUnits.OrderByDescending(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);

                        CheckTrainingParameters(wo.WorkUnits.SelectMany(x => x.WorkingSets), wo.TrainingVolume, wo.TrainingDensity, wo.TrainingIntensity, null);
                    }

                }

                // Add WUs
                nWorkUnits = wo.WorkUnits.Count;

                for (int i = 0; i < wuAddNum; i++)
                {
                    IdType excerciseId = new IdType(RandomFieldGenerator.RandomInt(500, 1000));
                    IdType noteId = new IdType(RandomFieldGenerator.RandomInt(1, 500));
                    int wsNum = RandomFieldGenerator.RandomInt(1, 5);
                    int itNum = RandomFieldGenerator.RandomInt(0, 2);
                    
                    WorkUnitTemplate toAdd = StaticUtils.BuildRandomWorkUnit(1, wo.WorkUnits.Count);
                    wo.AddWorkUnit(toAdd.ExcerciseId, toAdd.WorkingSets.ToList(), toAdd.IntensityTechniquesIds.ToList(), toAdd.OwnerNoteId);

                    WorkUnitTemplate added = wo.FindWorkUnitByProgressiveNumber((int)toAdd.ProgressiveNumber);      // Id is different, not PNum

                    Assert.Equal(nWorkUnits + i + 1, wo.WorkUnits.Count);
                    Assert.Equal(toAdd.ProgressiveNumber, added.ProgressiveNumber);
                    Assert.Equal(toAdd.ExcerciseId, added.ExcerciseId);
                    Assert.Equal(toAdd.OwnerNoteId, added.OwnerNoteId);
                    Assert.Equal(toAdd.TrainingDensity, added.TrainingDensity);
                    Assert.Equal(toAdd.TrainingIntensity, added.TrainingIntensity);
                    Assert.Equal(toAdd.TrainingVolume, added.TrainingVolume);
                    Assert.Equal(toAdd.WorkingSets.Count, added.WorkingSets.Count);
                    Assert.Equal(toAdd.IntensityTechniquesIds.Count, added.IntensityTechniquesIds.Count);


                    CheckTrainingParameters(wo.WorkUnits.SelectMany(x => x.WorkingSets), wo.TrainingVolume, wo.TrainingDensity, wo.TrainingIntensity, null);
                }
            }
        }







        private static void CheckTrainingParameters(IEnumerable<WorkingSetTemplate> srcWorkingSets, 
            TrainingVolumeParametersValue volume, TrainingDensityParametersValue density, TrainingIntensityParametersValue intensity, TrainingEffortTypeEnum mainEffortType = null)
        {
            TrainingEffortValue avgEffort = null;
            float intensityPercentageTolerance = 0.025f;
            float rpeAndRmTolerance = 0.06f;      // Smaller numbers -> Higher tolerance. IE: 5RPE Vs 5.5RPE must be considered equivalent

            // Get the expected training parameters
            int totalReps = srcWorkingSets.Sum(x => x.ToRepetitions());
            int totalWs = srcWorkingSets.Count();
            float totalWorkload = srcWorkingSets.Sum(x => x.ToWorkload().Value);
            int totalTempo = srcWorkingSets.Sum(x => x.ToSecondsUnderTension());
            int totalRest = srcWorkingSets.Sum(x => x.ToTotalSeconds()) - totalTempo;


            // Convert to Main Effort Type
            if (totalWs == 0)
            {
                // Check Volume
                Assert.Equal(totalReps, volume.TotalReps);
                Assert.Equal(totalWs, volume.TotalWorkingSets);
                Assert.Equal(0, volume.TotalWorkload.Value);
                Assert.Equal(0, volume.GetAverageRepetitions(), 1);
                Assert.Equal(0, volume.GetAverageWorkloadPerSet().Value);

                // Check Density
                Assert.Equal(totalRest, density.TotalRest);
                Assert.Equal(totalTempo, density.TotalSecondsUnderTension);
                Assert.Equal(0, density.GetAverageRest(), 1);
                Assert.Equal(0, density.GetAverageSecondsUnderTension(), 1);

                // Check Intensity
                Assert.Null(intensity.AverageIntensity);
            }
            else
            {
                avgEffort = StaticUtils.ComputeAverageEffort(srcWorkingSets, mainEffortType);

                // Check Volume
                Assert.Equal(totalReps, volume.TotalReps);
                Assert.Equal(totalWs, volume.TotalWorkingSets);
                Assert.Equal(0, volume.TotalWorkload.Value);
                Assert.Equal((float)totalReps / (float)totalWs, volume.GetAverageRepetitions(), 1);
                Assert.Equal(0, volume.GetAverageWorkloadPerSet().Value);

                // Check Density
                Assert.Equal(totalRest, density.TotalRest);
                Assert.Equal(totalTempo, density.TotalSecondsUnderTension);
                Assert.Equal((float)totalRest / (float)totalWs, density.GetAverageRest(), 1);
                Assert.Equal((float)totalTempo / (float)totalWs, density.GetAverageSecondsUnderTension(), 1);

                // Check Intensity
                if (srcWorkingSets.All(x => !x.IsAMRAP()) && !avgEffort.IsRPE())     // AMRAP with RPE effort, can't be managed!
                {
                    if(avgEffort.IsIntensityPercentage())
                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: intensityPercentageTolerance);    // Rounding error can be moderately high because of multiple conversions

                    else
                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: rpeAndRmTolerance);    // Rounding error can be moderately high because of multiple conversions
                }
            }
        }


        private static void CheckWorkUnitSets(WorkUnitTemplate wu, IEnumerable<WorkingSetTemplate> ws, TrainingEffortTypeEnum effortType, bool wsFullCheck = true)
        {
            TrainingEffortValue avgEffort;

            foreach(WorkingSetTemplate wsCheck in ws)
            {
                Assert.Contains(wsCheck, wu.WorkingSets);

                if(wsFullCheck)
                {
                    CheckWorkingSet(wsCheck, wu.FindWorkingSetByProgressiveNumber((int)wsCheck.ProgressiveNumber), wu);
                    CheckWorkingSet(wsCheck, wu.FindWorkingSetById(new IdType(wsCheck.Id.Id)), wu);
                }
            }

            CheckTrainingParameters(wu.WorkingSets, wu.TrainingVolume, wu.TrainingDensity, wu.TrainingIntensity, effortType);;
        }


        private static void CheckWorkingSet(WorkingSetTemplate left, WorkingSetTemplate right, WorkUnitTemplate wunit = null)
        {
            Assert.Equal(left.Repetitions, right.Repetitions);
            Assert.Equal(left.Rest, right.Rest);
            Assert.Equal(left.Tempo, right.Tempo);
            Assert.Equal(left.Effort, right.Effort);

            // Left and right must have the same Int techniques
            if(wunit == null)
            {
                Assert.Equal(left.IntensityTechniqueIds, right.IntensityTechniqueIds);

                foreach (IdType idLeft in left.IntensityTechniqueIds)
                    Assert.Contains(idLeft, right.IntensityTechniqueIds);
            }
            else
            {
                // Right has the left Int techniques + the WU ones
                Assert.Equal(left.IntensityTechniqueIds.Count + wunit.IntensityTechniquesIds.Count, right.IntensityTechniqueIds.Count);

                foreach (IdType idLeft in left.IntensityTechniqueIds)
                    Assert.Contains(idLeft, right.IntensityTechniqueIds);

                foreach (IdType idWunit in wunit.IntensityTechniquesIds)
                    Assert.Contains(idWunit, right.IntensityTechniqueIds);
            }
        }


        private static void CheckWorkUnitChanges(WorkoutTemplate workout, WorkUnitTemplate workUnit)
        {
            int excerciseIdMin = 1, excerciseIdMax = 200;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;
            int noteIdMin = 25, noteIdMax = 500;
            int wsToRemoveNum = 2;

            // Get the WU from the WO - must be equal to iwu
            WorkUnitTemplate toCheck = workout.FindWorkUnitById(workUnit.Id);


            IdType newExcerciseId = new IdType(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax));
            IdType newNoteId = new IdType(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));

            int srcIntensityTechniquesNum = workUnit.IntensityTechniquesIds.Count;

            workout.AssignWorkUnitExcercise(workUnit.Id, newExcerciseId);
            workout.AssignWorkUnitNote(workUnit.Id, newNoteId);

            // Add a single intensity technique
            IdType newIntTechnique = new IdType(RandomFieldGenerator.RandomIntValueExcluded(intTechniqueIdMin, intTechniqueIdMax,
                workUnit.IntensityTechniquesIds.Select(x => (int)x.Id)));

            workout.AddWorkUnitIntensityTechnique(workUnit.Id, newIntTechnique);

            Assert.Equal(srcIntensityTechniquesNum + 1, toCheck.IntensityTechniquesIds.Count);
            Assert.Contains(newIntTechnique, toCheck.IntensityTechniquesIds);

            foreach (WorkingSetTemplate wset in toCheck.WorkingSets)
                Assert.Contains(newIntTechnique, wset.IntensityTechniqueIds);

            // Remove a single intensity technique
            toCheck.RemoveWorkUnitIntensityTechnique(newIntTechnique);
            Assert.Equal(srcIntensityTechniquesNum, toCheck.IntensityTechniquesIds.Count);
            Assert.DoesNotContain(newIntTechnique, toCheck.IntensityTechniquesIds);

            foreach (WorkingSetTemplate wset in toCheck.WorkingSets)
                Assert.DoesNotContain(newIntTechnique, wset.IntensityTechniqueIds);


            // Check WU changes
            Assert.Equal(newNoteId, toCheck.OwnerNoteId);
            Assert.Equal(newExcerciseId, toCheck.ExcerciseId);
            toCheck.RemoveNote();
            Assert.Null(toCheck.OwnerNoteId);

            // Check PNums
            uint newPnum = (uint)RandomFieldGenerator.ChooseAmong<int>(workout.WorkUnits.Select(x => (int)x.ProgressiveNumber).ToList());
            IdType srcWuId = workout.FindWorkUnitByProgressiveNumber((int)newPnum).Id;
            uint srcPnum = workout.FindWorkUnitById(workUnit.Id).ProgressiveNumber;

            workout.MoveWorkUnitToNewProgressiveNumber(workUnit.Id, newPnum);
            Assert.Equal(newPnum, workout.FindWorkUnitById(workUnit.Id).ProgressiveNumber);
            Assert.Equal(srcPnum, workout.FindWorkUnitById(srcWuId).ProgressiveNumber);

            // Remove WSs
            List<WorkingSetTemplate> originalSets = toCheck.WorkingSets.ToList();
            List<WorkingSetTemplate> finalSets = toCheck.WorkingSets.ToList();
            int totalReps = originalSets.Sum(x => x.ToRepetitions());
            int totalRest = originalSets.Sum(x => x.ToTotalSeconds() - x.ToSecondsUnderTension());
            int totalTempo = originalSets.Sum(x => x.ToSecondsUnderTension());
            //int finalReps = totalReps, finalRest = totalRest, finalTempo = totalTempo;

            for (int iws = 0; iws < wsToRemoveNum; iws++)
            {
                long removeIdNum = RandomFieldGenerator.ChooseAmong(toCheck.WorkingSets.Select(x => x.Id.Id).ToList());

                WorkingSetTemplate removed = workout.FindWorkUnitById(toCheck.Id).FindWorkingSetById(new IdType(removeIdNum));

                if (toCheck.WorkingSets.Count > 1)
                {
                    workout.RemoveWorkingSet(new IdType(removeIdNum));

                    Assert.Equal(originalSets.Count - iws - 1, toCheck.WorkingSets.Count);
                    Assert.DoesNotContain(removed, toCheck.WorkingSets);

                    IEnumerable<WorkingSetTemplate> wsRemoved = originalSets.Where(x => x.Id.Id == removeIdNum);
                    finalSets = finalSets.Where(x => x.Id.Id != removeIdNum).ToList();

                    //finalReps -= wsRemoved.Sum(x => x.ToRepetitions());
                    //finalRest -= wsRemoved.Sum(x => x.Rest.Value);
                    //finalTempo -= wsRemoved.Sum(x => x.ToSecondsUnderTension());

                    CheckWorkUnitSets(toCheck, finalSets, toCheck.TrainingIntensity.AverageIntensity.EffortType, false);  // Asserts
                }
                else
                    ;       // Removing the last WS will lead to an error in the following iterations: no check for failure
            }
        }


        private static void CheckWorkUnit(WorkUnitTemplate workUnit, WorkoutTemplate workout)
        {
            float intPercTolerance = 0.3f;
            float rpeAndRmTolerance = 0.5f;

            // Check WU
            Assert.Equal(workUnit, workout.FindWorkUnitById(workUnit.Id));
            Assert.Equal(workUnit.ExcerciseId, workout.FindWorkUnitById(workUnit.Id).ExcerciseId);
            Assert.Equal(workUnit.IntensityTechniquesIds, workout.FindWorkUnitById(workUnit.Id).IntensityTechniquesIds);
            Assert.Equal(workUnit.OwnerNoteId, workout.FindWorkUnitById(workUnit.Id).OwnerNoteId);
            Assert.Equal(workUnit.ProgressiveNumber, workout.FindWorkUnitById(workUnit.Id).ProgressiveNumber);

            // Check WSs
            foreach (WorkingSetTemplate iws in workUnit.WorkingSets)
            {
                CheckWorkingSet(iws, workout.FindWorkingSetById(iws.Id));
                CheckWorkingSet(iws, workout.FindWorkingSetByProgressiveNumber((int)workUnit.ProgressiveNumber, (int)iws.ProgressiveNumber));
                CheckWorkingSet(iws, workout.FindWorkUnitById(workUnit.Id).FindWorkingSetById(iws.Id));
                CheckWorkingSet(iws, workout.FindWorkUnitByProgressiveNumber((int)workUnit.ProgressiveNumber).FindWorkingSetById(iws.Id));
            }

            // Check Training Parameters
            TrainingEffortTypeEnum mainEffortType = workUnit.TrainingIntensity.AverageIntensity.EffortType;
            float avgEffort;

            if(mainEffortType == TrainingEffortTypeEnum.RM)
                avgEffort = workUnit.WorkingSets.Average(x => x.Effort.ToRm(x.Repetitions).Value);

            else if (mainEffortType == TrainingEffortTypeEnum.RPE)
                avgEffort = workUnit.WorkingSets.Average(x => x.Effort.ToRPE(x.Repetitions).Value);

            else
                avgEffort = workUnit.WorkingSets.Average(x => x.Effort.ToIntensityPercentage(x.Repetitions).Value);

            float avgReps = (float)workUnit.WorkingSets.Average(x => x.ToRepetitions());
            float avgWorkload = workUnit.WorkingSets.Average(x => x.ToWorkload().Value);
            float avgRest = (float)workUnit.WorkingSets.Average(x => x.ToTotalSeconds() - x.ToSecondsUnderTension());
            float avgTut = (float)workUnit.WorkingSets.Average(x => x.ToSecondsUnderTension());

            Assert.Equal(avgReps, workUnit.TrainingVolume.GetAverageRepetitions());
            Assert.Equal(avgWorkload, workUnit.TrainingVolume.GetAverageWorkloadPerSet().Value);
            Assert.Equal(avgRest, workUnit.TrainingDensity.GetAverageRest());
            Assert.Equal(avgTut, workUnit.TrainingDensity.GetAverageSecondsUnderTension());

            if (mainEffortType == TrainingEffortTypeEnum.IntensityPerc)
                StaticUtils.CheckConversions(avgEffort, workUnit.TrainingIntensity.AverageIntensity.Value, tolerance: intPercTolerance);

            else
                StaticUtils.CheckConversions(avgEffort, workUnit.TrainingIntensity.AverageIntensity.Value, tolerance: rpeAndRmTolerance);
        }


        private static void CheckWorkUnitChanges(WorkUnitTemplate src)
        {
            // Tuning parameters
            int pnumMin = 0, pnumMax = 100;
            int excerciseIdMin = 1, excerciseIdMax = 1000;
            int noteIdMin = 1, noteIdMax = 1000;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;

            uint newPnum = (uint)RandomFieldGenerator.RandomInt(pnumMin, pnumMax);
            IdType newExcerciseId = new IdType(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax));
            IdType newNoteId = new IdType(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));

            int srcIntensityTechniquesNum = src.IntensityTechniquesIds.Count;

            src.AssignNote(newNoteId);
            src.AssignExcercise(newExcerciseId);
            src.MoveToNewProgressiveNumber(newPnum);

            // Add one intensity technique
            //IdType newIntTechnique = new IdType(RandomFieldGenerator.RandomIntValueExcluded(intTechniqueIdMax + 1, 2 * intTechniqueIdMax, src.IntensityTechniquesIds.Select(x => (int)x.Id)));
            IdType newIntTechnique = new IdType(RandomFieldGenerator.RandomIntValueExcluded(intTechniqueIdMin, intTechniqueIdMax, 
                src.IntensityTechniquesIds.Select(x => (int)x.Id)));

            src.AddWorkUnitIntensityTechnique(newIntTechnique);
            Assert.Equal(srcIntensityTechniquesNum + 1, src.IntensityTechniquesIds.Count);
            Assert.Contains(newIntTechnique, src.IntensityTechniquesIds);

            foreach (WorkingSetTemplate wset in src.WorkingSets)
                Assert.Contains(newIntTechnique, wset.IntensityTechniqueIds);

            // Remove one intensity technique
            src.RemoveWorkUnitIntensityTechnique(newIntTechnique);
            Assert.Equal(srcIntensityTechniquesNum, src.IntensityTechniquesIds.Count);
            Assert.DoesNotContain(newIntTechnique, src.IntensityTechniquesIds);

            foreach (WorkingSetTemplate wset in src.WorkingSets)
                Assert.DoesNotContain(newIntTechnique, wset.IntensityTechniqueIds);


            // Check WU changes
            Assert.Equal(newPnum, src.ProgressiveNumber);
            Assert.Equal(newNoteId, src.OwnerNoteId);
            Assert.Equal(newExcerciseId, src.ExcerciseId);
            src.RemoveNote();
            Assert.Null(src.OwnerNoteId);
        }



    }
}
