﻿using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class ExcerciseRelation
    {
        public long ParentExcerciseId { get; set; }
        public long ChildExcerciseId { get; set; }
        public string AdditionalNotes { get; set; }

        public virtual Excercise ChildExcercise { get; set; }
        public virtual Excercise ParentExcercise { get; set; }
    }
}
