using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class ExerciseFocus
    {
        public long PerformanceFocusId { get; set; }
        public long ExerciseId { get; set; }

        public virtual Excercise Exercise { get; set; }
        public virtual PerformanceFocus PerformanceFocus { get; set; }
    }
}
