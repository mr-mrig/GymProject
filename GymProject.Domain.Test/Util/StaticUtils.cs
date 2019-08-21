using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;


namespace GymProject.Domain.Test.Util
{
    internal static class StaticUtils
    {

        /// <summary>
        /// Checks current timestamp is as expected
        /// </summary>
        /// <param name="toBeChecked">The datetime object storing the current timestamp</param>
        internal static void CheckCurrentTimestamp(DateTime toBeChecked)
        {
            Assert.InRange(toBeChecked, DateTime.Now.Subtract(TimeSpan.FromSeconds(1)), DateTime.Now);
        }

        /// <summary>
        /// Checks that two values are inside a specific tolerance.
        /// This should be used to check conversions where rounding can lead to precision issues
        /// </summary>
        /// <param name="srcValue">The original value</param>
        /// <param name="convertedValue">The converted value</param>
        /// <param name="srcUnitMeasId">The Meas Unit ID of the original value</param>
        /// <param name="convertedUnitMeasId">The Meas Unit ID of the converted  value</param>
        /// <param name="tolerance">The tolerance as a [0-1] float - default = 1.5% </param>
        internal static void CheckConversions(float srcValue, float convertedValue, int srcUnitMeasId = -1, int convertedUnitMeasId = -1, float tolerance = 0.02f)
        {
            Assert.InRange(convertedValue, srcValue * (1 - tolerance), srcValue * (1 + tolerance));

            if(srcUnitMeasId > 0 && convertedUnitMeasId > 0)
                Assert.Equal(srcUnitMeasId, convertedUnitMeasId);
        }


        /// <summary>
        /// Compute the average effort of the specified working sets
        /// </summary>
        /// <param name="workingSets">The input working sets</param>
        /// <param name="mainEffortType">The main effort type of the WSs as the most used one - optional</param>
        /// <returns>The average effort value</returns>
        internal static TrainingEffortValue ComputeAverageEffort(IEnumerable<IWorkingSet> workingSets, TrainingEffortTypeEnum mainEffortType = null)
        {
            TrainingEffortValue result = null;
            WSRepetitionValue avgReps;

            // Get the main effort type if not specified, as the most used effort among the WSs ones
            if (mainEffortType == null)
            {
                mainEffortType = workingSets?.GroupBy(x => x.Effort.EffortType)?.Select(x
                 => new
                 {
                     Counter = x.Count(),
                     EffortType = x.Key
                 })?.OrderByDescending(x => x.Counter)?.FirstOrDefault()?.EffortType;
            }


            if (mainEffortType == TrainingEffortTypeEnum.RM)
            {
                IEnumerable<IWorkingSet> rpeSets = workingSets.Where(x => x.Effort.IsRPE());

                if (rpeSets.Count() == 0)
                    avgReps = null;
                else
                    avgReps = WSRepetitionValue.TrackRepetitionSerie((uint)rpeSets.Average(x => x.ToRepetitions()));

                result = TrainingEffortValue.AsRM((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.ToRm(avgReps).Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.ToRm().Value)) / workingSets.Count());
            }

            else if (mainEffortType == TrainingEffortTypeEnum.RPE)
            {
                WSRepetitionValue avgRepsInt;
                WSRepetitionValue avgRepsRM;

                IEnumerable<IWorkingSet> intSets = workingSets.Where(x => x.Effort.IsIntensityPercentage());

                if (intSets.Count() == 0)
                    avgRepsInt = null;
                else
                    avgRepsInt = WSRepetitionValue.TrackRepetitionSerie((uint)intSets.Average(x => x.ToRepetitions()));

                IEnumerable<IWorkingSet> rmSets = workingSets.Where(x => x.Effort.IsRM());

                if (rmSets.Count() == 0)
                    avgRepsRM = null;
                else
                    avgRepsRM = WSRepetitionValue.TrackRepetitionSerie((uint)rmSets.Average(x => x.ToRepetitions()));


                result = TrainingEffortValue.AsRPE((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.ToRPE(avgRepsRM).Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.ToRPE(avgRepsInt).Value)) / workingSets.Count());
            }

            else if (mainEffortType == TrainingEffortTypeEnum.IntensityPerc)
            {
                IEnumerable<IWorkingSet> rpeSets = workingSets.Where(x => x.Effort.IsRPE());

                if (rpeSets.Count() == 0)
                    avgReps = null;
                else
                    avgReps = WSRepetitionValue.TrackRepetitionSerie((uint)rpeSets.Average(x => x.ToRepetitions()));

                result = TrainingEffortValue.AsRM((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.ToIntensityPercentage().Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.ToIntensityPercentage(avgReps).Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.Value)) / workingSets.Count());
            }

            return result;
        }



        internal static ICollection<IdTypeValue> BuildIdsCollection(int minNumber, int maxNumber)
        {
            int nElments = RandomFieldGenerator.RandomInt(minNumber, maxNumber);
            List<IdTypeValue> result = new List<IdTypeValue>();

            for (int iel = 0; iel < nElments; iel++)
                result.Add(new IdTypeValue(iel + 1));

            return result;
        }

        internal static TrainingWeekTemplate BuildRandomTrainingWeek(long id, int progn, int nWorkoutsMin = 3, int nWorkoutsMax = 7, TrainingWeekTypeEnum weekType = null, float noWorkoutsProb = 0.05f)
        {
            // Workouts
            List<WorkoutTemplate> workouts = new List<WorkoutTemplate>();

            int? nWorkouts = RandomFieldGenerator.RandomIntNullable(nWorkoutsMin, nWorkoutsMax, noWorkoutsProb);

            if (nWorkouts == null)
                nWorkouts = 0;


            for (int iwo = 0; iwo < nWorkouts; iwo++)
            {
                //long strongId = ((id - 1) * wuIdOffset + iwu + 1);      // Easiest to read
                long strongId = ((id - 1) * nWorkoutsMax + iwo + 1);      // Smallest possible

                workouts.Add(BuildRandomWorkout(strongId, iwo));
            }

            // Week Type
            if (weekType == null)
            {
                int? weekTypeId = RandomFieldGenerator.RandomIntNullable(0, TrainingWeekTypeEnum.Peak.Id, 0.1f);

                if (weekTypeId == null)
                    weekType = TrainingWeekTypeEnum.Generic;
                else
                    weekType = TrainingWeekTypeEnum.From(weekTypeId.Value);
            }

            return TrainingWeekTemplate.AddTrainingWeekToPlan(
                id: new IdTypeValue(id),
                progressiveNumber: (uint)progn,
                workouts: workouts,
                weekType: weekType
                );
        }


        internal static WorkoutTemplate BuildRandomWorkout(long id, int progn, int nWorkUnitsMin = 3, int nWorkUnitsMax = 7, WeekdayEnum specificDay = null, float emptyWorkoutProb = 0.05f)
        {
            // Work Units
            List<WorkUnitTemplate> workUnits = new List<WorkUnitTemplate>();

            int? nWorkUnits = RandomFieldGenerator.RandomIntNullable(nWorkUnitsMin, nWorkUnitsMax, emptyWorkoutProb);

            if (nWorkUnits == null)
                nWorkUnits = 0;


            for (int iwu = 0; iwu < nWorkUnits; iwu++)
            {
                //long strongId = ((id - 1) * wuIdOffset + iwu + 1);      // Easiest to read
                long strongId = ((id - 1) * nWorkUnitsMax + iwu + 1);      // Smallest possible

                workUnits.Add(BuildRandomWorkUnit(strongId, iwu));
            }

            // Specific Day
            if(specificDay == null)
            {
                int? weekDayId = RandomFieldGenerator.RandomIntNullable(WeekdayEnum.Monday.Id, WeekdayEnum.AllTheWeek, 0.1f);

                if (weekDayId == null)
                    specificDay = WeekdayEnum.Generic;
                else
                    specificDay = WeekdayEnum.From(weekDayId.Value);
            }


            return WorkoutTemplate.PlanWorkout(
                id: new IdTypeValue(id),
                progressiveNumber: (uint)progn,
                workUnits: workUnits,
                workoutName: RandomFieldGenerator.RandomTextValue(4, 5, 0.05f),
                weekday: specificDay
                );
        }



        internal static WorkUnitTemplate BuildRandomWorkUnit(long id, int progn, int wsNumMin = 2, int wsNumMax = 7, int excerciseIdMin = 1, int excerciseIdMax = 500,
            int ownerNoteIdMin = 10, int ownerNoteIdMax = 500)
        {
            List<WorkingSetTemplate> workingSets = new List<WorkingSetTemplate>();
            TrainingEffortTypeEnum effortType;

            int wuIntTechniquesMin = 0, wuIntTechniquesMax = 3;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;

            float intPercMin = 0.5f, intePercMax = 1f;
            float rmMin = 1f, rmMax = 20f;
            float rpeMin = 3f, rpeMax = 11f;
            float effortMin, effortMax;

            //int wsIdOffset = 100;       // Used to ensure no ID collisions among WSs of different WUs

            // Build randomic Effort
            if (id % 2 == 0)
            {
                effortType = TrainingEffortTypeEnum.IntensityPerc;
                effortMin = intPercMin;
                effortMax = intePercMax;
            }

            else if (id % 3 == 0)
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

            // Build randomic TUT
            List<TUTValue> tuts = new List<TUTValue>()
            {
                TUTValue.PlanTUT("1010"),
                TUTValue.PlanTUT("3030"),
                TUTValue.PlanTUT("5151"),
                TUTValue.PlanTUT("30x0"),
                TUTValue.SetGenericTUT(),
            };

            // Build randomic WU intensity techniques
            int wuIntTechniquesNum = RandomFieldGenerator.RandomInt(wuIntTechniquesMin, wuIntTechniquesMax);
            List<IdTypeValue> wuIntensityTechniques = new List<IdTypeValue>();
            for (int i = 0; i < wuIntTechniquesNum; i++)
                wuIntensityTechniques.Add(new IdTypeValue(RandomFieldGenerator.RandomIntValueExcluded(intTechniqueIdMin, intTechniqueIdMax, wuIntensityTechniques.Select(x => (int)x.Id))));

            // Add randomic Working Sets
            int iwsMax = RandomFieldGenerator.RandomInt(wsNumMin, wsNumMax);

            for (int iws = 0; iws < iwsMax; iws++)
            {
                //long strongId = ((id - 1) * wsIdOffset + iws + 1);      // Easiest to read
                long strongId = ((id - 1) * wsNumMax + iws + 1);      // Smallest possible

                workingSets.Add(BuildRandomWorkingSet(strongId, iws, effortType, effortMin, effortMax, tutToChooseAmong: tuts));
            }

            return WorkUnitTemplate.PlanWorkUnit(
                id: new IdTypeValue(id),
                progressiveNumber: (uint)progn,
                excerciseId: new IdTypeValue(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax)),
                workingSets: workingSets,
                workUnitIntensityTechniqueIds: wuIntensityTechniques,
                ownerNoteId: new IdTypeValue(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
            );
        }


        internal static WorkingSetTemplate BuildRandomWorkingSet(long id, int progn, TrainingEffortTypeEnum effortType, float effortMin, float effortMax,
            int repsMin = 1, int repsMax = 20, bool repetitionsSerie = true, float amrapProbability = 0.1f, int restMin = 5, int restMax = 500,
            IList<TUTValue> tutToChooseAmong = null, IList<IdTypeValue> techniquesId = null)
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
                    effort = effortType == TrainingEffortTypeEnum.RPE ? TrainingEffortValue.AsRM(10) : effort; // Couldn't resolve this
                    //effort = effortType == TrainingEffortTypeEnum.IntensityPerc ? effort.ToIntensityPercentage(serie) : effort.ToRm(serie);
                }
                else
                    serie = WSRepetitionValue.TrackRepetitionSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));
            }
            else
                serie = WSRepetitionValue.TrackTimedSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));

            return WorkingSetTemplate.AddWorkingSet(
                new IdTypeValue(id),
                (uint)progn,
                serie,
                RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax)),
                effort,
                tutToChooseAmong == null ? null : tutToChooseAmong[RandomFieldGenerator.RandomInt(0, tutToChooseAmong.Count - 1)],
                techniquesId
            );
        }

    }
}
