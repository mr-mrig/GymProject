using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class Muscle
    {
        public Muscle()
        {
            Excercise = new HashSet<Excercise>();
            ExcerciseSecondaryTarget = new HashSet<ExcerciseSecondaryTarget>();
            TrainingMuscleFocus = new HashSet<TrainingMuscleFocus>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public virtual ICollection<Excercise> Excercise { get; set; }
        public virtual ICollection<ExcerciseSecondaryTarget> ExcerciseSecondaryTarget { get; set; }
        public virtual ICollection<TrainingMuscleFocus> TrainingMuscleFocus { get; set; }
    }
}
