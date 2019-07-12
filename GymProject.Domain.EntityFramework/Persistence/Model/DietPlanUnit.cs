using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class DietPlanUnit
    {
        public DietPlanUnit()
        {
            DietPlanDay = new HashSet<DietPlanDay>();
        }

        public long Id { get; set; }
        public long StartDate { get; set; }
        public long? EndDatePlanned { get; set; }
        public long? EndDate { get; set; }
        public long DietPlanId { get; set; }

        public virtual DietPlan DietPlan { get; set; }
        public virtual ICollection<DietPlanDay> DietPlanDay { get; set; }
    }
}
