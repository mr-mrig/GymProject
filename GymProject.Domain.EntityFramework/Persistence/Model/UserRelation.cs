using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class UserRelation
    {
        public long SourceUserId { get; set; }
        public long TargetUserId { get; set; }
        public long StartDate { get; set; }
        public long RelationStatusId { get; set; }

        public virtual RelationStatus RelationStatus { get; set; }
        public virtual User SourceUser { get; set; }
        public virtual User TargetUser { get; set; }
    }
}
