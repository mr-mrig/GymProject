using GymProject.Domain.TrainingDomain.AthleteAggregate;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GymProject.Domain.Test.Util
{
    public class UserTrainingPlanEqualityComparer : IEqualityComparer<UserTrainingPlanEntity>
    {

        public bool Equals([AllowNull] UserTrainingPlanEntity x, [AllowNull] UserTrainingPlanEntity y)
        {
            int differences = 0;

            if(x == null)
            {
                if (y == null)
                    return true;
                else
                    return false;
            }

            if (y == null)
                return false;

            if(!x.IsTransient() && !y.IsTransient())
                differences = differences + x.Id == y.Id ? 0 : 1;

            differences += x.Name == y.Name ? 0 : 1;
            differences += x.ParentPlanId == y.ParentPlanId ? 0 : 1;
            differences += x.TrainingPlanNoteId == y.TrainingPlanNoteId ? 0 : 1;
            differences += x.TrainingPlanId == y.TrainingPlanId ? 0 : 1;
            differences += x.IsBookmarked == y.IsBookmarked ? 0 : 1;
            differences += x.HashtagIds.SequenceEqual(y.HashtagIds) ? 0 : 1;
            differences += x.MuscleFocusIds.SequenceEqual(y.MuscleFocusIds) ? 0 : 1;
            differences += x.TrainingPhaseIds.SequenceEqual(y.TrainingPhaseIds) ? 0 : 1;
            differences += x.TrainingProficiencyIds.SequenceEqual(y.TrainingProficiencyIds) ? 0 : 1;

            return differences == 0;
        }


        public int GetHashCode([DisallowNull] UserTrainingPlanEntity obj)
        {
            return GetHashCode();
        }
    }
}
