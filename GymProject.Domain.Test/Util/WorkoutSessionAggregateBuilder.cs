using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Infrastructure.Utils;
using System.Collections.Generic;

namespace GymProject.Domain.Test.Util
{
    internal class WorkoutSessionAggregateBuilder
    {




        internal static WorkUnitEntity BuildRandomWorkUnit(uint id, uint progn, bool isTransient, int wsNum = -1, uint? excerciseId = 0, uint? repetitions = 0)
        {
            int excerciseIdMin = 1, excerciseIdMax = 500;
            int wsNumMin = 3, wsNumMax = 7;

            List<WorkingSetEntity> workingSets = new List<WorkingSetEntity>();

            if (excerciseId != null && excerciseId == 0)
                excerciseId = (uint?)RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax);

            // Add randomic Working Sets
            if (wsNum < 0)
                wsNum = RandomFieldGenerator.RandomInt(wsNumMin, wsNumMax);

            for (int iws = 0; iws < wsNum; iws++)
            {
                //long strongId = ((id - 1) * wsIdOffset + iws + 1);      // Easiest to read
                uint strongId = (uint)((id - 1) * wsNumMax + iws + 1);      // Smallest possible

                workingSets.Add(BuildRandomWorkingSet(strongId, iws, isTransient, repetitions));
            }

            if (isTransient)

                return WorkUnitEntity.TrackExcercise(
                    id: null,
                    progressiveNumber: (uint)progn,
                    excerciseId: (uint?)(excerciseId),
                    workingSets: workingSets,
                    userRating: RatingValue.Rate(RandomFieldGenerator.RandomFloat(RatingValue.MinimumValue, RatingValue.MaximumValue - 1))
                );
            else

                return WorkUnitEntity.TrackExcercise(
                    id: id,
                    progressiveNumber: (uint)progn,
                    excerciseId: (uint?)(excerciseId),
                    workingSets: workingSets,
                    userRating: RatingValue.Rate(RandomFieldGenerator.RandomFloat(RatingValue.MinimumValue, RatingValue.MaximumValue - 1))
                );
        }


        internal static WorkingSetEntity BuildRandomWorkingSet(uint id, int progn, bool isTransient, uint? repetitions = 0)
        {
            int noteIdMin = 32451, noteIdMax = 88888;
            float loadMin = 0, loadMax = 200;
            int repsMin = 1, repsMax = 25;

            WSRepetitionsValue serie;

            if (repetitions == null)
                serie = null;

            else if (repetitions < 1)
                serie = WSRepetitionsValue.TrackRepetitionSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));

            else
                serie = WSRepetitionsValue.TrackRepetitionSerie(repetitions.Value);

            WeightPlatesValue load = WeightPlatesValue.MeasureKilograms(RandomFieldGenerator.RandomFloat(loadMin, loadMax));
            uint? noteId = (uint?)RandomFieldGenerator.RandomIntNullable(noteIdMin, noteIdMax);

            if (isTransient)

                return WorkingSetEntity.TrackWorkingSet(null, (uint)progn, serie, load, noteId);
            else

                return WorkingSetEntity.TrackWorkingSet(id, (uint)progn, serie, load, noteId);
        }


    }
}
