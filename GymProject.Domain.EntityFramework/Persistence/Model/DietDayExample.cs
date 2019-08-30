using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class DietDayExample
    {
        public DietDayExample()
        {
            DietDayMealExample = new HashSet<DietDayMealExample>();
            DietPlanDayExample = new HashSet<DietPlanDayExample>();
        }

        public long Id { get; set; }
        public string Introduction { get; set; }
        public string PrivateNote { get; set; }
        public long? DietDayTypeId { get; set; }

        public virtual DietDayType DietDayType { get; set; }
        public virtual ICollection<DietDayMealExample> DietDayMealExample { get; set; }
        public virtual ICollection<DietPlanDayExample> DietPlanDayExample { get; set; }
    }
}
