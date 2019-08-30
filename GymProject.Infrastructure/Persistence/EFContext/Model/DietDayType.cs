using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class DietDayType
    {
        public DietDayType()
        {
            DietDay = new HashSet<DietDay>();
            DietDayExample = new HashSet<DietDayExample>();
            DietPlanDay = new HashSet<DietPlanDay>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DietDay> DietDay { get; set; }
        public virtual ICollection<DietDayExample> DietDayExample { get; set; }
        public virtual ICollection<DietPlanDay> DietPlanDay { get; set; }
    }
}
