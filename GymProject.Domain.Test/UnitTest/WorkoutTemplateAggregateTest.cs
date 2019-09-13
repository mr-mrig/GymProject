//using GymProject.Domain.Base;
//using GymProject.Domain.SharedKernel;
//using GymProject.Domain.Test.Util;
//using GymProject.Domain.TrainingDomain.Common;
//using GymProject.Domain.TrainingDomain.Exceptions;
//using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;

//namespace GymProject.Domain.Test.UnitTest
//{
//    public class WorkoutTemplateAggregateTest
//    {


//        public const int ntests = 500;



//        [Fact]
//        public void WorkoutFail()
//        {

//            bool isTransient;

//            uint? id = 1;
//            IList<WorkUnitTemplateEntity> wusFirstNull = new List<WorkUnitTemplateEntity>();
//            IList<WorkUnitTemplateEntity> wusLastNull = new List<WorkUnitTemplateEntity>();
//            IList<WorkUnitTemplateEntity> wusMiddleNull = new List<WorkUnitTemplateEntity>();

//            for (int itest = 0; itest < ntests; itest++)
//            {
//                isTransient = itest % 2 == 0;

//                wusFirstNull.Add(null);

//                for (uint i = 0; i < 5; i++)
//                {
//                    wusFirstNull.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i, isTransient));
//                    wusLastNull.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i, isTransient));
//                    if (i == 1)
//                        wusMiddleNull.Add(null);
//                    else
//                        wusMiddleNull.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i, isTransient));
//                }

//                wusLastNull.Add(null);

//                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, wusFirstNull, string.Empty));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, wusMiddleNull, string.Empty));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, wusLastNull, string.Empty));

//                IList<WorkUnitTemplateEntity> wusPnumStarts1 = new List<WorkUnitTemplateEntity>();
//                IList<WorkUnitTemplateEntity> wusPnumGap = new List<WorkUnitTemplateEntity>();

//                for (uint i = 0; i < 5; i++)
//                {
//                    wusPnumStarts1.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i + 1, isTransient));

//                    if (i == 1)
//                        wusPnumGap.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i + 100, isTransient));
//                    else
//                        wusPnumGap.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(i + 1, i + 1, isTransient));
//                }

//                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, wusPnumStarts1, string.Empty));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplateRoot.PlanWorkout(id, wusPnumGap, string.Empty));
//            }
//        }



//        [Fact]
//        public void WorkoutFullTest()
//        {
//            int wuMin = 2, wuMax = 7;
//            int initialWuMin = 0, initialWuMax = 2;
//            int woNameLengthMin = 4, woNameLengthMax = 50;
//            int pnumMin = 0, pnumMax = 50;
//            int wuRemoveNum = wuMin;
//            int wuAddNum = 2;

//            float transientEntityProbability = 0.1f;

//            bool isTransient;
//            WorkoutTemplateRoot workout;
//            WorkUnitTemplateEntity wu;
//            uint? woId = 1;

//            for (int itest = 0; itest < ntests; itest++)
//            {
//                // Generate random parameters
//                int workUnitsNum = RandomFieldGenerator.RandomInt(wuMin, wuMax);
//                string woName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax);
//                WeekdayEnum specificDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.Sunday.Id));
//                List<WorkUnitTemplateEntity> initialWus = new List<WorkUnitTemplateEntity>();
//                List<WorkUnitTemplateEntity> wus = new List<WorkUnitTemplateEntity>();
//                List<uint?> wuIds = new List<uint?>();

//                isTransient = RandomFieldGenerator.RollEventWithProbability(transientEntityProbability);

//                // Initial WUs
//                int initialWuNum = RandomFieldGenerator.RandomInt(initialWuMin, initialWuMax);
//                uint? idnum = (uint?)(itest * ntests);
//                uint pnum = 0;

//                for (int iwu = 0; iwu < initialWuNum; iwu++)
//                {
//                    // Act as the ID has been retrieved from the DB
//                    idnum++;
//                    wuIds.Add(idnum);
//                    wu = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(wuIds.Last().Value, pnum++, false);   // Initial WUs are never transient

//                    initialWus.Add(wu);
//                    wus.Add(wu);
//                }

//                // WO with initial WUs
//                if (isTransient)
//                    workout = WorkoutTemplateRoot.PlanTransientWorkout(initialWus, woName, specificDay);  // Transient
//                else
//                    workout = WorkoutTemplateRoot.PlanWorkout(woId, initialWus, woName, specificDay);     // Persistent

//                // No WUs loaded from the DB -> start the sequence
//                if (initialWuNum == 0)
//                    idnum = 0;

//                // Add WUs
//                for (int iwu = 0; iwu < workUnitsNum; iwu++)
//                {
//                    idnum++;
//                    wuIds.Add(idnum);
//                    wu = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(wuIds.Last().Value, pnum++, isTransient);

//                    if (isTransient)
//                        workout.PlanTransientExcercise(wu.ExcerciseId, wu.WorkingSets.ToList(), wu.IntensityTechniquesIds.ToList(), wu.WorkUnitNoteId);
//                    else
//                        workout.PlanExcercise(wu);

//                    wus.Add(wu);
//                }

//                // Check WO
//                Assert.Equal(woName, workout.Name);
//                Assert.Equal(specificDay, workout.SpecificWeekday);
//                Assert.Equal(initialWuNum + workUnitsNum, workout.WorkUnits.Count());

//                // Check WUs
//                foreach (WorkUnitTemplateEntity iwu in wus)
//                    CheckWorkUnit(iwu, workout, isTransient);

//                // Modify WO
//                string newName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax, true, 0.05f);
//                uint newPnum = (uint)RandomFieldGenerator.RandomInt(pnumMin, pnumMax);
//                WeekdayEnum newDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.AllTheWeek));

//                workout.GiveName(newName);
//                //workout.MoveToNewProgressiveNumber(newPnum);
//                workout.ScheduleToSpecificDay(newDay);

//                Assert.Equal(newName, workout.Name);
//                //Assert.Equal(newPnum, workout.ProgressiveNumber);
//                Assert.Equal(newDay, workout.SpecificWeekday);

//                workout.UnscheduleSpecificDay();
//                Assert.Equal(WeekdayEnum.Generic, workout.SpecificWeekday);


//                // Modify WUs
//                foreach (WorkUnitTemplateEntity iwu in workout.WorkUnits)
//                    CheckWorkUnitChanges(workout, iwu, isTransient);


//                // Modify PNums -> Keep it separate from the Modify WUs block as changing the Pnums will mess with the other tests
//                foreach (WorkUnitTemplateEntity iwu in workout.WorkUnits)
//                {
//                    uint srcPnum = iwu.ProgressiveNumber;
//                    uint destPnum = (uint)RandomFieldGenerator.ChooseAmong<int>(
//                        workout.WorkUnits.Select(x => (int)x.ProgressiveNumber).ToList());

//                    WorkUnitTemplateEntity swappedWorkUnit = workout.CloneWorkUnit(destPnum);

//                    workout.MoveWorkUnitToNewProgressiveNumber(srcPnum, destPnum);

//                    WorkUnitTemplateEntity toCheck = workout.CloneWorkUnit(destPnum);
//                    swappedWorkUnit.MoveToNewProgressiveNumber(srcPnum);    // Simulate the move on the original WU copy

//                    CheckWorkUnitsBasicCompare(swappedWorkUnit, workout.CloneWorkUnit(srcPnum), isTransient);
//                    CheckWorkUnitsBasicCompare(toCheck, workout.CloneWorkUnit(destPnum), isTransient);
//                }


//                // Remove WUs
//                int nWorkUnits = workout.WorkUnits.Count();

//                for (uint iwu = 0; iwu < wuRemoveNum; iwu++)
//                {
//                    uint toRemovePnum = RandomFieldGenerator.ChooseAmong(workout.WorkUnits.Select(x => x.ProgressiveNumber).ToList());
//                    WorkUnitTemplateEntity removed = workout.CloneWorkUnit(toRemovePnum);

//                    workout.UnplanExcercise(toRemovePnum);

//                    Assert.Equal(nWorkUnits - iwu - 1, workout.WorkUnits.Count());

//                    if (workout.WorkUnits.Count() > 0)
//                    {
//                        Assert.True(workout.WorkUnits.Select(x => (int)x.ProgressiveNumber).
//                            SequenceEqual(Enumerable.Range(0, workout.WorkUnits.Count())));
                            
//                        Assert.DoesNotContain(removed, workout.WorkUnits);
//                        // Check pnum boundaries
//                        Assert.Equal(0, (int)workout.WorkUnits.OrderBy(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);
//                        Assert.Equal(workout.WorkUnits.Count() - 1, (int)workout.WorkUnits.OrderByDescending(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);

//                        CheckTrainingParameters(workout.WorkUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity, null);
//                    }
//                }

//                // Add WUs
//                uint toAddId;
//                bool wasAdded = false;
//                WorkUnitTemplateEntity toAdd;
//                nWorkUnits = workout.WorkUnits.Count();

//                for (int iwu = 0; iwu < wuAddNum; iwu++)
//                {
//                    uint? excerciseId = (uint?)(RandomFieldGenerator.RandomInt(500, 1000));
//                    uint? noteId = (uint?)(RandomFieldGenerator.RandomInt(1, 500));


//                    if (isTransient)
//                    {
//                        wasAdded = true;
//                        toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(1, (uint)workout.WorkUnits.Count(), isTransient);
//                        workout.PlanTransientExcercise(toAdd.ExcerciseId, toAdd.WorkingSets.ToList(), toAdd.IntensityTechniquesIds.ToList(), toAdd.WorkUnitNoteId);
//                    }
//                    else
//                    {
//                        // Trying to add an already present item
//                        if(nWorkUnits > 0 && RandomFieldGenerator.RollEventWithProbability(0.1f))
//                        {
//                            wasAdded = false;
//                            toAddId = (uint)RandomFieldGenerator.ChooseAmong(
//                                workout.WorkUnits.Select(x => x.Id).ToList());


//                            toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(toAddId, (uint)workout.WorkUnits.Count(), isTransient);
//                            Assert.Throws<ArgumentException>(() => workout.PlanExcercise(toAdd));
//                        }
//                        else
//                        {
//                            wasAdded = true;

//                            toAddId = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 25,    // Not too big right boundary to avoid overflow on derived Ids
//                                workout.WorkUnits.Select(x => (int)x.Id).ToList());

//                            toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkUnitTemplate(toAddId, (uint)workout.WorkUnits.Count(), isTransient);
//                            workout.PlanExcercise(toAdd);
//                        }
//                    }

//                    if(wasAdded)
//                    {
//                        WorkUnitTemplateEntity added = workout.CloneWorkUnit(toAdd.ProgressiveNumber);      // Id is different, not PNum

//                        Assert.Equal(++nWorkUnits, workout.WorkUnits.Count());
//                        CheckWorkUnitsBasicCompare(toAdd, added, isTransient);
//                    }
//                    else
//                        Assert.Equal(nWorkUnits, workout.WorkUnits.Count());


//                    CheckTrainingParameters(workout.WorkUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity, null);
//                }
//            }
//        }


//        #region Support Check Functions
//        internal static void CheckTrainingParameters(IEnumerable<WorkingSetTemplateEntity> srcWorkingSets,
//            TrainingVolumeParametersValue volume, TrainingDensityParametersValue density, TrainingIntensityParametersValue intensity, TrainingEffortTypeEnum mainEffortType = null)
//        {
//            TrainingEffortValue avgEffort = null;
//            float intensityPercentageTolerance = 0.025f;
//            float rpeAndRmTolerance = 0.0625f;      // Smaller numbers -> Higher tolerance. IE: 5RPE Vs 5.5RPE must be considered equivalent

//            // Get the expected training parameters
//            int totalReps = srcWorkingSets.Sum(x => x.ToRepetitions());
//            int totalWs = srcWorkingSets.Count();
//            float totalWorkload = srcWorkingSets.Sum(x => x.ToWorkload().Value);
//            int totalTempo = srcWorkingSets.Sum(x => x.ToSecondsUnderTension());
//            int totalRest = srcWorkingSets.Sum(x => x.ToTotalSeconds()) - totalTempo;


//            // Convert to Main Effort Type
//            if (totalWs == 0)
//            {
//                // Check Volume
//                Assert.Equal(totalReps, volume.TotalReps);
//                Assert.Equal(totalWs, volume.TotalWorkingSets);
//                Assert.Equal(0, volume.TotalWorkload.Value);
//                Assert.Equal(0, volume.GetAverageRepetitions(), 1);
//                Assert.Equal(0, volume.GetAverageWorkloadPerSet().Value);

//                // Check Density
//                Assert.Equal(totalRest, density.TotalRest);
//                Assert.Equal(totalTempo, density.TotalSecondsUnderTension);
//                Assert.Equal(0, density.GetAverageRest(), 1);
//                Assert.Equal(0, density.GetAverageSecondsUnderTension(), 1);

//                // Check Intensity
//                Assert.Null(intensity.AverageIntensity);
//            }
//            else
//            {
//                avgEffort = StaticUtils.ComputeAverageEffort(srcWorkingSets, mainEffortType);

//                // Check Volume
//                Assert.Equal(totalReps, volume.TotalReps);
//                Assert.Equal(totalWs, volume.TotalWorkingSets);
//                Assert.Equal(0, volume.TotalWorkload.Value);
//                Assert.Equal((float)totalReps / (float)totalWs, volume.GetAverageRepetitions(), 1);
//                Assert.Equal(0, volume.GetAverageWorkloadPerSet().Value);

//                // Check Density
//                Assert.Equal(totalRest, density.TotalRest);
//                Assert.Equal(totalTempo, density.TotalSecondsUnderTension);
//                Assert.Equal((float)totalRest / (float)totalWs, density.GetAverageRest(), 1);
//                Assert.Equal((float)totalTempo / (float)totalWs, density.GetAverageSecondsUnderTension(), 1);

//                // Check Intensity
//                if (srcWorkingSets.All(x => !x.IsAMRAP()) && !avgEffort.IsRPE())     // AMRAP with RPE effort, can't be managed!
//                {
//                    if (avgEffort.IsIntensityPercentage())
//                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: intensityPercentageTolerance);    // Rounding error can be moderately high because of multiple conversions

//                    else
//                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: rpeAndRmTolerance);    // Rounding error can be moderately high because of multiple conversions
//                }
//            }
//        }


//        internal static void CheckWorkUnitSets(WorkUnitTemplateEntity wu, IEnumerable<WorkingSetTemplateEntity> ws, bool isTransient, TrainingEffortTypeEnum effortType = null, bool wsFullCheck = true)
//        {
//            foreach (WorkingSetTemplateEntity wsCheck in ws)
//            {
//                if(!isTransient)
//                    Assert.Contains(wsCheck, wu.WorkingSets);

//                if (wsFullCheck)
//                    CheckWorkingSet(wsCheck, wu.CloneWorkingSet(wsCheck.ProgressiveNumber), isTransient, wu);
//            }
//            CheckTrainingParameters(wu.WorkingSets, wu.TrainingVolume, wu.TrainingDensity, wu.TrainingIntensity, effortType);
//        }


//        internal static void CheckWorkingSet(WorkingSetTemplateEntity left, WorkingSetTemplateEntity right, bool isTransient, WorkUnitTemplateEntity wunit = null)
//        {
//            if (!isTransient)
//                Assert.Equal(left, right);

//            Assert.Equal(left.Repetitions, right.Repetitions);
//            Assert.Equal(left.Rest, right.Rest);
//            Assert.Equal(left.Tempo, right.Tempo);
//            Assert.Equal(left.Effort, right.Effort);

//            // Left and right must have the same Int techniques
//            if (wunit == null)
//            {
//                Assert.Equal(left.IntensityTechniqueIds, right.IntensityTechniqueIds);

//                foreach (uint? idLeft in left.IntensityTechniqueIds)
//                    Assert.Contains(idLeft, right.IntensityTechniqueIds);
//            }
//            else
//            {
//                // Right has the left Int techniques + the WU ones
//                Assert.Equal(left.IntensityTechniqueIds.Count + wunit.IntensityTechniquesIds.Count, right.IntensityTechniqueIds.Count);

//                foreach (uint? idLeft in left.IntensityTechniqueIds)
//                    Assert.Contains(idLeft, right.IntensityTechniqueIds);

//                foreach (uint? idWunit in wunit.IntensityTechniquesIds)
//                    Assert.Contains(idWunit, right.IntensityTechniqueIds);
//            }
//        }


//        internal static void CheckWorkUnitChanges(WorkoutTemplateRoot workout, WorkUnitTemplateEntity workUnit, bool isTransient)
//        {
//            int excerciseIdMin = 1, excerciseIdMax = 200;
//            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;
//            int noteIdMin = 25, noteIdMax = 500;
//            int wsToRemoveNum = 2, wsToAddNum = 2, wsToChangeNum = 2;

//            float failInsertionProbability = 0.05f;
//            float failRemovalProbability = 0.05f;


//            // Need to clone the WU after every change
//            WorkUnitTemplateEntity toCheck;
//            WorkingSetTemplateEntity toAdd;

//            uint? newExcerciseId = (uint?)(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax));
//            uint? newNoteId = (uint?)(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));

//            int srcIntensityTechniquesNum = workUnit.IntensityTechniquesIds.Count;

//            workout.AssignWorkUnitExcercise(workUnit.ProgressiveNumber, newExcerciseId);
//            workout.AssignWorkUnitNote(workUnit.ProgressiveNumber, newNoteId);

//            // Add a single intensity technique
//            // Fake Add
//            if (workUnit.IntensityTechniquesIds.Count > 0 && RandomFieldGenerator.RollEventWithProbability(failInsertionProbability))
//            {
//                uint? duplicateIntTechnique = RandomFieldGenerator.ChooseAmong(workUnit.IntensityTechniquesIds);

//                workout.LinkWorkUnits(workUnit.ProgressiveNumber, duplicateIntTechnique);
//                toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);

//                // No changes
//                Assert.Equal(srcIntensityTechniquesNum, toCheck.IntensityTechniquesIds.Count);
//                Assert.Contains(duplicateIntTechnique, toCheck.IntensityTechniquesIds);
//            }
//            // Real Add
//            uint? newIntTechnique = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(
//                intTechniqueIdMin, intTechniqueIdMax, workUnit.IntensityTechniquesIds.Select(x => (int)x)));

//            workout.LinkWorkUnits(workUnit.ProgressiveNumber, newIntTechnique);
//            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);

//            Assert.Equal(srcIntensityTechniquesNum + 1, toCheck.IntensityTechniquesIds.Count);
//            Assert.Contains(newIntTechnique, toCheck.IntensityTechniquesIds);

//            foreach (WorkingSetTemplateEntity workingSet in toCheck.WorkingSets)
//                Assert.Contains(newIntTechnique, workingSet.IntensityTechniqueIds);


//            // Remove a single intensity technique
//            uint? removedIntTechnique = RandomFieldGenerator.ChooseAmong(toCheck.IntensityTechniquesIds.ToList());

//            workout.RemoveWorkUnitIntensityTechnique(workUnit.ProgressiveNumber, removedIntTechnique);
//            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);

//            Assert.Equal(srcIntensityTechniquesNum, toCheck.IntensityTechniquesIds.Count);
//            Assert.DoesNotContain(removedIntTechnique, toCheck.IntensityTechniquesIds);

//            foreach (WorkingSetTemplateEntity wset in toCheck.WorkingSets)
//                Assert.DoesNotContain(removedIntTechnique, wset.IntensityTechniqueIds);


//            // Check WU changes
//            Assert.Equal(newNoteId, toCheck.WorkUnitNoteId);
//            Assert.Equal(newExcerciseId, toCheck.ExcerciseId);

//            workout.RemoveWorkUnitNote(toCheck.ProgressiveNumber);
//            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);
//            Assert.Null(toCheck.WorkUnitNoteId);

//            // Remove WSs
//            List<WorkingSetTemplateEntity> originalSets = toCheck.WorkingSets.ToList();
//            List<WorkingSetTemplateEntity> finalSets = toCheck.WorkingSets.ToList();
//            int wsRemoveCounter = 0;

//            for (int iws = 0; iws < wsToRemoveNum; iws++)
//            {
//                if(RandomFieldGenerator.RollEventWithProbability(failRemovalProbability))
//                    Assert.Throws<InvalidOperationException>(() => workout.RemoveWorkingSet(toCheck.ProgressiveNumber, int.MaxValue));
//                else
//                {
//                    uint pnumToRemove = RandomFieldGenerator.ChooseAmong(toCheck.WorkingSets.Select(x => x.ProgressiveNumber).ToList());
//                    WorkingSetTemplateEntity removed = workout.CloneWorkingSet(toCheck.ProgressiveNumber, pnumToRemove);

//                    if (toCheck.WorkingSets.Count > 1)
//                    {
//                        workout.RemoveWorkingSet(toCheck.ProgressiveNumber, pnumToRemove);
//                        toCheck = workout.CloneWorkUnit(toCheck.ProgressiveNumber);

//                        Assert.Equal(originalSets.Count - ++wsRemoveCounter, toCheck.WorkingSets.Count);
//                        Assert.DoesNotContain(removed, toCheck.WorkingSets);

//                        IEnumerable<WorkingSetTemplateEntity> wsRemoved = originalSets.Where(x => x.ProgressiveNumber == pnumToRemove);

//                        finalSets = StaticUtils.ForceConsecutiveProgressiveNumbers(
//                            finalSets.Where(x => x.ProgressiveNumber != pnumToRemove)).ToList();

//                        Assert.True(toCheck.WorkingSets.Select(x => (int)x.ProgressiveNumber).
//                            SequenceEqual(Enumerable.Range(0, toCheck.WorkingSets.Count)));

//                        CheckWorkUnitSets(toCheck, finalSets, isTransient, toCheck.TrainingIntensity.AverageIntensity.EffortType, false);
//                    }
//                    //else
//                    //    ;       // Removing the last WS will lead to an error in the following iterations: no check for failure
//                }
//            }

//            // Add WSs
//            bool duplicateId = false;
//            int setsAddedCount = 0;

//            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);    // Keep it updated

//            originalSets = toCheck.WorkingSets.ToList();
//            finalSets = toCheck.WorkingSets.ToList();

//            for (int iws = 0; iws < wsToAddNum; iws++)
//            {
//                if (isTransient)
//                {
//                    duplicateId = false;
//                    toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, finalSets.Count, isTransient, toCheck.GetMainEffortType());
//                    workout.AddTransientWorkingSet(toCheck.ProgressiveNumber, toAdd.Repetitions, toAdd.Rest, toAdd.Effort, toAdd.Tempo, toAdd.IntensityTechniqueIds.ToList());
//                }
//                else
//                {
//                    duplicateId = RandomFieldGenerator.RollEventWithProbability(failInsertionProbability) && originalSets.Count > 0;

//                    uint idToAdd = duplicateId
//                        ? RandomFieldGenerator.ChooseAmong(finalSets.Select(x => (uint)x.Id))
//                        : (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, finalSets.Select(x => (int)x.Id.Value));

//                    toAdd = WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToAdd, finalSets.Count, isTransient, toCheck.GetMainEffortType());
//                }

//                if (duplicateId)
//                    Assert.Throws<ArgumentException>(() => workout.AddWorkingSet(toCheck.ProgressiveNumber, toAdd));
//                else
//                {
//                    if(!isTransient)
//                        workout.AddWorkingSet(toCheck.ProgressiveNumber, toAdd);

//                    finalSets.Add(toAdd);
//                    toCheck = workout.CloneWorkUnit(toCheck.ProgressiveNumber); // Keep it updated

//                    Assert.Equal(originalSets.Count + ++setsAddedCount, toCheck.WorkingSets.Count);

//                    finalSets = StaticUtils.ForceConsecutiveProgressiveNumbers(finalSets).ToList();

//                    Assert.True(toCheck.WorkingSets.Select(x => (int)x.ProgressiveNumber).
//                        SequenceEqual(Enumerable.Range(0, toCheck.WorkingSets.Count)));

//                    CheckWorkUnitSets(toCheck, finalSets, isTransient, toCheck.TrainingIntensity.AverageIntensity.EffortType, false);
//                }
//            }

//            // Change Working Sets
//            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);    // Keep it updated
//            finalSets = toCheck.WorkingSets.ToList();

//            for (int iws = 0; iws < wsToChangeNum; iws++)
//            {
//                uint toChangePnum = (uint)RandomFieldGenerator.RandomInt(0, workUnit.WorkingSets.Count - 1);

//                WorkingSetTemplateEntity workingSetBefeore = finalSets.Single(x => x.ProgressiveNumber == toChangePnum);
//                WorkingSetTemplateEntity newWorkingSet = WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(
//                    isTransient ? 1 : workingSetBefeore.Id.Value, (int)toChangePnum, isTransient, TrainingEffortTypeEnum.RM);

//                workingSetBefeore = newWorkingSet;

//                workout.ReviseWorkingSetEffort(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Effort);
//                workout.ReviseWorkingSetLiftingTempo(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Tempo);
//                workout.ReviseWorkingSetRepetitions(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Repetitions);
//                workout.ReviseWorkingSetRestPeriod(workUnit.ProgressiveNumber, toChangePnum, newWorkingSet.Rest);

//                WorkUnitTemplateEntity workUnitAfter = workout.CloneWorkUnit(toCheck.ProgressiveNumber);

//                CheckWorkUnitSets(workUnitAfter, finalSets, isTransient, null, false);
//            }
//        }


//        internal static void CheckWorkUnit(WorkUnitTemplateEntity workUnit, WorkoutTemplateRoot workout, bool isTransient)
//        {
//            uint workUnitPnum = workUnit.ProgressiveNumber;

//            // Check WU
//            if (!isTransient)
//                Assert.Equal(workUnit, workout.CloneWorkUnit(workUnitPnum));
//            Assert.Equal(workUnit.ExcerciseId, workout.CloneWorkUnit(workUnitPnum).ExcerciseId);
//            Assert.Equal(workUnit.IntensityTechniquesIds, workout.CloneWorkUnit(workUnitPnum).IntensityTechniquesIds);
//            Assert.Equal(workUnit.WorkUnitNoteId, workout.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
//            Assert.Equal(workUnitPnum, workout.CloneWorkUnit(workUnitPnum).ProgressiveNumber);

//            // Check WSs
//            foreach (WorkingSetTemplateEntity iws in workUnit.WorkingSets)
//            {
//                CheckWorkingSet(iws, workout.CloneWorkingSet(workUnitPnum, iws.ProgressiveNumber), isTransient);
//                CheckWorkingSet(iws, workout.CloneWorkUnit(workUnit.ProgressiveNumber).CloneWorkingSet(iws.ProgressiveNumber), isTransient);
//            }

//            // Check Training Parameters
//            CheckTrainingParameters(workout.CloneAllWorkingSets(), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity);
//        }


//        internal static void CheckWorkUnitsBasicCompare(WorkUnitTemplateEntity left, WorkUnitTemplateEntity right, bool isTransient)
//        {
//            if (!isTransient)
//                Assert.Equal(left, right);

//            Assert.Equal(left.ExcerciseId, right.ExcerciseId);
//            Assert.Equal(left.IntensityTechniquesIds, right.IntensityTechniquesIds);
//            Assert.Equal(left.WorkUnitNoteId, right.WorkUnitNoteId);
//            Assert.Equal(left.ProgressiveNumber, right.ProgressiveNumber);

//            // Check WSs
//            foreach (WorkingSetTemplateEntity leftWs in left.WorkingSets)
//            {
//                WorkingSetTemplateEntity rightWs = right.CloneWorkingSet(leftWs.ProgressiveNumber);

//                if (!isTransient)
//                    Assert.Equal(leftWs, rightWs);

//                Assert.Equal(leftWs.Repetitions, rightWs.Repetitions);
//                Assert.Equal(leftWs.Rest, rightWs.Rest);
//                Assert.Equal(leftWs.Tempo, rightWs.Tempo);
//                Assert.Equal(leftWs.Effort, rightWs.Effort);
//            }

//            // Skip Training Parameters
//        }
//        #endregion

//    }
//}
