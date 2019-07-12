using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class Post
    {
        public Post()
        {
            Comment = new HashSet<Comment>();
            Image = new HashSet<Image>();
            PostHasHashtag = new HashSet<PostHasHashtag>();
            UserLike = new HashSet<UserLike>();
        }

        public long Id { get; set; }
        public string Caption { get; set; }
        public string IsPublic { get; set; }
        public long CreatedOn { get; set; }
        public long? LastUpdate { get; set; }
        public long UserId { get; set; }

        public virtual User User { get; set; }
        public virtual DietPlan DietPlan { get; set; }
        public virtual FitnessDayEntry FitnessDayEntry { get; set; }
        public virtual MeasuresEntry MeasuresEntry { get; set; }
        public virtual TrainingSchedule TrainingSchedule { get; set; }
        public virtual UserPhase UserPhase { get; set; }
        public virtual Weight Weight { get; set; }
        public virtual WorkoutSession WorkoutSession { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Image> Image { get; set; }
        public virtual ICollection<PostHasHashtag> PostHasHashtag { get; set; }
        public virtual ICollection<UserLike> UserLike { get; set; }
    }
}
