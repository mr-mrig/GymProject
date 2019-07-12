using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class FitnessDayEntry
    {
        public long Id { get; set; }
        public long DayDate { get; set; }
        public long? Rating { get; set; }

        public virtual Post IdNavigation { get; set; }
        public virtual ActivityDay ActivityDay { get; set; }
        public virtual DietDay DietDay { get; set; }
        public virtual Weight Weight { get; set; }
        public virtual WellnessDay WellnessDay { get; set; }
    }
}
