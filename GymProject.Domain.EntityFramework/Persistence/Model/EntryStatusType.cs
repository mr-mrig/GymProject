using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class EntryStatusType
    {
        public EntryStatusType()
        {
            DietHashtag = new HashSet<DietHashtag>();
            Excercise = new HashSet<Excercise>();
            Food = new HashSet<Food>();
            Hashtag = new HashSet<Hashtag>();
            IntensityTechnique = new HashSet<IntensityTechnique>();
            Mus = new HashSet<Mus>();
            Phase = new HashSet<Phase>();
            TraineeHashtag = new HashSet<TraineeHashtag>();
            TrainingHashtag = new HashSet<TrainingHashtag>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DietHashtag> DietHashtag { get; set; }
        public virtual ICollection<Excercise> Excercise { get; set; }
        public virtual ICollection<Food> Food { get; set; }
        public virtual ICollection<Hashtag> Hashtag { get; set; }
        public virtual ICollection<IntensityTechnique> IntensityTechnique { get; set; }
        public virtual ICollection<Mus> Mus { get; set; }
        public virtual ICollection<Phase> Phase { get; set; }
        public virtual ICollection<TraineeHashtag> TraineeHashtag { get; set; }
        public virtual ICollection<TrainingHashtag> TrainingHashtag { get; set; }
    }
}
