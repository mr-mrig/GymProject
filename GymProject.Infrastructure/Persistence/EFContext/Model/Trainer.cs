using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class Trainer
    {
        public Trainer()
        {
            TraineeHasHashtag = new HashSet<TraineeHasHashtag>();
            TrainingCollaboration = new HashSet<TrainingCollaboration>();
        }

        public long Id { get; set; }
        public string ExperienceSummary { get; set; }

        public virtual User IdNavigation { get; set; }
        public virtual ICollection<TraineeHasHashtag> TraineeHasHashtag { get; set; }
        public virtual ICollection<TrainingCollaboration> TrainingCollaboration { get; set; }
    }
}
