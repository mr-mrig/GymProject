﻿using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class Weight
    {
        public long Id { get; set; }
        public long Kg { get; set; }

        public virtual Post Id1 { get; set; }
        public virtual FitnessDayEntry IdNavigation { get; set; }
    }
}
