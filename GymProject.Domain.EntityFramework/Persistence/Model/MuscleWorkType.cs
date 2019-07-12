using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class MuscleWorkType
    {
        public MuscleWorkType()
        {
            ExcerciseSecondaryTarget = new HashSet<ExcerciseSecondaryTarget>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long DefaultEffectivness { get; set; }

        public virtual ICollection<ExcerciseSecondaryTarget> ExcerciseSecondaryTarget { get; set; }
    }
}
