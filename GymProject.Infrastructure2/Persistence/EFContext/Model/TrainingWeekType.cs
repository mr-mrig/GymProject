using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingWeekType
    {
        public TrainingWeekType()
        {
            TrainingWeek = new HashSet<TrainingWeek>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrainingWeek> TrainingWeek { get; set; }
    }
}
