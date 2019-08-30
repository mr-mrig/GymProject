using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class DietDayMealExample
    {
        public long DietDayExampleId { get; set; }
        public long MealExampleId { get; set; }
        public long? MealTypeId { get; set; }

        public virtual DietDayExample DietDayExample { get; set; }
        public virtual MealExample MealExample { get; set; }
        public virtual MealType MealType { get; set; }
    }
}
