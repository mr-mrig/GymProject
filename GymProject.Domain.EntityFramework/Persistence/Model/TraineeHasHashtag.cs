using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TraineeHasHashtag
    {
        public long HashtagId { get; set; }
        public long TraineeId { get; set; }
        public long TrainerId { get; set; }
        public long ProgressiveNumber { get; set; }

        public virtual TraineeHashtag Hashtag { get; set; }
        public virtual User Trainee { get; set; }
        public virtual Trainer Trainer { get; set; }
    }
}
