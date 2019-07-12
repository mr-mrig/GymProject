using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class MeasuresEntry
    {
        public long Id { get; set; }
        public long MeasureDate { get; set; }
        public string OwnerNote { get; set; }
        public long? Rating { get; set; }
        public long OwnerId { get; set; }

        public virtual Post IdNavigation { get; set; }
        public virtual User Owner { get; set; }
        public virtual BiaEntry BiaEntry { get; set; }
        public virtual Circumference Circumference { get; set; }
        public virtual Plicometry Plicometry { get; set; }
    }
}
