using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class PerformanceType
    {
        public PerformanceType()
        {
            Excercise = new HashSet<Excercise>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Excercise> Excercise { get; set; }
    }
}
