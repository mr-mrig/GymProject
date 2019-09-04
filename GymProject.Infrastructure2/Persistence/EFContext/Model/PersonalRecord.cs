using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class PersonalRecord
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ExcerciseId { get; set; }
        public long RecordDate { get; set; }
        public long Value { get; set; }

        public virtual Excercise Excercise { get; set; }
        public virtual User User { get; set; }
    }
}
