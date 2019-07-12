using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TrainingCollaboration
    {
        public long Id { get; set; }
        public long StartDate { get; set; }
        public long? EndDate { get; set; }
        public long? ExpirationDate { get; set; }
        public string TrainerNote { get; set; }
        public string TraineeNote { get; set; }
        public long TrainerId { get; set; }
        public long TraineeId { get; set; }

        public virtual User Trainee { get; set; }
        public virtual Trainer Trainer { get; set; }
    }
}
