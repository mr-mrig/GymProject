using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class BiaDevice
    {
        public BiaDevice()
        {
            BiaEntry = new HashSet<BiaEntry>();
        }

        public long Id { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public long BrandId { get; set; }
        public long DeviceTypeId { get; set; }

        public virtual BiaDeviceBrand Brand { get; set; }
        public virtual BiaDeviceType DeviceType { get; set; }
        public virtual ICollection<BiaEntry> BiaEntry { get; set; }
    }
}
