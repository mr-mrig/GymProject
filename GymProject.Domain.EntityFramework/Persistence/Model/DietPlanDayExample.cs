﻿using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class DietPlanDayExample
    {
        public long DietPlanId { get; set; }
        public long DietDayExampleId { get; set; }

        public virtual DietDayExample DietDayExample { get; set; }
        public virtual DietPlan DietPlan { get; set; }
    }
}
