using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class DietPlan
    {
        public DietPlan()
        {
            DietHasHashtag = new HashSet<DietHasHashtag>();
            DietPlanDayExample = new HashSet<DietPlanDayExample>();
            DietPlanUnit = new HashSet<DietPlanUnit>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? WeeklyFreeMealsNumber { get; set; }
        public long CreatedOn { get; set; }
        public string OwnerNote { get; set; }
        public long? OwnerId { get; set; }

        public virtual Post IdNavigation { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<DietHasHashtag> DietHasHashtag { get; set; }
        public virtual ICollection<DietPlanDayExample> DietPlanDayExample { get; set; }
        public virtual ICollection<DietPlanUnit> DietPlanUnit { get; set; }
    }
}
