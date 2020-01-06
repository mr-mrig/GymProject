using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class WorkoutSessionAggregateTest
    {



        internal enum WorkoutConstructors : byte
        {
            Schedule = 0,
            FullTrack,
            NumberOfElements,
        }


        public const int ntests = 100;



        [Fact]
        public void WorkoutSessionFail()
        {
            int ntests = 50;

            bool isTransient, faked = false;
            int workUnitsNum = 5;

            float fakeChance = 0.33f;

            IList<WorkUnitEntity> wusFirstNull = new List<WorkUnitEntity>();
            IList<WorkUnitEntity> wusLastNull = new List<WorkUnitEntity>();
            IList<WorkUnitEntity> wusMiddleNull = new List<WorkUnitEntity>();
            IList<WorkUnitEntity> wusAllNull = new List<WorkUnitEntity>();
            IList<WorkUnitEntity> fakeWorkUnits;
            IList<WorkingSetEntity> fakeWorkingSets;

            for (int itest = 0; itest < ntests; itest++)
            {
                isTransient = itest % 2 == 0;

                uint? id = isTransient ? null : (uint?)1;

                DateTime start = RandomFieldGenerator.RandomDate(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(1));
                DateTime end = RandomFieldGenerator.RandomDate(start.AddMinutes(50), start.AddHours(4));
                DateTime planned = RandomFieldGenerator.RandomDate(start.AddDays(-5), start.AddDays(5));

                uint? workoutTemplateId = (uint?)RandomFieldGenerator.RandomIntNullable(1, 999999);

                // WorkUnits with gaps in consecutive numbers
                IList<WorkUnitEntity> wusPnumStartsFromOne = new List<WorkUnitEntity>();
                IList<WorkUnitEntity> wusPnumWithGaps = new List<WorkUnitEntity>();
                faked = false;

                for (uint i = 0; i < workUnitsNum; i++)
                {
                    wusPnumStartsFromOne.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(i + 1, i + 1, isTransient));

                    if (RandomFieldGenerator.RollEventWithProbability(fakeChance))
                    {
                        faked = true;
                        wusPnumWithGaps.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(i + 1, i + 100, isTransient));
                    }
                    else
                        wusPnumWithGaps.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(i + 1, i, isTransient));
                }

                if (!faked)
                    wusPnumWithGaps[2] = WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(2, 100, isTransient);

                // Constructor1
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutSessionRoot.TrackWorkout(id, start, end, planned, workoutTemplateId, wusPnumStartsFromOne));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutSessionRoot.TrackWorkout(id, start, end, planned, workoutTemplateId, wusPnumWithGaps));

                // WS failures: invalid repetitions
                fakeWorkUnits = new List<WorkUnitEntity>();
                faked = false;

                for (int iwu = 0; iwu < workUnitsNum; iwu++)
                {
                    fakeWorkUnits.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit((uint)iwu + 1, (uint)iwu, isTransient));

                    if (RandomFieldGenerator.RollEventWithProbability(fakeChance))
                    {
                        int toFakePNum = RandomFieldGenerator.RandomInt(0, fakeWorkUnits[iwu].WorkingSets.Count - 1);

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => fakeWorkUnits[iwu].ReviseWorkingSetRepetitions((uint)toFakePNum,
                            RandomFieldGenerator.RollEventWithProbability()
                            ? WSRepetitionsValue.TrackAMRAP()
                            : WSRepetitionsValue.TrackNotSetRepetitions()));
                    }
                }


                // WorkUnit failures: NULL excercise
                fakeWorkUnits = new List<WorkUnitEntity>();

                for (uint iwu = 0; iwu < workUnitsNum; iwu++)
                    Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                        fakeWorkUnits.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(iwu + 1, iwu, isTransient, excerciseId: null)));


                //// WorkUnit failures: Empty Working Sets
                //fakeWorkUnits = new List<WorkUnitEntity>();
                //fakeWorkingSets = new List<WorkingSetEntity>();

                //for (uint iwu = 0; iwu < workUnitsNum; iwu++)
                //    fakeWorkUnits.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(iwu + 1, iwu, isTransient));

                //Assert.Throws<TrainingDomainInvariantViolationException>(()
                //    => fakeWorkUnits[(int)fakeWorkUnit.ProgressiveNumber] = WorkUnitEntity
                //        .TrackExcercise(fakeWorkUnit.Id, fakeWorkUnit.ProgressiveNumber, fakeWorkUnit.ExcerciseId, fakeWorkingSets, fakeWorkUnit.UserRating));


                // WorkUnit failures: Non consecutive Working Sets
                fakeWorkUnits = new List<WorkUnitEntity>();
                fakeWorkingSets = new List<WorkingSetEntity>();

                for (uint iwu = 0; iwu < workUnitsNum; iwu++)
                    fakeWorkUnits.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(iwu + 1, iwu, isTransient));

                WorkUnitEntity fakeWorkUnit = fakeWorkUnits[RandomFieldGenerator.RandomInt(0, fakeWorkUnits.Count - 1)];
                fakeWorkingSets = fakeWorkUnits[(int)fakeWorkUnit.ProgressiveNumber].WorkingSets.ToList();
                fakeWorkingSets.RemoveAt(RandomFieldGenerator.RandomInt(0, fakeWorkingSets.Count - 2));     // -2 beacause removing the last one won't raise any error

                Assert.Throws<TrainingDomainInvariantViolationException>(()
                    => fakeWorkUnits[(int)fakeWorkUnit.ProgressiveNumber] = WorkUnitEntity
                        .TrackExcercise(fakeWorkUnit.Id, fakeWorkUnit.ProgressiveNumber, fakeWorkUnit.ExcerciseId, fakeWorkingSets, fakeWorkUnit.UserRating));
            }
        }




        [Fact]
        public void WorkoutSessionFullTest()
        {
            bool isTransient;
            float fakeChance = 0.1f;

            int workUnitsToAddNumber = 2;
            int workUnitsToRemoveNumber = 2;
            int workUnitsToChangeNumber = 2;

            List<WorkUnitEntity> workUnits;
            WorkoutSessionRoot workout;
            WorkUnitEntity toAdd;
            WorkUnitEntity workUnitAdded;

            DateTime planned;

            for (int itest = 0; itest < ntests; itest++)
            {
                isTransient = RandomFieldGenerator.RollEventWithProbability();

                uint? id = isTransient ? null : (uint?)1;

                WorkoutConstructors constructorId = (WorkoutConstructors)RandomFieldGenerator.RandomInt
                    (0, (int)WorkoutConstructors.NumberOfElements - 1);

                workout = BuildAndCheckWorkout(id, isTransient, constructorId);

                // Change Workout
                if (RandomFieldGenerator.RollEventWithProbability(fakeChance))

                    Assert.Throws<InvalidOperationException>(()
                        => workout.ScheduleToDate(RandomFieldGenerator.RandomDate(DateTime.Now.AddYears(-1), 100)));
                else
                {
                    if(RandomFieldGenerator.RollEventWithProbability(fakeChance))
                    {
                        planned = RandomFieldGenerator.RandomDate(DateTime.Now.AddDays(-500), DateTime.Now.AddDays(-1));
                        Assert.Throws<InvalidOperationException>(() => workout.ScheduleToDate(planned));
                    }
                    else
                    {
                        planned = RandomFieldGenerator.RandomDate(DateTime.Now, DateTime.Now.AddDays(300));
                        workout.ScheduleToDate(planned);
                        Assert.Equal(planned, workout.PlannedDate);
                    }
                }

                DateTime end = RandomFieldGenerator.RandomDate(DateTime.Now.AddDays(-30), 50);
                workout.Unschedule();
                workout.FinishWorkout(end);

                Assert.Null(workout.PlannedDate);
                Assert.Equal(end, workout.EndTime);

                // Add Work Units
                workUnits = workout.WorkUnits.ToList();
                int wuCount = workout.WorkUnits.Count;

                bool trackingJustCompletedWorkingSets = RandomFieldGenerator.RollEventWithProbability();

                for (uint iwu = 0; iwu < workUnitsToAddNumber; iwu++)
                {
                    uint wuId = isTransient ? 1
                        : (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 999999, workUnits.Select(x => (int)x.Id.Value));

                    toAdd = WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(wuId, (uint)wuCount++, isTransient);
                    workUnits.Add(toAdd);

                    if(trackingJustCompletedWorkingSets)
                    {
                        workout.StartTrackingExcercise(toAdd.ExcerciseId);
                        workUnitAdded = workout.CloneWorkUnit(toAdd.ProgressiveNumber);

                        Assert.Equal(toAdd.ExcerciseId, workUnitAdded.ExcerciseId);
                        Assert.Empty(workUnitAdded.WorkingSets);

                        foreach (WorkingSetEntity ws in toAdd.WorkingSets)
                        {
                            if(RandomFieldGenerator.RollEventWithProbability())
                                workout.TrackWorkingSet(toAdd.ProgressiveNumber, ws);
                            else
                                workout.TrackWorkingSet(toAdd.ProgressiveNumber, ws.Repetitions, ws.Load);
                        }

                        workUnitAdded = workout.CloneWorkUnit(toAdd.ProgressiveNumber);
                        CheckWorkUnit(toAdd, workUnitAdded, isTransient, true);
                    }
                    else
                    {
                        if(!isTransient && workout.WorkUnits.Count > 0
                            && RandomFieldGenerator.RollEventWithProbability(fakeChance))
                        {
                            Assert.Throws<ArgumentException>(() =>
                                workout.TrackExcercise(workout.CloneWorkUnit((uint)RandomFieldGenerator.RandomInt(0, workout.WorkUnits.Count - 1))));

                            wuCount--;
                        }
                        else
                        {
                            workout.TrackExcercise(toAdd);
                            workUnitAdded = workout.CloneWorkUnit(toAdd.ProgressiveNumber);

                            CheckWorkUnit(toAdd, workUnitAdded, isTransient);
                        }
                    }
                }

                // Revise Work Units
                for (int iwu = 0; iwu < workUnitsToChangeNumber; iwu++)
                {
                    uint pnumToChange = (uint)RandomFieldGenerator.RandomInt(0, workout.WorkUnits.Count - 1);
                    uint destPnum = (uint)RandomFieldGenerator.RandomInt(0, workout.WorkUnits.Count - 1);

                    RatingValue rating = RatingValue.Rate(RandomFieldGenerator.RandomFloat(0, 5, 1));
                    workout.RatePerformance(pnumToChange, rating);
                    Assert.Equal(rating, workout.CloneWorkUnit(pnumToChange).UserRating);

                    int newWorkingSetNum = RandomFieldGenerator.RandomInt(1, 3);
                    WorkingSetEntity ws;
                    uint wsId;

                    for (int iws = 0; iws < newWorkingSetNum; iws++)
                    {
                        WorkUnitEntity originalWorkUnit = workout.CloneWorkUnit(pnumToChange);

                        if (RandomFieldGenerator.RollEventWithProbability(fakeChance))
                        {   
                            // Duplicate Id
                            if(!isTransient)
                            {
                                wsId = (uint)RandomFieldGenerator.ChooseAmong(originalWorkUnit.WorkingSets.Where(x => x.Id.HasValue).Select(x => (int?)x.Id ?? 1));

                                ws = WorkoutSessionAggregateBuilder.BuildRandomWorkingSet(wsId, originalWorkUnit.WorkingSets.Count, isTransient);
                                Assert.Throws<ArgumentException>(() => workout.TrackWorkingSet(pnumToChange, ws));
                            }

                            // Non consecutive PNums
                            wsId = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 999999, originalWorkUnit.WorkingSets.Select(x => (int)(x.Id ?? 1)));

                            ws = WorkoutSessionAggregateBuilder.BuildRandomWorkingSet(wsId, originalWorkUnit.WorkingSets.Count * 10, isTransient);
                            Assert.Throws<TrainingDomainInvariantViolationException>(() => workout.TrackWorkingSet(pnumToChange, ws));

                            // Null input
                            Assert.Throws<ArgumentNullException>(() => workout.TrackWorkingSet(pnumToChange, null));

                            // Skip next iterations to avoid further Invariant Violations
                            iwu = workUnitsToChangeNumber;
                            break;
                        }
                        else
                        {
                            // Valid Id
                            wsId = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 999999, originalWorkUnit.WorkingSets.Select(x => (int)(x.Id ?? 1)));

                            ws = WorkoutSessionAggregateBuilder.BuildRandomWorkingSet(wsId, originalWorkUnit.WorkingSets.Count, isTransient);
                            workout.TrackWorkingSet(pnumToChange, ws);

                            CheckWorkingSets(originalWorkUnit.WorkingSets.Union(new List<WorkingSetEntity>() { ws }),
                                workout.CloneWorkUnit(pnumToChange).WorkingSets, isTransient);
                        }
                    }

                    //workout.UntrackWorkingSet();
                    //workout.WriteWorkingSetNote();
                    //workout.ReviseWorkingSetLoad();
                    //workout.ReviseWorkingSetRepetitions();



                    //workout.MoveWorkUnitToNewProgressiveNumber(pnumToChange, destPnum);

                    //throw new NotImplementedException();
                }

                // Remove Work Units
            }



        }





        #region Support Check Functions

        internal static void CheckWorkUnit(WorkUnitEntity left, WorkUnitEntity right, bool isTransient, bool trackingJustCompletedWorkingSets = false)
        {
            Assert.Equal(left.ExcerciseId, right.ExcerciseId);
            Assert.Equal(left.ProgressiveNumber, right.ProgressiveNumber);
            Assert.Equal(left.TrainingVolume, right.TrainingVolume);

            if(trackingJustCompletedWorkingSets)
            {
                Assert.True(left.WorkingSets.Select(x => x.ProgressiveNumber)
                    .SequenceEqual(right.WorkingSets.Select(x => x.ProgressiveNumber)));

                Assert.True(left.WorkingSets.Select(x => x.Load)
                    .SequenceEqual(right.WorkingSets.Select(x => x.Load)));

                Assert.True(left.WorkingSets.Select(x => x.Repetitions)
                    .SequenceEqual(right.WorkingSets.Select(x => x.Repetitions)));
            }
            else
            {
                Assert.Equal(left.UserRating, right.UserRating);

                Assert.True(left.WorkingSets.Select(x => x.NoteId)
                    .SequenceEqual(right.WorkingSets.Select(x => x.NoteId)));

                if (isTransient)
                {
                    Assert.True(left.WorkingSets.Select(x => x.ProgressiveNumber)
                        .SequenceEqual(right.WorkingSets.Select(x => x.ProgressiveNumber)));

                    Assert.True(left.WorkingSets.Select(x => x.Load)
                        .SequenceEqual(right.WorkingSets.Select(x => x.Load)));

                    Assert.True(left.WorkingSets.Select(x => x.Repetitions)
                        .SequenceEqual(right.WorkingSets.Select(x => x.Repetitions)));
                }
                else
                {
                    Assert.Equal(left.Id, right.Id);
                    Assert.True(left.WorkingSets.SequenceEqual(right.WorkingSets));
                }
            }

            CheckTrainingParameters(left.WorkingSets, right.TrainingVolume);
        }


        internal static void CheckWorkingSets(IEnumerable<WorkingSetEntity> left, IEnumerable<WorkingSetEntity> right, bool isTransient)
        {
            if (!isTransient)
                Assert.True(left.Select(x => x.Id).SequenceEqual(right.Select(x => x.Id)));

            Assert.True(left.Select(x => x.NoteId)
                .SequenceEqual(right.Select(x => x.NoteId)));

            Assert.True(left.Select(x => x.ProgressiveNumber)
                .SequenceEqual(right.Select(x => x.ProgressiveNumber)));

            Assert.True(left.Select(x => x.Load)
                .SequenceEqual(right.Select(x => x.Load)));

            Assert.True(left.Select(x => x.Repetitions)
                .SequenceEqual(right.Select(x => x.Repetitions)));
        }


        internal static void CheckTrainingParameters(IEnumerable<WorkingSetEntity> srcWorkingSets, TrainingVolumeParametersValue volume)
        {
            // Get the expected training parameters
            int totalReps = srcWorkingSets.Sum(x => x.ToRepetitions());
            int totalWs = srcWorkingSets.Count();
            float totalWorkload = (float)Math.Round(srcWorkingSets.Sum(x => x.ToWorkload().Value), 2);


            // Convert to Main Effort Type
            if (totalWs == 0)
            {
                // Check Volume
                Assert.Equal(totalReps, volume.TotalReps);
                Assert.Equal(totalWs, volume.TotalWorkingSets);
                Assert.Equal(0, volume.TotalWorkload.Value);
                Assert.Equal(0, volume.GetAverageRepetitions(), 1);
                Assert.Equal(0, volume.GetAverageWorkloadPerSet().Value);
            }
            else
            {
                // Check Volume
                Assert.Equal(totalReps, volume.TotalReps);
                Assert.Equal(totalWs, volume.TotalWorkingSets);
                Assert.Equal(totalWorkload, volume.TotalWorkload.Value);
                Assert.Equal((float)totalReps / (float)totalWs, volume.GetAverageRepetitions(), 1);

                StaticUtils.CheckConversions((float)totalWorkload / (float)totalWs, volume.GetAverageWorkloadPerSet().Value);
            }
        }



        internal WorkoutSessionRoot BuildAndCheckWorkout(uint? id, bool isTransient, WorkoutConstructors constructorId = WorkoutConstructors.FullTrack)
        {
            WorkoutSessionRoot workout;
            List<WorkUnitEntity> workUnits = new List<WorkUnitEntity>();

            int workUnitsMin = 2, workUnitsMax = 8;

            DateTime start = RandomFieldGenerator.RandomDate(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(1));
            DateTime end = RandomFieldGenerator.RandomDate(start.AddMinutes(50), start.AddHours(4));
            DateTime planned = RandomFieldGenerator.RandomDate(start.AddDays(-5), start.AddDays(5));

            uint? workoutTemplateId = (uint?)RandomFieldGenerator.RandomIntNullable(1, 999999);

            switch (constructorId)
            {
                case WorkoutConstructors.Schedule:

                    if (isTransient)
                        workout = WorkoutSessionRoot.ScheduleWorkout(planned, workoutTemplateId);
                    else
                        workout = WorkoutSessionRoot.ScheduleWorkout(id, planned, workoutTemplateId);

                    Assert.Equal(planned, workout.PlannedDate);
                    Assert.Equal(workoutTemplateId, workout.WorkoutTemplateId);
                    Assert.Null(workout.StartTime);
                    Assert.Null(workout.EndTime);
                    Assert.Empty(workout.WorkUnits);
                    Assert.Equal(0, workout.TrainingVolume.TotalReps);
                    Assert.Equal(0, workout.TrainingVolume.TotalWorkingSets);
                    Assert.Equal(0, workout.TrainingVolume.TotalWorkload.Value);

                    break;


                default:

                    int workUnitsNum = RandomFieldGenerator.RandomInt(workUnitsMin, workUnitsMax);

                    for (uint iwu = 0; iwu < workUnitsNum; iwu++)
                    {
                        uint strongId = isTransient ? 1
                            : (uint)((id.Value - 1) * workUnitsNum + iwu + 1);      // Smallest possible

                        workUnits.Add(WorkoutSessionAggregateBuilder.BuildRandomWorkUnit(strongId, iwu, isTransient));
                    }

                    if (isTransient)
                        workout = WorkoutSessionRoot.TrackWorkout(null, start, end, planned, workoutTemplateId, workUnits);
                    else
                        workout = WorkoutSessionRoot.TrackWorkout(id, start, end, planned, workoutTemplateId, workUnits);

                    Assert.Equal(planned, workout.PlannedDate);
                    Assert.Equal(workoutTemplateId, workout.WorkoutTemplateId);
                    Assert.Equal(start, workout.StartTime);
                    Assert.Equal(end, workout.EndTime);

                    if(isTransient)
                    {
                        Assert.Equal(workUnits.Count, workout.WorkUnits.Count);
                        Assert.True(workout.WorkUnits.Select(x => x.ExcerciseId).SequenceEqual(workUnits.Select(x => x.ExcerciseId)));
                        Assert.True(workout.WorkUnits.Select(x => x.ProgressiveNumber).SequenceEqual(workUnits.Select(x => x.ProgressiveNumber)));
                        Assert.True(workout.WorkUnits.Select(x => x.WorkingSets.Count).SequenceEqual(workUnits.Select(x => x.WorkingSets.Count)));
                    }
                    else
                        Assert.True(workout.WorkUnits.SequenceEqual(workUnits));

                    CheckTrainingParameters(workUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume);

                    break;
            }
            return workout;
        }
        #endregion
    }


}