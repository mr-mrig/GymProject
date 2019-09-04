using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class Comment
    {
        public long Id { get; set; }
        public string Body { get; set; }
        public long CreatedOn { get; set; }
        public string LastUpdate { get; set; }
        public long UserId { get; set; }
        public long PostId { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
