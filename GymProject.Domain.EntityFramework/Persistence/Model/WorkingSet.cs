using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class WorkingSet
    {
        public WorkingSet()
        {
            WorkingSetNote = new HashSet<WorkingSetNote>();
        }

        public long Id { get; set; }
        public long WorkUnitId { get; set; }
        public long ProgressiveNumber { get; set; }
        public long? Repetitions { get; set; }
        public long? Kg { get; set; }

        public virtual WorkUnit WorkUnit { get; set; }
        public virtual ICollection<WorkingSetNote> WorkingSetNote { get; set; }
    }
}
