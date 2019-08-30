using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class MealExample
    {
        public MealExample()
        {
            DietDayMealExample = new HashSet<DietDayMealExample>();
            MealExampleHasFood = new HashSet<MealExampleHasFood>();
        }

        public long Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DietDayMealExample> DietDayMealExample { get; set; }
        public virtual ICollection<MealExampleHasFood> MealExampleHasFood { get; set; }
    }
}
