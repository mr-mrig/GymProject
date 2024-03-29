﻿using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class MealExampleHasFood
    {
        public long MealExampleId { get; set; }
        public long FoodId { get; set; }

        public virtual Food Food { get; set; }
        public virtual MealExample MealExample { get; set; }
    }
}
