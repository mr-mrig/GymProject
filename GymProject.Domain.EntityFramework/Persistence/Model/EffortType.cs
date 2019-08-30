using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class EffortType
    {
        public EffortType()
        {
            WorkingSetTemplate = new HashSet<WorkingSetTemplate>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WorkingSetTemplate> WorkingSetTemplate { get; set; }
    }
}
