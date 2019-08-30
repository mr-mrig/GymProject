using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class RelationStatus
    {
        public RelationStatus()
        {
            UserRelation = new HashSet<UserRelation>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<UserRelation> UserRelation { get; set; }
    }
}
