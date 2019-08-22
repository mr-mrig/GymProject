using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using Xunit;
using GymProject.Domain.Test.Util;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;

namespace GymProject.Domain.Test.UnitTest
{
    public class WorkoutTemplateAggregateValueObjectsTest
    {

        [Fact]
        public void RestPeriodSetFail()
        {
            // No bad cases
        }


        [Fact]
        public void RestPeriodSet()
        {
            int rest1 = 90, rest2 = (int)77.7f;

            RestPeriodValue restVal1 = RestPeriodValue.SetRestSeconds((uint)rest1);
            RestPeriodValue restVal2 = RestPeriodValue.SetRestSeconds((uint)rest2);
            RestPeriodValue restVal3 = RestPeriodValue.SetRestNotSpecified();
            RestPeriodValue restVal4 = RestPeriodValue.SetFullRecoveryRest();

            Assert.Equal(rest1, restVal1.Value);
            Assert.Equal(rest2, restVal2.Value);
            Assert.Equal(RestPeriodValue.FullRecoveryRestValue, restVal4.Value);
            Assert.Equal(TimeMeasureUnitEnum.Seconds, restVal1.MeasureUnit);
            Assert.Equal(TimeMeasureUnitEnum.Seconds, restVal2.MeasureUnit);
            Assert.Equal(TimeMeasureUnitEnum.Seconds, restVal4.MeasureUnit);
            Assert.True(restVal1.IsRestSpecified());
            Assert.True(restVal2.IsRestSpecified());
            Assert.False(restVal3.IsRestSpecified());
        }


        [Fact]
        public void RestPeriodOperatorsCheck()
        {
            int rest1 = 90, rest2 = (int)77.7f;

            RestPeriodValue restVal1 = RestPeriodValue.SetRestNotSpecified();
            RestPeriodValue sum = restVal1 + rest1;
            RestPeriodValue diff = sum - rest2;

            Assert.Equal(rest1, sum.Value);
            Assert.Equal(rest1 - rest2, diff.Value);
            Assert.Equal(TimeMeasureUnitEnum.Seconds, restVal1.MeasureUnit);


            Assert.Throws<TrainingDomainInvariantViolationException>(() => restVal1 - rest1);
        }


        [Fact]
        public void WSRepetitionValueSetFail()
        {
            // No bad cases
        }


        [Fact]
        public void WSRepetitionValueSet()
        {
            int repsVal1 = 0, repsVal2 = 10, seconds1 = 0, seconds2 = (int)90.9f;

            WSRepetitionValue amrap1 = WSRepetitionValue.TrackAMRAP();
            WSRepetitionValue notSetReps = WSRepetitionValue.TrackNotSetRepetitions();
            WSRepetitionValue notSetTime = WSRepetitionValue.TrackNotSetTime();
            WSRepetitionValue reps1 = WSRepetitionValue.TrackRepetitionSerie((uint)repsVal1);
            WSRepetitionValue reps2 = WSRepetitionValue.TrackRepetitionSerie((uint)repsVal2);
            WSRepetitionValue timed1 = WSRepetitionValue.TrackTimedSerie((uint)seconds1);
            WSRepetitionValue timed2 = WSRepetitionValue.TrackTimedSerie((uint)seconds2);

            Assert.True(amrap1.IsAMRAP());
            Assert.False(notSetReps.IsAMRAP());
            Assert.False(notSetTime.IsAMRAP());
            Assert.False(reps1.IsAMRAP());
            Assert.False(reps2.IsAMRAP());
            Assert.False(timed1.IsAMRAP());
            Assert.False(timed2.IsAMRAP());

            Assert.True(amrap1.IsRepetitionBasedSerie());
            Assert.True(notSetReps.IsRepetitionBasedSerie());
            Assert.False(notSetTime.IsRepetitionBasedSerie());
            Assert.True(reps1.IsRepetitionBasedSerie());
            Assert.True(reps2.IsRepetitionBasedSerie());
            Assert.False(timed1.IsRepetitionBasedSerie());
            Assert.False(timed2.IsRepetitionBasedSerie());

            Assert.False(amrap1.IsTimedBasedSerie());
            Assert.False(notSetReps.IsTimedBasedSerie());
            Assert.True(notSetTime.IsTimedBasedSerie());
            Assert.False(reps1.IsTimedBasedSerie());
            Assert.False(reps2.IsTimedBasedSerie());
            Assert.True(timed1.IsTimedBasedSerie());
            Assert.True(timed2.IsTimedBasedSerie());

            Assert.True(amrap1.IsValueSpecified());
            Assert.False(notSetReps.IsValueSpecified());
            Assert.False(notSetTime.IsValueSpecified());
            Assert.False(reps1.IsValueSpecified());
            Assert.True(reps2.IsValueSpecified());
            Assert.False(timed1.IsValueSpecified());
            Assert.True(timed2.IsValueSpecified());

            Assert.Equal(repsVal1, reps1.Value);
            Assert.Equal(repsVal2, reps2.Value);
            Assert.Equal(seconds1, timed1.Value);
            Assert.Equal(seconds2, timed2.Value);

            Assert.Equal(WSWorkTypeEnum.RepetitionBasedSerie, amrap1.WorkType);
            Assert.Equal(WSWorkTypeEnum.RepetitionBasedSerie, reps1.WorkType);
            Assert.Equal(WSWorkTypeEnum.RepetitionBasedSerie, reps2.WorkType);
            Assert.Equal(WSWorkTypeEnum.TimeBasedSerie, timed1.WorkType);
            Assert.Equal(WSWorkTypeEnum.TimeBasedSerie, timed2.WorkType);
        }

        
        [Fact]
        public void TUTPlanFail()
        {
            string tooLong = "30303", tooShort1 = "303", tooShort2 = "1";
            string invalidChars1 = "3e30", invalidChars2 = "303p", invalidChars3 = ".020";

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TUTValue.PlanTUT(tooLong));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TUTValue.PlanTUT(tooShort1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TUTValue.PlanTUT(tooShort2));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TUTValue.PlanTUT(invalidChars1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TUTValue.PlanTUT(invalidChars2));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TUTValue.PlanTUT(invalidChars3));
        }



        [Fact]
        public void TUTPlanSet()
        {
            string notSetVal = null, notSetVal1 = "", tutVal1 = "3030", tutVal2 = "30x0", tutVal3 = "X303", tutVal4 = "205x";

            TUTValue notSet1 = TUTValue.PlanTUT(notSetVal);
            TUTValue notSet2 = TUTValue.PlanTUT(notSetVal1);
            TUTValue tut1 = TUTValue.PlanTUT(tutVal1);
            TUTValue tut2 = TUTValue.PlanTUT(tutVal2);
            TUTValue tut3 = TUTValue.PlanTUT(tutVal3);
            TUTValue tut4 = TUTValue.PlanTUT(tutVal4);

            Assert.Equal(TUTValue.DefaultTempo, notSet1.TUT, ignoreCase: true);
            Assert.Equal(TUTValue.DefaultTempo, notSet2.TUT, ignoreCase: true);
            Assert.Equal(tutVal1, tut1.TUT, ignoreCase: true);
            Assert.Equal(tutVal2, tut2.TUT, ignoreCase: true);
            Assert.Equal(tutVal3, tut3.TUT, ignoreCase: true);
            Assert.Equal(tutVal4, tut4.TUT, ignoreCase: true);

            Assert.Equal(3, tut1.GetConcentric());
            Assert.Equal(0, tut1.GetStop1());
            Assert.Equal(3, tut1.GetEccentric());
            Assert.Equal(0, tut1.GetStop2());

            Assert.Equal(2, tut4.GetConcentric());
            Assert.Equal(0, tut4.GetStop1());
            Assert.Equal(5, tut4.GetEccentric());
            Assert.Equal(0, tut4.GetStop2());

            Assert.Equal(1, notSet1.GetConcentric());
            Assert.Equal(0, notSet1.GetStop1());
            Assert.Equal(2, notSet1.GetEccentric());
            Assert.Equal(0, notSet1.GetStop2());
        }


        [Fact]
        public void TUTPlanFunctions()
        {
            string notSetVal = null, tutVal1 = "3030", tutVal2 = "205x";

            TUTValue notSet1 = TUTValue.PlanTUT(notSetVal);
            TUTValue tut1 = TUTValue.PlanTUT(tutVal1);
            TUTValue tut2 = TUTValue.PlanTUT(tutVal2);

            Assert.False(notSet1.IsTempoSpecified());
            Assert.True(tut1.IsTempoSpecified());
            Assert.True(tut2.IsTempoSpecified());

            Assert.Equal(3 + 0 + 3 + 0, tut1.ToSeconds());
            Assert.Equal(2 + 0 + 5 + 0, tut2.ToSeconds());
            Assert.Equal(1 + 0 + 2 + 0, notSet1.ToSeconds());
        }


        [Fact]
        public void TrainingEffortFail()
        {
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsIntensityPerc(115f));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsIntensityPerc(-1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsIntensityPerc(0));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsRM(-1));
            //Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsRM(11.5f));
            //Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsRM(4.2f));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsRM(0));

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsRPE(-1));
            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingEffortValue.AsRPE(0));
        }


        [Fact]
        public void TrainingEffortIntensity()
        {
            int precision = 2;
            float intensityRatio1 = 0.7f, intensityRatio2 = 1.05f, intensityPerc1 = 75.5f, intensityPerc2 = 105f, intensityPerc3 = 1.25f;

            TrainingEffortValue intensityR1 = TrainingEffortValue.AsIntensityPerc(intensityRatio1);
            TrainingEffortValue intensityR2 = TrainingEffortValue.AsIntensityPerc(intensityRatio2);
            TrainingEffortValue intensityP1 = TrainingEffortValue.AsIntensityPerc(intensityPerc1);
            TrainingEffortValue intensityP2 = TrainingEffortValue.AsIntensityPerc(intensityPerc2);
            TrainingEffortValue intensityP3 = TrainingEffortValue.AsIntensityPerc(intensityPerc3);

            Assert.Equal(intensityRatio1 * 100f, intensityR1.Value, precision);
            Assert.Equal(intensityRatio2 * 100f, intensityR2.Value, precision);
            Assert.Equal(intensityPerc1, intensityP1.Value, precision);
            Assert.Equal(intensityPerc2, intensityP2.Value, precision);
            Assert.Equal(intensityPerc3, intensityP3.Value, precision);

            Assert.Equal(TrainingEffortTypeEnum.IntensityPerc, intensityR1.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.IntensityPerc, intensityR2.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.IntensityPerc, intensityP1.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.IntensityPerc, intensityP2.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.IntensityPerc, intensityP3.EffortType);

            Assert.True(intensityR1.IsIntensityPercentage());
            Assert.True(intensityR2.IsIntensityPercentage());
            Assert.True(intensityP1.IsIntensityPercentage());
            Assert.True(intensityP2.IsIntensityPercentage());
            Assert.True(intensityP3.IsIntensityPercentage());

            Assert.False(intensityR1.IsRM());
            Assert.False(intensityR2.IsRM());
            Assert.False(intensityP1.IsRM());
            Assert.False(intensityP2.IsRM());
            Assert.False(intensityP3.IsRM());

            Assert.False(intensityR1.IsRPE());
            Assert.False(intensityR2.IsRPE());
            Assert.False(intensityP1.IsRPE());
            Assert.False(intensityP2.IsRPE());
            Assert.False(intensityP3.IsRPE());
        }


        [Fact]
        public void TrainingEffortRM()
        {
            float rmVal1 = 10f, rmVal2 = 4f, rmVal3 = 100f;

            TrainingEffortValue rm1 = TrainingEffortValue.AsRM(rmVal1);
            TrainingEffortValue rm2 = TrainingEffortValue.AsRM(rmVal2);
            TrainingEffortValue rm3 = TrainingEffortValue.AsRM(rmVal3);

            Assert.Equal(rmVal1, rm1.Value);
            Assert.Equal(rmVal2, rm2.Value);
            Assert.Equal(rmVal3, rm3.Value);

            Assert.Equal(TrainingEffortTypeEnum.RM, rm1.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.RM, rm2.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.RM, rm3.EffortType);

            Assert.False(rm1.IsIntensityPercentage());
            Assert.False(rm2.IsIntensityPercentage());
            Assert.False(rm3.IsIntensityPercentage());

            Assert.True(rm1.IsRM());
            Assert.True(rm2.IsRM());
            Assert.True(rm3.IsRM());

            Assert.False(rm1.IsRPE());
            Assert.False(rm2.IsRPE());
            Assert.False(rm3.IsRPE());
        }


        [Fact]
        public void TrainingEffortRPE()
        {
            int precision = 1;
            float rpeVal1 = 10f, rpeVal2 = 11.51f, rpeVal3 = 3.4f;

            TrainingEffortValue rpe1 = TrainingEffortValue.AsRPE(rpeVal1);
            TrainingEffortValue rpe2 = TrainingEffortValue.AsRPE(rpeVal2);
            TrainingEffortValue rpe3 = TrainingEffortValue.AsRPE(rpeVal3);

            //Assert.Equal(CommonUtilities.RoundToPointFive(rpeVal1), rpe1.Value, precision);
            //Assert.Equal(CommonUtilities.RoundToPointFive(rpeVal2), rpe2.Value, precision);
            //Assert.Equal(CommonUtilities.RoundToPointFive(rpeVal3), rpe3.Value, precision);

            Assert.Equal(rpeVal1, rpe1.Value, precision);
            Assert.Equal(rpeVal2, rpe2.Value, precision);
            Assert.Equal(rpeVal3, rpe3.Value, precision);

            Assert.Equal(TrainingEffortTypeEnum.RPE, rpe1.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.RPE, rpe2.EffortType);
            Assert.Equal(TrainingEffortTypeEnum.RPE, rpe3.EffortType);

            Assert.False(rpe1.IsIntensityPercentage());
            Assert.False(rpe2.IsIntensityPercentage());
            Assert.False(rpe3.IsIntensityPercentage());

            Assert.False(rpe1.IsRM());
            Assert.False(rpe2.IsRM());
            Assert.False(rpe3.IsRM());

            Assert.True(rpe1.IsRPE());
            Assert.True(rpe2.IsRPE());
            Assert.True(rpe3.IsRPE());
        }


        [Fact]
        public void TrainingEffortConversions()
        {
            int intPrecision = 2, otherPrecision = 1;

            float intensityVal1 = 100f, intensityVal2 = 105f, intensityVal3 = 84f;

            WSRepetitionValue amrap = WSRepetitionValue.TrackAMRAP();
            WSRepetitionValue reps10 = WSRepetitionValue.TrackRepetitionSerie(10);
            WSRepetitionValue reps5 = WSRepetitionValue.TrackRepetitionSerie(5);

            TrainingEffortValue int1 = TrainingEffortValue.AsIntensityPerc(intensityVal1);
            TrainingEffortValue int2 = TrainingEffortValue.AsIntensityPerc(intensityVal2);
            TrainingEffortValue int3 = TrainingEffortValue.AsIntensityPerc(intensityVal3);

            // No conversion
            Assert.Equal(intensityVal1, int1.ToIntensityPercentage().Value, intPrecision);
            Assert.Equal(intensityVal2, int2.ToIntensityPercentage().Value, intPrecision);
            Assert.Equal(intensityVal3, int3.ToIntensityPercentage().Value, intPrecision);

            // Intensity To RM
            Assert.Equal(1, int1.ToRm().Value, otherPrecision);
            Assert.Equal(1, int2.ToRm().Value, otherPrecision);
            Assert.Equal(6, int3.ToRm().Value, otherPrecision);

            // Intensity To RPE
            Assert.Throws<ArgumentException>(() => int1.ToRPE().Value);
            Assert.Equal(10, int1.ToRPE(amrap).Value, otherPrecision);
            Assert.Equal(10, int2.ToRPE(amrap).Value, otherPrecision);
            Assert.Equal(10, int3.ToRPE(amrap).Value, otherPrecision);
            StaticUtils.CheckConversions(19f, int1.ToRPE(reps10).Value, tolerance: 0.025f);
            StaticUtils.CheckConversions(19f, int2.ToRPE(reps10).Value, tolerance: 0.025f);
            StaticUtils.CheckConversions(14f, int3.ToRPE(reps10).Value, tolerance: 0.025f);
            StaticUtils.CheckConversions(14f, int1.ToRPE(reps5).Value, tolerance: 0.025f);
            StaticUtils.CheckConversions(14f, int2.ToRPE(reps5).Value, tolerance: 0.025f);
            StaticUtils.CheckConversions(9f, int3.ToRPE(reps5).Value, tolerance: 0.025f);

            float rmVal1 = 1, rmVal2 = 6;

            TrainingEffortValue rm1 = TrainingEffortValue.AsRM(rmVal1);
            TrainingEffortValue rm2 = TrainingEffortValue.AsRM(rmVal2);

            // No conversion
            Assert.Equal(rmVal1, rm1.ToRm().Value, intPrecision);
            Assert.Equal(rmVal2, rm2.ToRm().Value, intPrecision);

            // RM To Intensity Percentage
            Assert.Equal(100f, rm1.ToIntensityPercentage().Value, intPrecision);
            StaticUtils.CheckConversions(82.25f, rm2.ToIntensityPercentage().Value);

            // RM to RPE
            Assert.Throws<ArgumentException>(() => (10, rm2.ToRPE().Value, otherPrecision));
            Assert.Equal(10, rm1.ToRPE(amrap).Value, otherPrecision);
            Assert.Equal(10, rm2.ToRPE(amrap).Value, otherPrecision);
            Assert.Equal(19, rm1.ToRPE(reps10).Value, otherPrecision);
            Assert.Equal(14, rm2.ToRPE(reps10).Value, otherPrecision);
            Assert.Equal(14, rm1.ToRPE(reps5).Value, otherPrecision);
            Assert.Equal(9, rm2.ToRPE(reps5).Value, otherPrecision);

            float rpeVal1 = 6, rpeVal2 = 10, rpeVal3 = 11, rpeVal4 = 9.5f;

            TrainingEffortValue rpe1 = TrainingEffortValue.AsRPE(rpeVal1);
            TrainingEffortValue rpe2 = TrainingEffortValue.AsRPE(rpeVal2);
            TrainingEffortValue rpe3 = TrainingEffortValue.AsRPE(rpeVal3);
            TrainingEffortValue rpe4 = TrainingEffortValue.AsRPE(rpeVal4);

            // No conversion
            Assert.Equal(rpeVal1, rpe1.ToRPE().Value, otherPrecision);
            Assert.Equal(rpeVal2, rpe2.ToRPE().Value, otherPrecision);
            Assert.Equal(rpeVal3, rpe3.ToRPE().Value, otherPrecision);
            Assert.Equal(rpeVal4, rpe4.ToRPE().Value, otherPrecision);

            // RPE to Intensity Percentage
            Assert.Throws<ArgumentException>(() => rpe1.ToIntensityPercentage(amrap));
            StaticUtils.CheckConversions(68f, rpe1.ToIntensityPercentage(reps10).Value);
            StaticUtils.CheckConversions(77f, rpe1.ToIntensityPercentage(reps5).Value);
            StaticUtils.CheckConversions(75f, rpe2.ToIntensityPercentage(reps10).Value);
            StaticUtils.CheckConversions(86f, rpe2.ToIntensityPercentage(reps5).Value);
            StaticUtils.CheckConversions(77f, rpe3.ToIntensityPercentage(reps10).Value);
            StaticUtils.CheckConversions(89f, rpe3.ToIntensityPercentage(reps5).Value);
            StaticUtils.CheckConversions(75f, rpe4.ToIntensityPercentage(reps10).Value);
            StaticUtils.CheckConversions(86f, rpe4.ToIntensityPercentage(reps5).Value);

            // RPE to RM
            Assert.Throws<ArgumentException>(() => rpe1.ToRm(amrap));
            Assert.Equal(14, rpe1.ToRm(reps10).Value);
            Assert.Equal(9, rpe1.ToRm(reps5).Value);
            Assert.Equal(10, rpe2.ToRm(reps10).Value);
            Assert.Equal(5, rpe2.ToRm(reps5).Value);
            Assert.Equal(9, rpe3.ToRm(reps10).Value);
            Assert.Equal(4, rpe3.ToRm(reps5).Value);
            Assert.Equal(10, rpe4.ToRm(reps10).Value);
            Assert.Equal(5, rpe4.ToRm(reps5).Value);
        }


        [Fact]
        public void TrainingVolumeAndDensityCompute()
        {
            int wsNumMax = 4;
            int repsMin = 1;
            int repsMax = 15;
            int restMin = 11;
            int restMax = 500;
            int restDelta = 30;

            IEnumerable<TUTValue> tuts = new List<TUTValue>()
            {
                TUTValue.PlanTUT("1010"),
                TUTValue.PlanTUT("3030"),
                TUTValue.PlanTUT("5151"),
                TUTValue.SetGenericTUT(),
            };

            int totalReps;

            // RPE
            foreach (TUTValue tut in tuts)
            {
                for (int irest = restMin; irest <= restMax; irest += restDelta)
                {
                    RestPeriodValue rest = RestPeriodValue.SetRestSeconds((uint)irest);

                    for (int ireps = repsMin; ireps <= repsMax; ireps++)
                    {
                        totalReps = 0;
                        List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                        WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                        for (int iws = 0; iws < wsNumMax; iws++)
                        {
                            IdTypeValue id = IdTypeValue.Create(iws + 1);
                            ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, rest, null, tut));
                            totalReps += ireps;
                        }

                        TrainingVolumeParametersValue volume = TrainingVolumeParametersValue.ComputeFromWorkingSets(ws);
                        TrainingDensityParametersValue density = TrainingDensityParametersValue.ComputeFromWorkingSets(ws);

                        // Volume
                        Assert.Equal((int)wsNumMax, volume.TotalWorkingSets);
                        Assert.Equal(totalReps, volume.TotalReps);
                        Assert.Equal(WeightPlatesValue.MeasureKilograms(0), volume.TotalWorkload);
                        Assert.Equal((float)totalReps / (float)wsNumMax, volume.GetAverageRepetitions());

                        // Density
                        Assert.Equal(tut.ToSeconds() * ireps * wsNumMax, density.TotalSecondsUnderTension);
                        Assert.Equal(rest.Value * wsNumMax, density.TotalRest);
                        Assert.Equal(rest.Value, density.GetAverageRest());
                        Assert.Equal(tut.ToSeconds() * ireps, density.GetAverageSecondsUnderTension());
                    }
                }
            }
        }


        [Fact]
        public void TrainingVolumeAndDensityChange()
        {
            int wsNumMax = 5;
            int repsMin = 1;
            int repsMax = 20;
            int restMin = 11;
            int restMax = 500;
            int restDelta = 30;

            int additionalSetsMax = 2;
            int removeSetsMax = 2;


            IEnumerable<TUTValue> tuts = new List<TUTValue>()
            {
                TUTValue.PlanTUT("1010"),
                TUTValue.PlanTUT("3030"),
                TUTValue.PlanTUT("5151"),
                TUTValue.SetGenericTUT(),
            };

            int totalReps;

            // RPE
            foreach (TUTValue tut in tuts)
            {
                for (int irest = restMin; irest <= restMax; irest += restDelta)
                {
                    RestPeriodValue rest = RestPeriodValue.SetRestSeconds((uint)irest);

                    for (int ireps = repsMin; ireps <= repsMax; ireps++)
                    {
                        totalReps = 0;
                        List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                        WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                        for (int iws = 0; iws < wsNumMax; iws++)
                        {
                            IdTypeValue id = IdTypeValue.Create(iws + 1);
                            ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, rest, null, tut));
                            totalReps += ireps;
                        }

                        TrainingVolumeParametersValue volume = TrainingVolumeParametersValue.ComputeFromWorkingSets(ws);
                        TrainingDensityParametersValue density = TrainingDensityParametersValue.ComputeFromWorkingSets(ws);

                        // Add sets to the parameters
                        for(int iadditionalWs = 0; iadditionalWs < additionalSetsMax; iadditionalWs++)
                        {
                            IdTypeValue id = IdTypeValue.Create(wsNumMax + iadditionalWs);
                            WSRepetitionValue additionalWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps + 2)); // Higher reps
                            volume = volume.AddWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsNumMax, additionalWs, rest, null, tut));
                            density = density.AddWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsNumMax, additionalWs, rest, null, tut));

                            totalReps += additionalWs.Value;
                        }

                        // Remove sets from the parameters
                        for (int iremoveWs = 0; iremoveWs < removeSetsMax; iremoveWs++)
                        {
                            IdTypeValue id = IdTypeValue.Create(wsNumMax + additionalSetsMax - iremoveWs);
                            WSRepetitionValue removeWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps));         // Same reps
                            volume = volume.RemoveWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsNumMax, removeWs, rest, null, tut));
                            density = density.RemoveWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsNumMax, removeWs, rest, null, tut));

                            totalReps -= removeWs.Value;
                        }

                        int newSetsNumber = wsNumMax + additionalSetsMax - removeSetsMax;
                        float newAvgReps = (float)totalReps / (float)newSetsNumber;

                        // Volume
                        Assert.Equal(newSetsNumber, volume.TotalWorkingSets);
                        Assert.Equal(totalReps, volume.TotalReps);
                        Assert.Equal(WeightPlatesValue.MeasureKilograms(0), volume.TotalWorkload);
                        Assert.Equal((float)totalReps / (float)wsNumMax, volume.GetAverageRepetitions());

                        // Density
                        Assert.Equal(tut.ToSeconds() * newAvgReps * newSetsNumber, density.TotalSecondsUnderTension, 0);
                        Assert.Equal(rest.Value * newSetsNumber, density.TotalRest);
                        Assert.Equal(rest.Value, density.GetAverageRest(), 1);
                        Assert.Equal(tut.ToSeconds() * newAvgReps, density.GetAverageSecondsUnderTension(), 1);
                    }
                }
            }
        }


        [Fact]
        public void TrainingIntensityChange()
        {
            int totalReps;

            int wsTot = 4;
            int repsMin = 1;
            int repsMax = 20;

            int additionalSetsMax = 3;
            int removeSetsMax = 3;

            // RPE
            float rpeMin = 1;
            float rpeMax = 10;
            float rpeDelta = 0.5f;
            float changedRpe = 7;

            for (float irpe = rpeMin; irpe <= rpeMax; irpe += rpeDelta)
            {
                TrainingEffortValue rpe2 = TrainingEffortValue.AsRPE(changedRpe);
                TrainingEffortValue rpe = TrainingEffortValue.AsRPE(irpe);

                for (int ireps = repsMin; ireps <= repsMax; ireps++)
                {
                    List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                    WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                    for (int iws = 0; iws < wsTot; iws++)
                    {
                        IdTypeValue id = IdTypeValue.Create(iws + 1);
                        ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, null, rpe, null));
                    }

                    TrainingIntensityParametersValue intensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(ws, TrainingEffortTypeEnum.RPE);


                    // Add sets to the parameters
                    for (int iadditionalWs = 0; iadditionalWs < additionalSetsMax; iadditionalWs++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot + iadditionalWs);
                        WSRepetitionValue additionalWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps + 2));
                        intensity = intensity.AddWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsTot, additionalWs, effort: rpe2));  // Lower effort
                    }

                    // Remove sets from the parameters
                    for (int iremoveWs = 0; iremoveWs < removeSetsMax; iremoveWs++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot + additionalSetsMax - iremoveWs);
                        WSRepetitionValue removeWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps));
                        intensity = intensity.RemoveWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsTot, removeWs, effort: rpe));    // Same effort
                    }

                    int newSetsNumber = wsTot + additionalSetsMax - removeSetsMax;

                    float newRpe = (float)((wsTot - removeSetsMax) * rpe.Value + additionalSetsMax * rpe2.Value) / newSetsNumber;

                    StaticUtils.CheckConversions(newRpe, intensity.AverageIntensity.Value);
                }
            }

            // Intensity
            float intMin = 50f;
            float intMax = 100f;
            float intDelta = 2.5f;

            for (float iintensity = intMin; iintensity <= intMax; iintensity += intDelta)
            {
                TrainingEffortValue intensiyEffort = TrainingEffortValue.AsIntensityPerc(iintensity);
                TrainingEffortValue intensiyEffort2 = TrainingEffortValue.AsIntensityPerc(iintensity - 5f);

                for (int ireps = repsMin; ireps <= repsMax; ireps++)
                {
                    totalReps = 0;
                    List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                    WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                    for (int iws = 0; iws < wsTot; iws++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot);
                        ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, null, intensiyEffort, null));
                        totalReps += ireps;
                    }

                    TrainingIntensityParametersValue intensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(ws);

                    // Add sets to the parameters
                    for (int iadditionalWs = 0; iadditionalWs < additionalSetsMax; iadditionalWs++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot + iadditionalWs);
                        WSRepetitionValue additionalWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps + 2));
                        intensity = intensity.AddWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsTot, additionalWs, effort: intensiyEffort2));  // Lower effort
                    }

                    // Remove sets from the parameters
                    for (int iremoveWs = 0; iremoveWs < removeSetsMax; iremoveWs++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot + additionalSetsMax - iremoveWs);
                        WSRepetitionValue removeWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps));
                        intensity = intensity.RemoveWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsTot, removeWs, effort: intensiyEffort));    // Same effort
                    }
                    int newSetsNumber = wsTot + additionalSetsMax - removeSetsMax;

                    float newInt = ((wsTot - removeSetsMax) * intensiyEffort.Value
                        + additionalSetsMax * intensiyEffort2.Value) / newSetsNumber;

                    StaticUtils.CheckConversions(newInt, intensity.AverageIntensity.Value);
                }
            }

            // RM
            float rmMin = 1f;
            float rmMax = 20f;
            float rmDelta = 1;

            for (float irm = rmMin; irm <= rmMax; irm += rmDelta)
            {
                TrainingEffortValue rm2 = TrainingEffortValue.AsRM(irm + 2);
                TrainingEffortValue rm = TrainingEffortValue.AsRM(irm);

                for (int ireps = repsMin; ireps <= repsMax; ireps++)
                {
                    totalReps = 0;
                    List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                    WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                    for (int iws = 0; iws < wsTot; iws++)
                    {
                        IdTypeValue id = IdTypeValue.Create(iws + 1);
                        ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, null, rm, null));
                        totalReps += ireps;
                    }

                    TrainingIntensityParametersValue intensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(ws, TrainingEffortTypeEnum.RM);

                    // Add sets to the parameters
                    for (int iadditionalWs = 0; iadditionalWs < additionalSetsMax; iadditionalWs++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot + iadditionalWs);
                        WSRepetitionValue additionalWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps + 2));
                        intensity = intensity.AddWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsTot, additionalWs, effort: rm2));  // Lower effort
                    }

                    // Remove sets from the parameters
                    for (int iremoveWs = 0; iremoveWs < removeSetsMax; iremoveWs++)
                    {
                        IdTypeValue id = IdTypeValue.Create(wsTot + additionalSetsMax - removeSetsMax);
                        WSRepetitionValue removeWs = WSRepetitionValue.TrackRepetitionSerie((uint)(ireps));
                        intensity = intensity.RemoveWorkingSet(WorkingSetTemplate.PlanWorkingSet(id, (uint)wsTot, removeWs, effort: rm));    // Same effort
                    }
                    int newSetsNumber = wsTot + additionalSetsMax - removeSetsMax;

                    float newRm = ((wsTot - removeSetsMax) * rm.Value
                        + additionalSetsMax * rm2.Value) / newSetsNumber;

                    StaticUtils.CheckConversions(newRm, intensity.AverageIntensity.Value);
                }
            }
        }


        [Fact]
        public void TrainingIntensityCompute()
        {
            int totalReps;

            int wsTot = 5;
            int repsMin = 1;
            int repsMax = 20;

            // RPE
            float rpeMin = 1;
            float rpeMax = 10;
            float rpeDelta = 0.5f;

            for (float irpe = rpeMin; irpe <= rpeMax; irpe += rpeDelta)
            {
                TrainingEffortValue rpe = TrainingEffortValue.AsRPE(irpe);

                for (int ireps = repsMin; ireps <= repsMax; ireps++)
                {
                    totalReps = 0;
                    List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                    WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                    for (int iws = 0; iws < wsTot; iws++)
                    {
                        IdTypeValue id = IdTypeValue.Create(iws + 1);
                        ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, null, rpe, null));
                        totalReps += ireps;
                    }
                    TrainingIntensityParametersValue intensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(ws, TrainingEffortTypeEnum.RPE);

                    Assert.Equal(rpe, intensity.AverageIntensity);
                }
            }

            // Intensity
            float intMin = 50f;
            float intMax = 100f;
            float intDelta = 2.5f;

            for (float iintensity = intMin; iintensity <= intMax; iintensity += intDelta)
            {
                TrainingEffortValue intensiyEffort = TrainingEffortValue.AsIntensityPerc(iintensity);

                for (int ireps = repsMin; ireps <= repsMax; ireps++)
                {
                    totalReps = 0;
                    List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                    WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                    for (int iws = 0; iws < wsTot; iws++)
                    {
                        IdTypeValue id = IdTypeValue.Create(iws + 1);
                        ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, null, intensiyEffort, null));
                        totalReps += ireps;
                    }
                    TrainingIntensityParametersValue intensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(ws);

                    Assert.Equal(intensiyEffort, intensity.AverageIntensity);
                }
            }

            // RM
            float rmMin = 1f;
            float rmMax = 20f;
            float rmDelta = 1;

            for (float irm = rmMin; irm <= rmMax; irm += rmDelta)
            {
                TrainingEffortValue rm = TrainingEffortValue.AsRM(irm);

                for (int ireps = repsMin; ireps <= repsMax; ireps++)
                {
                    totalReps = 0;
                    List<WorkingSetTemplate> ws = new List<WorkingSetTemplate>();
                    WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie((uint)ireps);

                    for (int iws = 0; iws < wsTot; iws++)
                    {
                        IdTypeValue id = IdTypeValue.Create(iws + 1);
                        ws.Add(WorkingSetTemplate.PlanWorkingSet(id, (uint)iws, reps, null, rm, null));
                        totalReps += ireps;
                    }
                    TrainingIntensityParametersValue intensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(ws, TrainingEffortTypeEnum.RM);

                    Assert.Equal(rm, intensity.AverageIntensity);
                }
            }
        }



    }
}
