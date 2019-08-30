using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class WellnessDay
    {
        public WellnessDay()
        {
            WellnessDayHasMus = new HashSet<WellnessDayHasMus>();
        }

        public long Id { get; set; }
        public double? Temperature { get; set; }
        public long? Glycemia { get; set; }

        public virtual FitnessDayEntry IdNavigation { get; set; }
        public virtual ICollection<WellnessDayHasMus> WellnessDayHasMus { get; set; }
    }
}
