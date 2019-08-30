using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class WorkingSetNote
    {
        public long Id { get; set; }
        public string Body { get; set; }
        public long WorkingSetId { get; set; }

        public virtual WorkingSet WorkingSet { get; set; }
    }
}
