using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class WorkoutTemplateAggregateTest
    {


        public const int ntests = 300;



        [Fact]
        public void WorkoutFail()
        {
            int ntests = 10;
            bool isTransient;

            IdTypeValue id = IdTypeValue.Create(1);
            IList<WorkUnitTemplate> wusFirstNull = new List<WorkUnitTemplate>();
            IList<WorkUnitTemplate> wusLastNull = new List<WorkUnitTemplate>();
            IList<WorkUnitTemplate> wusMiddleNull = new List<WorkUnitTemplate>();

            for (int itest = 0; itest < ntests; itest++)
            {
                isTransient = itest % 2 == 0;

                wusFirstNull.Add(null);

                for (int i = 0; i < 5; i++)
                {
                    wusFirstNull.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i, isTransient));
                    wusLastNull.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i, isTransient));
                    if (i == 1)
                        wusMiddleNull.Add(null);
                    else
                        wusMiddleNull.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i, isTransient));
                }

                wusLastNull.Add(null);

                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, wusFirstNull, string.Empty));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, wusMiddleNull, string.Empty));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, wusLastNull, string.Empty));

                IList<WorkUnitTemplate> wusPnumStarts1 = new List<WorkUnitTemplate>();
                IList<WorkUnitTemplate> wusPnumGap = new List<WorkUnitTemplate>();

                for (int i = 0; i < 5; i++)
                {
                    wusPnumStarts1.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i + 1, isTransient));

                    if (i == 1)
                        wusPnumGap.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i + 100, isTransient));
                    else
                        wusPnumStarts1.Add(StaticUtils.BuildRandomWorkUnit(i + 1, i + 1, isTransient));
                }

                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, wusPnumStarts1, string.Empty));
                Assert.Throws<TrainingDomainInvariantViolationException>(() => WorkoutTemplate.PlanWorkout(id, wusPnumGap, string.Empty));
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
            WorkoutTemplate workout;
            WorkUnitTemplate wu;
            IdTypeValue woId = IdTypeValue.Create(1);

            for (int itest = 0; itest < ntests; itest++)
            {
                // Generate random parameters
                int workUnitsNum = RandomFieldGenerator.RandomInt(wuMin, wuMax);
                string woName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax);
                WeekdayEnum specificDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.Sunday.Id));
                List<WorkUnitTemplate> initialWus = new List<WorkUnitTemplate>();
                List<WorkUnitTemplate> wus = new List<WorkUnitTemplate>();
                List<IdTypeValue> wuIds = new List<IdTypeValue>();

                isTransient = RandomFieldGenerator.RollEventWithProbability(transientEntityProbability);

                // Initial WUs
                int initialWuNum = RandomFieldGenerator.RandomInt(initialWuMin, initialWuMax);
                int idnum = itest * ntests;
                int pnum = 0;

                for (int iwu = 0; iwu < initialWuNum; iwu++)
                {
                    // Act as the ID has been retrieved from the DB
                    idnum++;
                    wuIds.Add(IdTypeValue.Create(idnum));
                    wu = StaticUtils.BuildRandomWorkUnit(wuIds.Last().Id, pnum++, false);   // Initial WUs are never transient

                    initialWus.Add(wu);
                    wus.Add(wu);
                }

                // WO with initial WUs
                if (isTransient)
                    workout = WorkoutTemplate.PlanTransientWorkout(initialWus, woName, specificDay);  // Transient
                else
                    workout = WorkoutTemplate.PlanWorkout(woId, initialWus, woName, specificDay);     // Persistent

                // No WUs loaded from the DB -> start the sequence
                if (initialWuNum == 0)
                    idnum = 0;

                // Add WUs
                for (int iwu = 0; iwu < workUnitsNum; iwu++)
                {
                    idnum++;
                    wuIds.Add(IdTypeValue.Create(idnum));
                    wu = StaticUtils.BuildRandomWorkUnit(wuIds.Last().Id, pnum++, isTransient);

                    if (isTransient)
                        workout.AddWorkUnit(wu.ExcerciseId, wu.WorkingSets.ToList(), wu.IntensityTechniquesIds.ToList(), wu.OwnerNoteId);
                    else
                        workout.AddWorkUnit(wu);

                    wus.Add(wu);
                }

                // Check WO
                Assert.Equal(woName, workout.Name);
                Assert.Equal(specificDay, workout.SpecificWeekday);
                Assert.Equal(initialWuNum + workUnitsNum, workout.WorkUnits.Count);

                // Check WUs
                foreach (WorkUnitTemplate iwu in wus)
                    CheckWorkUnit(iwu, workout, isTransient);

                // Modify WO
                string newName = RandomFieldGenerator.RandomTextValue(woNameLengthMin, woNameLengthMax, 0.05f);
                uint newPnum = (uint)RandomFieldGenerator.RandomInt(pnumMin, pnumMax);
                WeekdayEnum newDay = WeekdayEnum.From(RandomFieldGenerator.RandomInt(0, WeekdayEnum.AllTheWeek));

                workout.GiveName(newName);
                //workout.MoveToNewProgressiveNumber(newPnum);
                workout.ScheduleToSpecificDay(newDay);

                Assert.Equal(newName, workout.Name);
                //Assert.Equal(newPnum, workout.ProgressiveNumber);
                Assert.Equal(newDay, workout.SpecificWeekday);

                workout.UnscheduleSpecificDay();
                Assert.Equal(WeekdayEnum.Generic, workout.SpecificWeekday);

                // Modify WUs
                foreach (WorkUnitTemplate iwu in workout.WorkUnits)
                    CheckWorkUnitChanges(workout, iwu, isTransient);

                // Modify PNums -> Keep it separate from the Modify WUs block as changing the Pnums will mess with the other tests
                foreach (WorkUnitTemplate iwu in workout.WorkUnits)
                {
                    uint srcPnum = iwu.ProgressiveNumber;
                    uint destPnum = (uint)RandomFieldGenerator.ChooseAmong<int>(
                        workout.WorkUnits.Select(x => (int)x.ProgressiveNumber).ToList());

                    WorkUnitTemplate swappedWorkUnit = workout.CloneWorkUnit(destPnum);

                    workout.MoveWorkUnitToNewProgressiveNumber(srcPnum, destPnum);

                    WorkUnitTemplate toCheck = workout.CloneWorkUnit(destPnum);
                    swappedWorkUnit.MoveToNewProgressiveNumber(srcPnum);    // Simulate the move on the original WU copy

                    CheckWorkUnitsBasicCompare(swappedWorkUnit, workout.CloneWorkUnit(srcPnum), isTransient);
                    CheckWorkUnitsBasicCompare(toCheck, workout.CloneWorkUnit(destPnum), isTransient);
                }


                // Remove WUs
                int nWorkUnits = workout.WorkUnits.Count;

                for (int iwu = 0; iwu < wuRemoveNum; iwu++)
                {
                    uint toRemovePnum = RandomFieldGenerator.ChooseAmong(workout.WorkUnits.Select(x => x.ProgressiveNumber).ToList());
                    WorkUnitTemplate removed = workout.CloneWorkUnit(toRemovePnum);

                    workout.RemoveWorkUnit(toRemovePnum);

                    Assert.Equal(nWorkUnits - iwu - 1, workout.WorkUnits.Count);

                    if (workout.WorkUnits.Count > 0)
                    {
                        Assert.DoesNotContain(removed, workout.WorkUnits);
                        // Check pnum boundaries
                        Assert.Equal(0, (int)workout.WorkUnits.OrderBy(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);
                        Assert.Equal(workout.WorkUnits.Count - 1, (int)workout.WorkUnits.OrderByDescending(x => x.ProgressiveNumber).FirstOrDefault().ProgressiveNumber);

                        CheckTrainingParameters(workout.WorkUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity, null);
                    }
                }

                // Add WUs
                long toAddId;
                bool wasAdded = false;
                WorkUnitTemplate toAdd;
                nWorkUnits = workout.WorkUnits.Count;

                for (int iwu = 0; iwu < wuAddNum; iwu++)
                {
                    IdTypeValue excerciseId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(500, 1000));
                    IdTypeValue noteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 500));


                    if (isTransient)
                    {
                        wasAdded = true;
                        toAdd = StaticUtils.BuildRandomWorkUnit(1, workout.WorkUnits.Count, isTransient);
                        workout.AddWorkUnit(toAdd.ExcerciseId, toAdd.WorkingSets.ToList(), toAdd.IntensityTechniquesIds.ToList(), toAdd.OwnerNoteId);
                    }
                    else
                    {
                        // Trying to add an already present item
                        if(nWorkUnits > 0 && RandomFieldGenerator.RollEventWithProbability(0.1f))
                        {
                            wasAdded = false;
                            toAddId = RandomFieldGenerator.ChooseAmong(
                                workout.WorkUnits.Select(x => (int)x.Id.Id).ToList());
                        }
                        else
                        {
                            wasAdded = true;

                            toAddId = RandomFieldGenerator.RandomIntValueExcluded(1, 25,    // Not too big right boundary to avoid overflow on derived Ids
                                workout.WorkUnits.Select(x => (int)x.Id.Id).ToList());
                        }

                        toAdd = StaticUtils.BuildRandomWorkUnit(toAddId, workout.WorkUnits.Count, isTransient);
                        workout.AddWorkUnit(toAdd);
                    }

                    if(wasAdded)
                    {
                        WorkUnitTemplate added = workout.CloneWorkUnit(toAdd.ProgressiveNumber);      // Id is different, not PNum

                        Assert.Equal(++nWorkUnits, workout.WorkUnits.Count);
                        CheckWorkUnitsBasicCompare(toAdd, added, isTransient);
                    }
                    else
                        Assert.Equal(nWorkUnits, workout.WorkUnits.Count);

                    //Assert.Equal(toAdd.ProgressiveNumber, added.ProgressiveNumber);
                    //Assert.Equal(toAdd.ExcerciseId, added.ExcerciseId);
                    //Assert.Equal(toAdd.OwnerNoteId, added.OwnerNoteId);
                    //Assert.Equal(toAdd.TrainingDensity, added.TrainingDensity);
                    //Assert.Equal(toAdd.TrainingIntensity, added.TrainingIntensity);
                    //Assert.Equal(toAdd.TrainingVolume, added.TrainingVolume);
                    //Assert.Equal(toAdd.WorkingSets.Count, added.WorkingSets.Count);
                    //Assert.Equal(toAdd.IntensityTechniquesIds.Count, added.IntensityTechniquesIds.Count);


                    CheckTrainingParameters(workout.WorkUnits.SelectMany(x => x.WorkingSets), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity, null);
                }
            }
        }


        #region Support Check Functions
        internal static void CheckTrainingParameters(IEnumerable<WorkingSetTemplate> srcWorkingSets,
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
                    if (avgEffort.IsIntensityPercentage())
                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: intensityPercentageTolerance);    // Rounding error can be moderately high because of multiple conversions

                    else
                        StaticUtils.CheckConversions(avgEffort.Value, intensity.AverageIntensity.Value, tolerance: rpeAndRmTolerance);    // Rounding error can be moderately high because of multiple conversions
                }
            }
        }


        internal static void CheckWorkUnitSets(WorkUnitTemplate wu, IEnumerable<WorkingSetTemplate> ws, bool isTransient, TrainingEffortTypeEnum effortType, bool wsFullCheck = true)
        {
            foreach (WorkingSetTemplate wsCheck in ws)
            {
                if(!isTransient)
                    Assert.Contains(wsCheck, wu.WorkingSets);

                if (wsFullCheck)
                    CheckWorkingSet(wsCheck, wu.CloneWorkingSet(wsCheck.ProgressiveNumber), isTransient, wu);
            }
            CheckTrainingParameters(wu.WorkingSets, wu.TrainingVolume, wu.TrainingDensity, wu.TrainingIntensity, effortType); ;
        }


        internal static void CheckWorkingSet(WorkingSetTemplate left, WorkingSetTemplate right, bool isTransient, WorkUnitTemplate wunit = null)
        {
            if (!isTransient)
                Assert.Equal(left.Id, right.Id);

            Assert.Equal(left.Repetitions, right.Repetitions);
            Assert.Equal(left.Rest, right.Rest);
            Assert.Equal(left.Tempo, right.Tempo);
            Assert.Equal(left.Effort, right.Effort);

            // Left and right must have the same Int techniques
            if (wunit == null)
            {
                Assert.Equal(left.IntensityTechniqueIds, right.IntensityTechniqueIds);

                foreach (IdTypeValue idLeft in left.IntensityTechniqueIds)
                    Assert.Contains(idLeft, right.IntensityTechniqueIds);
            }
            else
            {
                // Right has the left Int techniques + the WU ones
                Assert.Equal(left.IntensityTechniqueIds.Count + wunit.IntensityTechniquesIds.Count, right.IntensityTechniqueIds.Count);

                foreach (IdTypeValue idLeft in left.IntensityTechniqueIds)
                    Assert.Contains(idLeft, right.IntensityTechniqueIds);

                foreach (IdTypeValue idWunit in wunit.IntensityTechniquesIds)
                    Assert.Contains(idWunit, right.IntensityTechniqueIds);
            }
        }


        private static void CheckWorkUnitChanges(WorkoutTemplate workout, WorkUnitTemplate workUnit, bool isTransient)
        {
            int excerciseIdMin = 1, excerciseIdMax = 200;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;
            int noteIdMin = 25, noteIdMax = 500;
            int wsToRemoveNum = 2;

            // Need to clone the WU after every change
            WorkUnitTemplate toCheck;

            IdTypeValue newExcerciseId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax));
            IdTypeValue newNoteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));

            int srcIntensityTechniquesNum = workUnit.IntensityTechniquesIds.Count;

            workout.AssignWorkUnitExcercise(workUnit.ProgressiveNumber, newExcerciseId);
            workout.AssignWorkUnitNote(workUnit.ProgressiveNumber, newNoteId);

            // Add a single intensity technique
            IdTypeValue newIntTechnique = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(
                intTechniqueIdMin, intTechniqueIdMax, workUnit.IntensityTechniquesIds.Select(x => (int)x.Id)));

            workout.AddWorkUnitIntensityTechnique(workUnit.ProgressiveNumber, newIntTechnique);
            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);

            Assert.Equal(srcIntensityTechniquesNum + 1, toCheck.IntensityTechniquesIds.Count);
            Assert.Contains(newIntTechnique, toCheck.IntensityTechniquesIds);

            foreach (WorkingSetTemplate workingSet in toCheck.WorkingSets)
                Assert.Contains(newIntTechnique, workingSet.IntensityTechniqueIds);

            // Remove a single intensity technique
            IdTypeValue removedIntTechnique = RandomFieldGenerator.ChooseAmong(toCheck.IntensityTechniquesIds.ToList());

            workout.RemoveWorkUnitIntensityTechnique(workUnit.ProgressiveNumber, removedIntTechnique);
            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);

            Assert.Equal(srcIntensityTechniquesNum, toCheck.IntensityTechniquesIds.Count);
            Assert.DoesNotContain(removedIntTechnique, toCheck.IntensityTechniquesIds);

            foreach (WorkingSetTemplate wset in toCheck.WorkingSets)
                Assert.DoesNotContain(removedIntTechnique, wset.IntensityTechniqueIds);


            // Check WU changes
            Assert.Equal(newNoteId, toCheck.OwnerNoteId);
            Assert.Equal(newExcerciseId, toCheck.ExcerciseId);

            workout.RemoveWorkUnitNote(toCheck.ProgressiveNumber);
            toCheck = workout.CloneWorkUnit(workUnit.ProgressiveNumber);
            Assert.Null(toCheck.OwnerNoteId);

            // Remove WSs
            List<WorkingSetTemplate> originalSets = toCheck.WorkingSets.ToList();
            List<WorkingSetTemplate> finalSets = toCheck.WorkingSets.ToList();

            for (int iws = 0; iws < wsToRemoveNum; iws++)
            {
                uint pnumToRemove = RandomFieldGenerator.ChooseAmong(toCheck.WorkingSets.Select(x => x.ProgressiveNumber).ToList());

                WorkingSetTemplate removed = workout.CloneWorkingSet(toCheck.ProgressiveNumber, pnumToRemove);

                if (toCheck.WorkingSets.Count > 1)
                {
                    workout.RemoveWorkingSet(toCheck.ProgressiveNumber, pnumToRemove);
                    toCheck = workout.CloneWorkUnit(toCheck.ProgressiveNumber);

                    Assert.Equal(originalSets.Count - iws - 1, toCheck.WorkingSets.Count);
                    Assert.DoesNotContain(removed, toCheck.WorkingSets);

                    IEnumerable<WorkingSetTemplate> wsRemoved = originalSets.Where(x => x.ProgressiveNumber == pnumToRemove);

                    finalSets = StaticUtils.ForceConsecutiveProgressiveNumbers(
                        finalSets.Where(x => x.ProgressiveNumber != pnumToRemove).ToList()).ToList();

                    CheckWorkUnitSets(toCheck, finalSets, isTransient, toCheck.TrainingIntensity.AverageIntensity.EffortType, false);
                }
                else
                    ;       // Removing the last WS will lead to an error in the following iterations: no check for failure
            }
        }


        private static void CheckWorkUnit(WorkUnitTemplate workUnit, WorkoutTemplate workout, bool isTransient)
        {
            uint workUnitPnum = workUnit.ProgressiveNumber;

            // Check WU
            if (!isTransient)
                Assert.Equal(workUnit.Id, workout.CloneWorkUnit(workUnitPnum).Id);
            Assert.Equal(workUnit.ExcerciseId, workout.CloneWorkUnit(workUnitPnum).ExcerciseId);
            Assert.Equal(workUnit.IntensityTechniquesIds, workout.CloneWorkUnit(workUnitPnum).IntensityTechniquesIds);
            Assert.Equal(workUnit.OwnerNoteId, workout.CloneWorkUnit(workUnitPnum).OwnerNoteId);
            Assert.Equal(workUnitPnum, workout.CloneWorkUnit(workUnitPnum).ProgressiveNumber);

            // Check WSs
            foreach (WorkingSetTemplate iws in workUnit.WorkingSets)
            {
                CheckWorkingSet(iws, workout.CloneWorkingSet(workUnitPnum, iws.ProgressiveNumber), isTransient);
                CheckWorkingSet(iws, workout.CloneWorkUnit(workUnit.ProgressiveNumber).CloneWorkingSet(iws.ProgressiveNumber), isTransient);
            }

            // Check Training Parameters
            CheckTrainingParameters(workout.CloneAllWorkingSets(), workout.TrainingVolume, workout.TrainingDensity, workout.TrainingIntensity);
        }


        private static void CheckWorkUnitsBasicCompare(WorkUnitTemplate left, WorkUnitTemplate right, bool isTransient)
        {
            if (!isTransient)
                Assert.Equal(left.Id, right.Id);

            Assert.Equal(left.ExcerciseId, right.ExcerciseId);
            Assert.Equal(left.IntensityTechniquesIds, right.IntensityTechniquesIds);
            Assert.Equal(left.OwnerNoteId, right.OwnerNoteId);
            Assert.Equal(left.ProgressiveNumber, right.ProgressiveNumber);

            // Check WSs
            foreach (WorkingSetTemplate leftWs in left.WorkingSets)
            {
                WorkingSetTemplate rightWs = right.CloneWorkingSet(leftWs.ProgressiveNumber);

                if (!isTransient)
                    Assert.Equal(leftWs.Id, rightWs.Id);

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
