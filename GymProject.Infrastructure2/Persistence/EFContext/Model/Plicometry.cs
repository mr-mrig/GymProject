using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class Plicometry
    {
        public long Id { get; set; }
        public long? Chest { get; set; }
        public long? Tricep { get; set; }
        public long? Armpit { get; set; }
        public long? Subscapular { get; set; }
        public long? Suprailiac { get; set; }
        public long? Abdomen { get; set; }
        public long? Thigh { get; set; }
        public long? Kg { get; set; }
        public long? Bf { get; set; }
        public long? OwnerId { get; set; }

        public virtual MeasuresEntry IdNavigation { get; set; }
        public virtual User Owner { get; set; }
    }
}
