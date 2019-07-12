using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class Image
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public long IsProgressPicture { get; set; }
        public long PostId { get; set; }

        public virtual Post Post { get; set; }
    }
}
