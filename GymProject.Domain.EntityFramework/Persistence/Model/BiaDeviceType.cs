using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class BiaDeviceType
    {
        public BiaDeviceType()
        {
            BiaDevice = new HashSet<BiaDevice>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BiaDevice> BiaDevice { get; set; }
    }
}
