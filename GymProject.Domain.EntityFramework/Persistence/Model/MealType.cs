using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class MealType
    {
        public MealType()
        {
            DietDayMealExample = new HashSet<DietDayMealExample>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DietDayMealExample> DietDayMealExample { get; set; }
    }
}
