using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class PerformanceFocus
    {
        public PerformanceFocus()
        {
            ExerciseFocus = new HashSet<ExerciseFocus>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ExerciseFocus> ExerciseFocus { get; set; }
    }
}
