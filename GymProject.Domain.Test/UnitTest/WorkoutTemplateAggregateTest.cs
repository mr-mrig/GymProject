using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class WorkoutTemplateAggregateTest
    {


        public int ntests = 500;



        [Fact]
        public void WorkoutFail()
        {

            bool isTransient;
            ntests = 100;

            uint? id = 1;
            uint validPnum = 1;
            uint validWeekId = 100;

            IList<WorkUnitTemplateEntity> wusFirstNull = new List<WorkUnitTemplateEntity>();
            IList<WorkUnitTemplateEntity> wusLastNull = new List<WorkUnitTemplateEntity>();
            IList<WorkUnitTemplateEntity> wusMiddleNull = new List<WorkUnitTemplateEntity>();

            for (int itest = 0; itest < ntests; itest++)
            {
                isTransient = itest % 2 == 0;

                wusFirstNull.Add(null);

                for (uint i = 0; i < 5; i++)
                {
                    wusFirstNull.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i, isTransient));
                    wusLastNull.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i, isTransient));
                    if (i == 1)
                        wusMiddleNull.Add(null);
                    else
                        wusMiddleNull.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i, isTransient));
                }

                wusLastNull.Add(null);

                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, validWeekId, validPnum, wusFirstNull, string.Empty));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, validWeekId, validPnum, wusMiddleNull, string.Empty));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, validWeekId, validPnum, wusLastNull, string.Empty));

                IList<WorkUnitTemplateEntity> wusPnumStarts1 = new List<WorkUnitTemplateEntity>();
                IList<WorkUnitTemplateEntity> wusPnumGap = new List<WorkUnitTemplateEntity>();

                for (uint i = 0; i < 5; i++)
                {
                    wusPnumStarts1.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i + 1, isTransient));

                    if (i == 1)
                        wusPnumGap.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i + 100, isTransient));
                    else
                        wusPnumGap.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i + 1, isTransient));
                }

                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, validWeekId, validPnum, wusPnumStarts1, string.Empty));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, validWeekId, validPnum, wusPnumGap, string.Empty));
            }
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

            float transientEntityProbability = 0.1f;

            bool isTransient;
            WorkoutTemplateRoot workout;
            WorkUnitTemplateEntity wu;
            uint? woId = 1;
            uint progressiveNumber = 0;

            for (int itest = 0; itest < ntests; itest++)
            {
                // Generate random parameters
                int workUnitsNum = RandomFieldGenerator.RandomInt(wuMin, wuMax);
                string woName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax);
                WeekdayEnum specificDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.Sunday.Id));
                List<WorkUnitTemplateEntity> initialWus = new List<WorkUnitTemplateEntity>();
                List<WorkUnitTemplateEntity> wus = new List<WorkUnitTemplateEntity>();
                List<uint?> wuIds = new List<uint?>();

                isTransient = RandomFieldGenerator.RollEventWithProbability(transientEntityProbability);

                // Initial WUs
                int initialWuNum = RandomFieldGenerator.RandomInt(initialWuMin, initialWuMax);
                uint? idnum = (uint?)(itest * ntests);
                uint pnum = 0;
                uint? weekId = (uint?)RandomFieldGenerator.RandomInt(500, 1000);

                for (int iwu = 0; iwu < initialWuNum; iwu++)
                {
                    // Act as the ID has been retrieved from the DB
                    idnum++;
                    wuIds.Add(idnum);
                    wu = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(wuIds.Last().Value, pnum++, false);   // Initial WUs are never transient

                    initialWus.Add(wu);
                    wus.Add(wu);
                }

                // WO with initial WUs
                if (isTransient)
                    workout = WorkoutTemplateRoot.PlanTransientWorkout(progressiveNumber, initialWus, woName, specificDay);  // Transient
                else
                    workout = WorkoutTemplateRoot.PlanWorkout(woId, weekId, progressiveNumber, initialWus, woName, specificDay);     // Persistent

                // No WUs loaded from the DB -> start the sequence
                if (initialWuNum == 0)
                    idnum = 0;

                // Add WUs
                for (int iwu = 0; iwu < workUnitsNum; iwu++)
                {
                    idnum++;
                    wuIds.Add(idnum);
                    wu = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(wuIds.Last().Value, pnum++, isTransient);

                    if (isTransient)
                        workout.PlanTransientExcercise(wu.ExcerciseId, wu.WorkingSets.ToList(), wu.LinkingIntensityTechniqueId, wu.WorkUnitNoteId);
                    else
                        workout.PlanExcercise(wu);

                    wus.Add(wu);
                }

                // Check WO
                if (!isTransient)
                    Assert.Equal(weekId, workout.TrainingWeekId);
                Assert.Equal(woName, workout.Name);
                Assert.Equal(specificDay, workout.SpecificWeekday);
                Assert.Equal(initialWuNum + workUnitsNum, workout.WorkUnits.Count());

                // Check WUs
                foreach (WorkUnitTemplateEntity iwu in wus)
                    CheckWorkUnit(iwu, workout, isTransient);

                // Modify WO
                string newName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax, true, 0.05f);
                uint newPnum = (uint)RandomFieldGenerator.RandomInt(pnumMin, pnumMax);
                WeekdayEnum newDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.AllTheWeek));

                workout.GiveName(newName);
                workout.MoveToNewProgressiveNumber(newPnum);
                workout.ScheduleToSpecificDay(newDay);

                Assert.Equal(newName, workout.Name);
                Assert.Equal(newPnum, workout.ProgressiveNumber);
                Assert.Equal(newDay, workout.SpecificWeekday);

                workout.UnscheduleSpecificDay();
                Assert.Equal(WeekdayEnum.Generic, workout.SpecificWeekday);


                // Modify WUs
                foreach (WorkUnitTemplateEntity iwu in workout.WorkUnits)
                    CheckWorkUnitChanges(workout, iwu, isTransient);


                // Modify PNums -> Keep it separate from the Modify WUs block as changing the Pnums will mess with the other tests
                foreach (WorkUnitTemplateEntity iwu in workout.WorkUnits)
                {
                    uint srcPnum = iwu.ProgressiveNumber;
                    uint destPnum = (uint)RandomFieldGenerator.ChooseAmong<int>(
                        workout.WorkUnits.Select(x => (int)x.ProgressiveNumber).ToList());

                    WorkUnitTemplateEntity swappedWorkUnit = workout.CloneWorkUnit(destPnum);

                    workout.MoveWorkUnitToNewProgressiveNumber(srcPnum, destPnum);

                    WorkUnitTemplateEntity toCheck = workout.CloneWorkUnit(destPnum);
                    swappedWorkUnit.MoveToNewProgressiveNumber(srcPnum);    // Simulate the move on the original WU copy

                    CheckWorkUnitsBasicCompare(swappedWorkUnit, workout.CloneWorkUnit(srcPnum), isTransient);
                    CheckWorkUnitsBasicCompare(toCheck, workout.CloneWorkUnit(destPnum), isTransient);
                }


                // Remove WUs
                int nWorkUnits = workout.WorkUnits.Count();

                for (uint iwu = 0; iwu < wuRemoveNum; iwu++)
                {
                    uint toRemovePnum = RandomFieldGenerator.ChooseAmong(workout.WorkUnits.Select(x => x.ProgressiveNumber).ToList());

                    WorkUnitTemplateEntity removed = workout.CloneWorkUnit(toRemovePnum);
                    WorkUnitTemplateEntity previous = workout.CloneWorkUnit(toRemovePnum - 1);    // Before removing

                    workout.UnplanExcercise(toRemovePnum);

                    Assert.Equal(nWorkUnits - iwu - 1, workout.WorkUnits.Count());

                    if (workout.WorkUnits.Count() > 0)
                    {
                        Assert.True(workout.WorkUnits.Select(x => (int)x.ProgressiveNumber).
                            SequenceEqual(Enumerable.Range(0, workout.WorkUnits.Count())));

                        Assert.DoesNotContain(removed, workout.WorkUnits);
                        // Check pnum boundaries
                        Assert.Equal(0, (int)workout.WorkUnits.OrderBy(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);
                        Assert.Equal(workout.WorkUnits.Count() - 1, (int)workout.WorkUnits.OrderByDescending(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);

                        //CheckTrainingParameters(workout.WorkUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity, null);

                        // Check linked WUs
                        if (toRemovePnum > 0)
                        {
                            //if (previous.LinkedWorkUnitId != null                   // The removed one was not linked to anything
                            //    || toRemovePnum == workout.WorkUnits.Count - 1)     // The remove one was the last one
                            //{
                            //    // If the removed one was not linked to anything, then the previous one should not be linked as well
                            //    if (removed.LinkedWorkUnitId == null)
                            //    {
                            //        Assert.Null(previous.LinkedWorkUnitId);
                            //        Assert.Null(previous.LinkingIntensityTechniqueId);
                            //    }
                            //    else
                            //    {
                            //        // Otherwise it should be linked to the one that was linked to the removed one
                            //        Assert.Equal(workout.CloneWorkUnit(toRemovePnum).Id, workout.CloneWorkUnit(toRemovePnum - 1).LinkedWorkUnitId);
                            //        Assert.Equal(removed.LinkingIntensityTechniqueId, workout.CloneWorkUnit(toRemovePnum - 1).LinkingIntensityTechniqueId);
                            //    }
                            //}
                            Assert.Null(previous.LinkingIntensityTechniqueId);
                        }

                        //// Check linked WUs
                        //if (toRemovePnum > 0)
                        //{
                        //    if (previous.LinkedWorkUnit?.LinkedWorkId != null       // The removed one was not linked to anything
                        //        || toRemovePnum == workout.WorkUnits.Count - 1)     // The remove one was the last one
                        //    {
                        //        // If the removed one was not linked to anything, then the previous one should not be linked as well
                        //        if (removed.LinkedWorkUnit?.LinkedWorkId == null)
                        //            Assert.Null(previous.LinkedWorkUnit);
                        //        else
                        //        {
                        //            // Otherwise it should be linked to the one that was linked to the removed one
                        //            Assert.Equal(workout.CloneWorkUnit(toRemovePnum).Id, workout.CloneWorkUnit(toRemovePnum - 1).LinkedWorkUnit.LinkedWorkId);
                        //            Assert.Equal(removed.LinkedWorkUnit.LinkingIntensityTechniqueId, workout.CloneWorkUnit(toRemovePnum - 1).LinkedWorkUnit.LinkingIntensityTechniqueId);
                        //        }
                        //    }
                        //}
                    }
                }

                // Add WUs
                uint toAddId;
                bool wasAdded = false;
                WorkUnitTemplateEntity toAdd;
                nWorkUnits = workout.WorkUnits.Count();

                for (int iwu = 0; iwu < wuAddNum; iwu++)
                {
                    uint? excerciseId = (uint?)(RandomFieldGenerator.RandomInt(500, 1000));
                    uint? noteId = (uint?)(RandomFieldGenerator.RandomInt(1, 500));


                    if (isTransient)
                    {
                        wasAdded = true;
                        toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(1, (uint)workout.WorkUnits.Count(), isTransient);
                        workout.PlanTransientExcercise(toAdd.ExcerciseId, toAdd.WorkingSets.ToList(), toAdd.LinkingIntensityTechniqueId, toAdd.WorkUnitNoteId);
                    }
                    else
                    {
                        // Trying to add an already present item
                        if (nWorkUnits > 0 && RandomFieldGenerator.RollEventWithProbability(0.1f))
                        {
                            wasAdded = false;
                            toAddId = (uint)RandomFieldGenerator.ChooseAmong(
                                workout.WorkUnits.Select(x => x.Id).ToList());


                            toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(toAddId, (uint)workout.WorkUnits.Count(), isTransient);
                            Assert.Throws<ArgumentException>(() => workout.PlanExcercise(toAdd));
                        }
                        else
                        {
                            wasAdded = true;

                            toAddId = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 25,    // Not too big right boundary to avoid overflow on derived Ids
                                workout.WorkUnits.Select(x => (int)x.Id).ToList());

                            toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(toAddId, (uint)workout.WorkUnits.Count(), isTransient);
                            workout.PlanExcercise(toAdd);
                        }
                    }

                    if (wasAdded)
                    {
                        WorkUnitTemplateEntity added = workout.CloneWorkUnit(toAdd.ProgressiveNumber);      // Id is different, not PNum

                        Assert.Equal(++nWorkUnits, workout.WorkUnits.Count());
                        CheckWorkUnitsBasicCompare(toAdd, added, isTransient);
                    }
                    else
                        Assert.Equal(nWorkUnits, workout.WorkUnits.Count());

                    //CheckTrainingParameters(workout.WorkUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity, null);
                }
            }
        }


        #region Support Check Functions
        internal static void CheckTrainingParameters(IEnumerable<WorkingSetTemplateEntity> srcWorkingSets,
            TrainingVolumeParametersValue volume, TrainingDensityParametersValue density, TrainingIntensityParametersValue intensity, TrainingEffortTypeEnum mainEffortType = null)
        {
            TrainingEffortValue avgEffort = null;
            float intensityPercentageTolerance = 0.025f;
            float rpeAndRmTolerance = 0.07f;      // Smaller numbers -> Higher tolerance. IE: 5RPE Vs 5.5RPE must be considered equivalent

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
                    if (avgEffort.IsIntensityPercentage())
                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: intensityPercentageTolerance);    // Rounding error can be moderately high because of multiple conversions

                    else
                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: rpeAndRmTolerance);    // Rounding error can be moderately high because of multiple conversions
                }
            }
        }


        internal static void CheckWorkUnitSets(WorkUnitTemplateEntity wu, IEnumerable<WorkingSetTemplateEntity> ws, bool isTransient, TrainingEffortTypeEnum effortType = null, bool wsFullCheck = true)
        {
            foreach (WorkingSetTemplateEntity wsCheck in ws)
            {
                if (!isTransient)
                    Assert.Contains(wsCheck, wu.WorkingSets);

                if (wsFullCheck)
                    CheckWorkingSet(wsCheck, wu.CloneWorkingSet(wsCheck.ProgressiveNumber), isTransient, wu);
            }
            //CheckTrainingParameters(wu.WorkingSets, wu.TrainingVolume, wu.TrainingDensity, wu.TrainingIntensity, effortType);
        }


        internal static void CheckWorkingSet(WorkingSetTemplateEntity left, WorkingSetTemplateEntity right, bool isTransient, WorkUnitTemplateEntity wunit = null)
        {
            if (!isTransient)
                Assert.Equal(left, right);

            Assert.Equal(left.Repetitions, right.Repetitions);
            Assert.Equal(left.Rest, right.Rest);
            Assert.Equal(left.Tempo, right.Tempo);
            Assert.Equal(left.Effort, right.Effort);

            // Left and right must have the same Int techniques
            if (wunit == null)
            {
                Assert.Equal(left.IntensityTechniqueIds, right.IntensityTechniqueIds);

                foreach (uint? idLeft in left.IntensityTechniqueIds)
                    Assert.Contains(idLeft, right.IntensityTechniqueIds);
            }
            else
            {
                // Right has the left Int techniques + the WU ones
                Assert.Equal(left.IntensityTechniqueIds.Count + (wunit.HasLinkedUnit() ? 1 : 0), right.IntensityTechniqueIds.Count);

                foreach (uint? idLeft in left.IntensityTechniqueIds)
                    Assert.Contains(idLeft, right.IntensityTechniqueIds);

                if(wunit.HasLinkedUnit())
                    Assert.Contains(wunit.LinkingIntensityTechniqueId, right.IntensityTechniqueIds);
            }
        }


        internal static void CheckWorkUnitChanges(WorkoutTemplateRoot workout, WorkUnitTemplateEntity workUnit, bool isTransient)
        {
            int excerciseIdMin = 1, excerciseIdMax = 200;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;
            int noteIdMin = 25, noteIdMax = 500;
            int wsToRemoveNum = 2, wsToAddNum = 2, wsToChangeNum = 2;

            float failInsertionProbability = 0.05f;
            float failRemovalProbability = 0.05f;


            // Need to clone the WU after every change
            WorkUnitTemplateEntity toCheck;
            WorkUnitTemplateEntity toCompare;
            WorkingSetTemplateEntity toAdd;

            uint? newExcerciseId = (uint?)(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax));
            uint? newNoteId = (uint?)(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));

            workout.AssignWorkUnitExcercise(workUnit.ProgressiveNumber, newExcerciseId);
            workout.AttachWorkUnitNote(workUnit.ProgressiveNumber, newNoteId);

            // Check WU changes
            Assert.Equal(newNoteId, workout.CloneWorkUnit(workUnit.ProgressiveNumber).WorkUnitNoteId);
            Assert.Equal(newExcerciseId, workout.CloneWorkUnit(workUnit.ProgressiveNumber).ExcerciseId);

            workout.DetachWorkUnitNote(workUnit.ProgressiveNumber);
            Assert.Null(workout.CloneWorkUnit(workUnit.ProgressiveNumber).WorkUnitNoteId);

            // Link To new Work Unit
            uint intTechniqueId;

            if (workUnit.ProgressiveNumber == workout.WorkUnits.Count - 1)
            {
                // The last WU cannot be linked to anything
                Assert.Throws<InvalidOperationException>(() => workout.LinkWorkUnits(workUnit.ProgressiveNumber, 100));
            }
            else
            {
                intTechniqueId = (uint)RandomFieldGenerator.RandomInt(intTechniqueIdMin, intTechniqueIdMax);

                if (isTransient)
                    // Cannot link to transient WUs
                    //Assert.Throws<InvalidOperationException>(() => workout.LinkWorkUnits(workUnit.ProgressiveNumber, intTechniqueId));
                    ;
                else
                {
                    workout.LinkWorkUnits(workUnit.ProgressiveNumber, intTechniqueId);

                    toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);
                    toCompare = workout.CloneWorkUnit(workUnit.ProgressiveNumber + 1);

                    Assert.True(toCheck.HasLinkedUnit());
                    Assert.Equal(intTechniqueId, toCheck.LinkingIntensityTechniqueId);
                    //Assert.Equal(toCompare.Id, toCheck.LinkedWorkUnitId);

                    //foreach (WorkingSetTemplateEntity workingSet in toCheck.WorkingSets)
                    //{
                    //    Assert.Contains(intTechniqueId, workingSet.IntensityTechniqueIds);
                    //    Assert.DoesNotContain(intTechniqueId, workingSet.NonLinkingIntensityTechniqueIds);
                    //    Assert.Equal(intTechniqueId, workingSet.LinkedWorkingSet.LinkingIntensityTechniqueId);
                    //    Assert.Equal(toCompare.CloneWorkingSet(workingSet.ProgressiveNumber).Id, workingSet.LinkedWorkingSet.LinkedWorkId);
                    //}

                    //// Double link -> Overwrite
                    //if (RandomFieldGenerator.RollEventWithProbability())
                    //{
                    //    linkedWunitPnum = (uint)RandomFieldGenerator.RandomIntValueExcluded(0, workout.WorkUnits.Count - 1, (int)workUnit.ProgressiveNumber);
                    //    workout.LinkWorkUnits(workUnit.ProgressiveNumber, linkedWunitPnum, intTechniqueId);

                    //    Assert.True(workout.CloneWorkUnit(workUnit.ProgressiveNumber).HasLinkedUnit());
                    //    Assert.False(workout.CloneWorkUnit(linkedWunitPnum).HasLinkedUnit());
                    //    Assert.Equal(intTechniqueId, workout.CloneWorkUnit(workUnit.ProgressiveNumber).LinkedWorkUnit.LinkingIntensityTechniqueId);
                    //    Assert.Equal(linkedWunitPnum, workout.CloneWorkUnit(workUnit.ProgressiveNumber).LinkedWorkUnit.LinkedWorkId);
                    //}
                }
            }

            // Unlink
            workout.UnlinkWorkUnits(workUnit.ProgressiveNumber);
            //Assert.Null(workout.CloneWorkUnit(workUnit.ProgressiveNumber).LinkedWorkUnitId);
            Assert.Null(workout.CloneWorkUnit(workUnit.ProgressiveNumber).LinkingIntensityTechniqueId);

            //foreach (WorkingSetTemplateEntity ws in workout.CloneWorkUnit(workUnit.ProgressiveNumber).WorkingSets)
            //    Assert.Null(ws.LinkedWorkingSet);

            // Remove WSs
            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);
            List<WorkingSetTemplateEntity> originalSets = toCheck.WorkingSets.ToList();
            List<WorkingSetTemplateEntity> finalSets = toCheck.WorkingSets.ToList();
            int wsRemoveCounter = 0;

            for (int iws = 0; iws < wsToRemoveNum; iws++)
            {
                if (RandomFieldGenerator.RollEventWithProbability(failRemovalProbability))
                {
                    Assert.Throws<InvalidOperationException>(() => workout.RemoveWorkingSet(int.MaxValue, 0));
                    Assert.Throws<InvalidOperationException>(() => workout.RemoveWorkingSet(toCheck.ProgressiveNumber, int.MaxValue));
                    Assert.Throws<InvalidOperationException>(() => workout.RemoveWorkingSet(int.MaxValue, int.MaxValue));
                }
                else
                {
                    uint pnumToRemove = RandomFieldGenerator.ChooseAmong(toCheck.WorkingSets.Select(x => x.ProgressiveNumber));

                    if (toCheck.WorkingSets.Count > 1)
                    {
                        WorkingSetTemplateEntity previousWorkingSet = toCheck.CloneWorkingSet(pnumToRemove - 1);    // Before removing
                        WorkingSetTemplateEntity removed = workout.CloneWorkingSet(toCheck.ProgressiveNumber, pnumToRemove);

                        workout.RemoveWorkingSet(toCheck.ProgressiveNumber, pnumToRemove);
                        toCheck = workout.CloneWorkUnit(toCheck.ProgressiveNumber);

                        Assert.Equal(originalSets.Count - ++wsRemoveCounter, toCheck.WorkingSets.Count);
                        Assert.DoesNotContain(removed, toCheck.WorkingSets);

                        IEnumerable<WorkingSetTemplateEntity> wsRemoved = originalSets.Where(x => x.ProgressiveNumber == pnumToRemove);

                        finalSets = StaticUtils.ForceConsecutiveProgressiveNumbers(
                            finalSets.Where(x => x.ProgressiveNumber != pnumToRemove)).ToList();

                        Assert.True(toCheck.WorkingSets.Select(x => (int)x.ProgressiveNumber).
                            SequenceEqual(Enumerable.Range(0, toCheck.WorkingSets.Count)));

                        //CheckWorkUnitSets(toCheck, finalSets, isTransient, toCheck.TrainingIntensity.AverageIntensity.EffortType, false);

                        //// Check linked WSs
                        //if (pnumToRemove > 0)
                        //{
                        //    if (previousWorkingSet.LinkedWorkingSet?.LinkedWorkId != null       // The removed one was not linked to anything
                        //    || pnumToRemove == toCheck.WorkingSets.Count)                       // The remove one was the last one
                        //    {
                        //        // If the removed one was not linked to anything, then the previous one should not be linked as well
                        //        if (removed.LinkedWorkingSet?.LinkedWorkId == null)
                        //            Assert.Null(previousWorkingSet.LinkedWorkingSet);
                        //        else
                        //        {
                        //            // Otherwise it should be linked to the one that was linked to the removed one
                        //            Assert.Equal(toCheck.CloneWorkingSet(pnumToRemove).Id, toCheck.CloneWorkingSet(pnumToRemove - 1).LinkedWorkingSet.LinkedWorkId);
                        //            Assert.Equal(removed.LinkedWorkingSet.LinkingIntensityTechniqueId, toCheck.CloneWorkingSet(pnumToRemove - 1).LinkedWorkingSet.LinkingIntensityTechniqueId);
                        //        }
                        //    }
                        //}
                    }
                    //else
                    //    ;       // Removing the last WS will lead to an error in the following iterations: no check for failure
                }
            }


            // Add WSs
            bool duplicateId = false;
            int setsAddedCount = 0;

            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);    // Keep it updated

            originalSets = toCheck.WorkingSets.ToList();
            finalSets = toCheck.WorkingSets.ToList();

            for (int iws = 0; iws < wsToAddNum; iws++)
            {
                if (isTransient)
                {
                    duplicateId = false;
                    toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, finalSets.Count, isTransient, toCheck.GetMainEffortType());
                    workout.AddTransientWorkingSet(toCheck.ProgressiveNumber, toAdd.Repetitions, toAdd.Rest, toAdd.Effort, toAdd.Tempo, toAdd.IntensityTechniqueIds.ToList());
                }
                else
                {
                    duplicateId = RandomFieldGenerator.RollEventWithProbability(failInsertionProbability) && originalSets.Count > 0;

                    uint idToAdd = duplicateId
                        ? RandomFieldGenerator.ChooseAmong(finalSets.Select(x => (uint)x.Id))
                        : (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, finalSets.Select(x => (int)x.Id.Value));

                    toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToAdd, finalSets.Count, isTransient, toCheck.GetMainEffortType());
                }

                if (duplicateId)
                    Assert.Throws<ArgumentException>(() => workout.AddWorkingSet(toCheck.ProgressiveNumber, toAdd));
                else
                {
                    if (!isTransient)
                        workout.AddWorkingSet(toCheck.ProgressiveNumber, toAdd);

                    finalSets.Add(toAdd);
                    toCheck = workout.CloneWorkUnit(toCheck.ProgressiveNumber); // Keep it updated

                    Assert.Equal(originalSets.Count + ++setsAddedCount, toCheck.WorkingSets.Count);

                    finalSets = StaticUtils.ForceConsecutiveProgressiveNumbers(finalSets).ToList();

                    Assert.True(toCheck.WorkingSets.Select(x => (int)x.ProgressiveNumber).
                        SequenceEqual(Enumerable.Range(0, toCheck.WorkingSets.Count)));

                    //CheckWorkUnitSets(toCheck, finalSets, isTransient, toCheck.TrainingIntensity.AverageIntensity.EffortType, false);
                }
            }

            // Change Working Sets
            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);    // Keep it updated
            finalSets = toCheck.WorkingSets.ToList();

            for (int iws = 0; iws < wsToChangeNum; iws++)
            {
                uint toChangePnum = (uint)RandomFieldGenerator.RandomInt(0, workUnit.WorkingSets.Count - 1);

                WorkingSetTemplateEntity workingSetBefore = finalSets.SingleOrDefault(x => x.ProgressiveNumber == toChangePnum);

                if(workingSetBefore != null)
                {
                    WorkingSetTemplateEntity newWorkingSet = WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(
                        isTransient ? 1 : workingSetBefore.Id.Value, (int)toChangePnum, isTransient, TrainingEffortTypeEnum.RM);

                    workingSetBefore = newWorkingSet;

                    workout.ReviseWorkingSetEffort(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Effort);
                    workout.ReviseWorkingSetLiftingTempo(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Tempo);
                    workout.ReviseWorkingSetRepetitions(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Repetitions);
                    workout.ReviseWorkingSetRestPeriod(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Rest);

                    WorkUnitTemplateEntity workUnitAfter = workout.CloneWorkUnit(toCheck.ProgressiveNumber);

                    CheckWorkUnitSets(workUnitAfter, finalSets, isTransient, null, false);
                }
            }
        }


        internal static void CheckWorkUnit(WorkUnitTemplateEntity workUnit, WorkoutTemplateRoot workout, bool isTransient)
        {
            uint workUnitPnum = workUnit.ProgressiveNumber;

            // Check WU
            if (!isTransient)
                Assert.Equal(workUnit, workout.CloneWorkUnit(workUnitPnum));

            Assert.Equal(workUnit.ExcerciseId, workout.CloneWorkUnit(workUnitPnum).ExcerciseId);
            //Assert.Equal(workUnit.LinkedWorkUnitId, workout.CloneWorkUnit(workUnitPnum).LinkedWorkUnitId);
            Assert.Equal(workUnit.WorkUnitNoteId, workout.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
            Assert.Equal(workUnitPnum, workout.CloneWorkUnit(workUnitPnum).ProgressiveNumber);

            // Check WSs
            foreach (WorkingSetTemplateEntity iws in workUnit.WorkingSets)
            {
                CheckWorkingSet(iws, workout.CloneWorkingSet(workUnitPnum, iws.ProgressiveNumber), isTransient);
                CheckWorkingSet(iws, workout.CloneWorkUnit(workUnit.ProgressiveNumber).CloneWorkingSet(iws.ProgressiveNumber), isTransient);
            }

            // Check Training Parameters
            //CheckTrainingParameters(workout.CloneAllWorkingSets(), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity);
        }


        internal static void CheckWorkUnitsBasicCompare(WorkUnitTemplateEntity left, WorkUnitTemplateEntity right, bool isTransient)
        {
            if (!isTransient)
                Assert.Equal(left, right);

            Assert.Equal(left.ExcerciseId, right.ExcerciseId);
            //Assert.Equal(left.LinkedWorkUnitId, right.LinkedWorkUnitId);
            Assert.Equal(left.WorkUnitNoteId, right.WorkUnitNoteId);
            Assert.Equal(left.ProgressiveNumber, right.ProgressiveNumber);

            // Check WSs
            foreach (WorkingSetTemplateEntity leftWs in left.WorkingSets)
            {
                WorkingSetTemplateEntity rightWs = right.CloneWorkingSet(leftWs.ProgressiveNumber);

                if (!isTransient)
                    Assert.Equal(leftWs, rightWs);

                Assert.Equal(leftWs.Repetitions, rightWs.Repetitions);
                Assert.Equal(leftWs.Rest, rightWs.Rest);
                Assert.Equal(leftWs.Tempo, rightWs.Tempo);
                Assert.Equal(leftWs.Effort, rightWs.Effort);
            }

            // Skip Training Parameters
        }
        #endregion

    }
}
