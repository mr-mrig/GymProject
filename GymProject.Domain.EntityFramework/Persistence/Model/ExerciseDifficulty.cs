using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class ExerciseDifficulty
    {
        public ExerciseDifficulty()
        {
            Excercise = new HashSet<Excercise>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Excercise> Excercise { get; set; }
    }
}
