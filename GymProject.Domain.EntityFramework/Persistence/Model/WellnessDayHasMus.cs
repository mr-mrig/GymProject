using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class WellnessDayHasMus
    {
        public long WellnessDayId { get; set; }
        public long MusId { get; set; }

        public virtual Mus Mus { get; set; }
        public virtual WellnessDay WellnessDay { get; set; }
    }
}
