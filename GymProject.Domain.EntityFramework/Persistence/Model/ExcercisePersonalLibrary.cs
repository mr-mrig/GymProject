using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class ExcercisePersonalLibrary
    {
        public long UserId { get; set; }
        public long ExcerciseId { get; set; }
        public long IsStarred { get; set; }

        public virtual Excercise Excercise { get; set; }
        public virtual User User { get; set; }
    }
}
