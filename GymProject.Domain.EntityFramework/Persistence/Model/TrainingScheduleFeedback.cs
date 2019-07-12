using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TrainingScheduleFeedback
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public long LastUpdate { get; set; }
        public long? Rating { get; set; }
        public long TrainingScheduleId { get; set; }
        public long UserId { get; set; }

        public virtual TrainingSchedule TrainingSchedule { get; set; }
        public virtual User User { get; set; }
    }
}
