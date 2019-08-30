using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class Mus
    {
        public Mus()
        {
            WellnessDayHasMus = new HashSet<WellnessDayHasMus>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long EntryStatusTypeId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual ICollection<WellnessDayHasMus> WellnessDayHasMus { get; set; }
    }
}
