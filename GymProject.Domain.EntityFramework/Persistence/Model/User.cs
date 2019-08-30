using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class User
    {
        public User()
        {
            BiaEntry = new HashSet<BiaEntry>();
            Circumference = new HashSet<Circumference>();
            Comment = new HashSet<Comment>();
            DietHashtag = new HashSet<DietHashtag>();
            DietPlan = new HashSet<DietPlan>();
            ExcercisePersonalLibrary = new HashSet<ExcercisePersonalLibrary>();
            Hashtag = new HashSet<Hashtag>();
            MeasuresEntry = new HashSet<MeasuresEntry>();
            PersonalRecord = new HashSet<PersonalRecord>();
            Phase = new HashSet<Phase>();
            Plicometry = new HashSet<Plicometry>();
            Post = new HashSet<Post>();
            TraineeHasHashtag = new HashSet<TraineeHasHashtag>();
            TraineeHashtag = new HashSet<TraineeHashtag>();
            TrainingCollaboration = new HashSet<TrainingCollaboration>();
            TrainingHashtag = new HashSet<TrainingHashtag>();
            TrainingPlan = new HashSet<TrainingPlan>();
            TrainingScheduleFeedback = new HashSet<TrainingScheduleFeedback>();
            UserHasProficiencyOwner = new HashSet<UserHasProficiency>();
            UserHasProficiencyProficiency = new HashSet<UserHasProficiency>();
            UserHasProficiencyUser = new HashSet<UserHasProficiency>();
            UserLike = new HashSet<UserLike>();
            UserPhase = new HashSet<UserPhase>();
            UserRelationSourceUser = new HashSet<UserRelation>();
            UserRelationTargetUser = new HashSet<UserRelation>();
        }

        public long Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public long SubscriptionDate { get; set; }
        public long? AccountStatusTypeId { get; set; }

        public virtual AccountStatusType AccountStatusType { get; set; }
        public virtual Trainer Trainer { get; set; }
        public virtual UserDetail UserDetail { get; set; }
        public virtual ICollection<BiaEntry> BiaEntry { get; set; }
        public virtual ICollection<Circumference> Circumference { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<DietHashtag> DietHashtag { get; set; }
        public virtual ICollection<DietPlan> DietPlan { get; set; }
        public virtual ICollection<ExcercisePersonalLibrary> ExcercisePersonalLibrary { get; set; }
        public virtual ICollection<Hashtag> Hashtag { get; set; }
        public virtual ICollection<MeasuresEntry> MeasuresEntry { get; set; }
        public virtual ICollection<PersonalRecord> PersonalRecord { get; set; }
        public virtual ICollection<Phase> Phase { get; set; }
        public virtual ICollection<Plicometry> Plicometry { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<TraineeHasHashtag> TraineeHasHashtag { get; set; }
        public virtual ICollection<TraineeHashtag> TraineeHashtag { get; set; }
        public virtual ICollection<TrainingCollaboration> TrainingCollaboration { get; set; }
        public virtual ICollection<TrainingHashtag> TrainingHashtag { get; set; }
        public virtual ICollection<TrainingPlan> TrainingPlan { get; set; }
        public virtual ICollection<TrainingScheduleFeedback> TrainingScheduleFeedback { get; set; }
        public virtual ICollection<UserHasProficiency> UserHasProficiencyOwner { get; set; }
        public virtual ICollection<UserHasProficiency> UserHasProficiencyProficiency { get; set; }
        public virtual ICollection<UserHasProficiency> UserHasProficiencyUser { get; set; }
        public virtual ICollection<UserLike> UserLike { get; set; }
        public virtual ICollection<UserPhase> UserPhase { get; set; }
        public virtual ICollection<UserRelation> UserRelationSourceUser { get; set; }
        public virtual ICollection<UserRelation> UserRelationTargetUser { get; set; }
    }
}
