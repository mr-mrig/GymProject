using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.Utils;
using GymProject.Infrastructure.Utils;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GymProject.Domain.Test.Util
{
    internal class WorkoutTemplateAggregateBuilder
    {




        internal static TrainingWeekEntity BuildRandomTrainingWeek(long id, int progn, bool isTransient,
            int nWorkoutsMin = 3, int nWorkoutsMax = 9, TrainingWeekTypeEnum weekType = null, float noWorkoutsProb = 0.05f)
        {
            float workoutWithNoWorkingSetsProbability = 0.05f;
            int workingSetsMin = 10, workingSetsMax = 30;

            // Workouts
            List<uint?> workoutsIds = new List<uint?>();

            // Week Type
            if (weekType == null)
            {
                int? weekTypeId = RandomFieldGenerator.RandomIntNullable(0, TrainingWeekTypeEnum.Peak.Id, 0.1f);

                if (weekTypeId == null)
                    weekType = TrainingWeekTypeEnum.Generic;
                else
                    weekType = TrainingWeekTypeEnum.From(weekTypeId.Value);
            }

            // Buil Workouts
            int? nWorkouts = RandomFieldGenerator.RandomIntNullable(nWorkoutsMin, nWorkoutsMax, noWorkoutsProb);

            if (nWorkouts == null || weekType == TrainingWeekTypeEnum.FullRest)
                nWorkouts = 0;

            for (int iwo = 0; iwo < nWorkouts; iwo++)
            {
                //long strongId = ((id - 1) * wuIdOffset + iwu + 1);      // Easiest to read
                uint strongId = (uint)((id - 1) * nWorkoutsMax + iwo + 1);      // Smallest possible

                // Build working sets
                int workingSetsNumber = RandomFieldGenerator.RollEventWithProbability(workoutWithNoWorkingSetsProbability)
                    ? 0
                    : RandomFieldGenerator.RandomInt(workingSetsMin, workingSetsMax);

                List<WorkingSetTemplateEntity> workoutSets = new List<WorkingSetTemplateEntity>();

                for (uint iws = 0; iws < workingSetsNumber; iws++)
                {
                    TrainingEffortTypeEnum effortType = TrainingEffortTypeEnum.From(RandomFieldGenerator.RandomInt(1, 3));

                    workoutSets.Add(BuildRandomWorkingSetTemplate(strongId * iws + 1, iwo, isTransient, effortType));
                }

                //if (workoutSets.ContainsDuplicates())
                //    System.Diagnostics.Debugger.Break();

                workoutsIds.Add((uint?)RandomFieldGenerator.RandomIntValueExcluded(1, 1234123, workoutsIds.Select(x => (int)x.Value)));
            }

            // Create the Week
            if (isTransient)
                return TrainingWeekEntity.PlanTransientTrainingWeek((uint)progn, workoutsIds, weekType);

            else

                return TrainingWeekEntity.PlanTrainingWeek((uint?)(id), (uint)progn, workoutsIds, weekType);
        }


        internal static WorkoutTemplateRoot BuildRandomWorkoutTemplate(long id, uint progressiveNumber, bool isTransient, uint? trainingWeekId = null,
            int nWorkUnitsMin = 3, int nWorkUnitsMax = 7, WeekdayEnum specificDay = null, float emptyWorkoutProb = 0.05f)
        {
            // Work Units
            List<WorkUnitTemplateEntity> workUnits = new List<WorkUnitTemplateEntity>();

            int? nWorkUnits = RandomFieldGenerator.RandomIntNullable(nWorkUnitsMin, nWorkUnitsMax, emptyWorkoutProb);

            if (nWorkUnits == null)
                nWorkUnits = 0;


            for (uint iwu = 0; iwu < nWorkUnits; iwu++)
            {
                //long strongId = ((id - 1) * wuIdOffset + iwu + 1);      // Easiest to read
                uint strongId = (uint)((id - 1) * nWorkUnitsMax + iwu + 1);      // Smallest possible

                workUnits.Add(BuildRandomWorkUnitTemplate(strongId, iwu, isTransient));
            }

            // Specific Day
            if (specificDay == null)
            {
                int? weekDayId = RandomFieldGenerator.RandomIntNullable(WeekdayEnum.Monday.Id, WeekdayEnum.AllTheWeek, 0.1f);

                if (weekDayId == null)
                    specificDay = WeekdayEnum.Generic;
                else
                    specificDay = WeekdayEnum.From(weekDayId.Value);
            }

            if (isTransient)

                return WorkoutTemplateRoot.PlanTransientWorkout(
                    progressiveNumber: progressiveNumber,
                    workUnits: workUnits,
                    workoutName: RandomFieldGenerator.RandomTextValue(4, 5, true, 0.05f),
                    weekday: specificDay
                    );
            else
                return WorkoutTemplateRoot.PlanWorkout(
                    id: (uint?)(id),
                    trainingWeekId: trainingWeekId ?? (uint?)RandomFieldGenerator.RandomInt(1, 1000),
                    progressiveNumber: progressiveNumber,
                    workUnits: workUnits,
                    workoutName: RandomFieldGenerator.RandomTextValue(4, 5, true, 0.05f),
                    weekday: specificDay
                    );
        }



        internal static WorkUnitTemplateEntity BuildRandomWorkUnitTemplate(uint id, uint progn, bool isTransient,
            int wsNumMin = 2, int wsNumMax = 7, int excerciseIdMin = 1, int excerciseIdMax = 500,
            int ownerNoteIdMin = 10, int ownerNoteIdMax = 500)
        {
            List<WorkingSetTemplateEntity> workingSets = new List<WorkingSetTemplateEntity>();
            TrainingEffortTypeEnum effortType;
            LinkedWorkValue linkedWorkUnit;

            int wuIntTechniquesMin = 0, wuIntTechniquesMax = 3;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;
            int linkedIdMin = 1, linkedIdMax = 1000;


            //int wsIdOffset = 100;       // Used to ensure no ID collisions among WSs of different WUs

            // Build randomic Effort
            if (id % 2 == 0)
                effortType = TrainingEffortTypeEnum.IntensityPercentage;

            else if (id % 3 == 0)
                effortType = TrainingEffortTypeEnum.RM;

            else
                effortType = TrainingEffortTypeEnum.RPE;

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

            // Add randomic Working Sets
            int iwsMax = RandomFieldGenerator.RandomInt(wsNumMin, wsNumMax);

            for (int iws = 0; iws < iwsMax; iws++)
            {
                //long strongId = ((id - 1) * wsIdOffset + iws + 1);      // Easiest to read
                uint strongId = (uint)((id - 1) * wsNumMax + iws + 1);      // Smallest possible

                workingSets.Add(BuildRandomWorkingSetTemplate(strongId, iws, isTransient, effortType, tutToChooseAmong: tuts));
            }

            if (isTransient)

                return WorkUnitTemplateEntity.PlanTransientWorkUnit(
                progressiveNumber: progn,
                excerciseId: (uint?)(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax)),
                workingSets: workingSets,
                linkingIntensityTechniqueId: null,
                ownerNoteId: (uint?)(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
            );
            else
            {
                if(RandomFieldGenerator.RollEventWithProbability(0.33f))
                {
                    //if(RandomFieldGenerator.RollEventWithProbability(0.33f))
                    //{
                    //    // Fake it
                    //    Assert.Throws<TrainingDomainInvariantViolationException>(() =>

                    //         WorkUnitTemplateEntity.PlanWorkUnit(
                    //            id: id,
                    //            progressiveNumber: progn,
                    //            excerciseId: (uint?)RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax),
                    //            workingSets: workingSets,
                    //            linkedWorkUnitId: id,         // Fake value
                    //            linkingIntensityTechniqueId: 1,
                    //            ownerNoteId: (uint?)(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
                    //        ));
                    //}

                    // Linked WU
                    return WorkUnitTemplateEntity.PlanWorkUnit(
                        id: id,
                        progressiveNumber: progn,
                        excerciseId: (uint?)RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax),
                        workingSets: workingSets,
                        //linkedWorkUnitId: (uint)RandomFieldGenerator.RandomIntValueExcluded(linkedIdMin, linkedIdMax, (int)id),
                        linkingIntensityTechniqueId: (uint)RandomFieldGenerator.RandomInt(intTechniqueIdMin, intTechniqueIdMax),
                        ownerNoteId: (uint?)(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
                    );
                }

                return WorkUnitTemplateEntity.PlanWorkUnit(
                    id: id,
                    progressiveNumber: (uint)progn,
                    excerciseId: (uint?)RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax),
                    workingSets: workingSets,
                    linkingIntensityTechniqueId: null,
                    ownerNoteId: (uint?)(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
                );
            }
        }


        internal static WorkingSetTemplateEntity BuildRandomWorkingSetTemplate(uint id, int progn, bool isTransient, TrainingEffortTypeEnum effortType,
            int repsMin = 1, int repsMax = 20, bool repetitionsSerie = true, float amrapProbability = 0.1f, int restMin = 5, int restMax = 500,
            IList<TUTValue> tutToChooseAmong = null, IList<uint?> techniquesId = null)
        {
            float rpeMin = 1, rpeMax = 11;
            float rmMin = 1, rmMax = 20;
            float intPercMin = 50, intPercMax = 105;

            TrainingEffortValue effort;
            WSRepetitionsValue serie;

            switch (effortType)
            {
                case var _ when effortType == TrainingEffortTypeEnum.IntensityPercentage:

                    effort = TrainingEffortValue.AsIntensityPerc(RandomFieldGenerator.RandomFloat(intPercMin, intPercMax));
                    break;

                case var _ when effortType == TrainingEffortTypeEnum.RM:

                    effort = TrainingEffortValue.AsRM(RandomFieldGenerator.RandomFloat(rmMin, rmMax));
                    break;

                case var _ when effortType == TrainingEffortTypeEnum.RPE:

                    effort = TrainingEffortValue.AsRPE((float)CommonUtilities.RoundToPointFive(RandomFieldGenerator.RandomFloat(rpeMin, rpeMax)));
                    break;

                default:

                    effort = null;
                    break;
            }

            if (repetitionsSerie)
            {
                if (RandomFieldGenerator.RandomDouble(0, 1) < amrapProbability)
                {
                    serie = WSRepetitionsValue.TrackAMRAP();
                    effort = effortType == TrainingEffortTypeEnum.RPE ? TrainingEffortValue.AsRM(10) : effort; // Couldn't resolve this
                    //effort = effortType == TrainingEffortTypeEnum.IntensityPerc ? effort.ToIntensityPercentage(serie) : effort.ToRm(serie);
                }
                else
                    serie = WSRepetitionsValue.TrackRepetitionSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));
            }
            else
                serie = WSRepetitionsValue.TrackTimedSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));

            if (isTransient)

                return WorkingSetTemplateEntity.PlanTransientWorkingSet(
                    (uint)progn,
                    serie,
                    RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax)),
                    effort,
                    tutToChooseAmong == null ? null : tutToChooseAmong[RandomFieldGenerator.RandomInt(0, tutToChooseAmong.Count - 1)],
                    techniquesId
                );
            else

                return WorkingSetTemplateEntity.PlanWorkingSet(
                    id,
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
