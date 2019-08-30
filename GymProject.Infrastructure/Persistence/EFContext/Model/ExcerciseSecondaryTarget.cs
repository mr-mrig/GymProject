using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class ExcerciseSecondaryTarget
    {
        public long ExcerciseId { get; set; }
        public long MuscleId { get; set; }
        public long? MuscleWorkTypeId { get; set; }

        public virtual Excercise Excercise { get; set; }
        public virtual Muscle Muscle { get; set; }
        public virtual MuscleWorkType MuscleWorkType { get; set; }
    }
}
