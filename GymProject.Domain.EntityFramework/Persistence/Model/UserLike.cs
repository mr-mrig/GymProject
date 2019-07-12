using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class UserLike
    {
        public long PostId { get; set; }
        public long UserId { get; set; }
        public long? Value { get; set; }
        public long CreatedOn { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
