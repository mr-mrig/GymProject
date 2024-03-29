﻿using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TrainingPlanTargetProficiency
    {
        public long TrainingPlanId { get; set; }
        public long TrainingProficiencyId { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }
        public virtual TrainingProficiency TrainingProficiency { get; set; }
    }
}
