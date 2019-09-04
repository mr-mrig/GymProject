using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class BiaDeviceBrand
    {
        public BiaDeviceBrand()
        {
            BiaDevice = new HashSet<BiaDevice>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BiaDevice> BiaDevice { get; set; }
    }
}
