using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class PostHasHashtag
    {
        public long PostId { get; set; }
        public long HashtagId { get; set; }
        public long ProgressiveNumber { get; set; }

        public virtual Hashtag Hashtag { get; set; }
        public virtual Post Post { get; set; }
    }
}
