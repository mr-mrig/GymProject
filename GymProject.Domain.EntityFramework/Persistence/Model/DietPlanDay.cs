using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class DietPlanDay
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CarbGrams { get; set; }
        public long? FatGrams { get; set; }
        public long? ProteinGrams { get; set; }
        public long? SaltGrams { get; set; }
        public long? WaterLiters { get; set; }
        public long? SpecificWeekDay { get; set; }
        public long DietPlanUnitId { get; set; }
        public long DietDayTypeId { get; set; }

        public virtual DietDayType DietDayType { get; set; }
        public virtual DietPlanUnit DietPlanUnit { get; set; }
    }
}
