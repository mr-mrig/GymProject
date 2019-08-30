using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class LinkedWorkUnitTemplate
    {
        public long FirstWorkUnitId { get; set; }
        public long SecondWorkUnitId { get; set; }
        public long IntensityTechniqueId { get; set; }

        public virtual WorkUnitTemplate FirstWorkUnit { get; set; }
        public virtual IntensityTechnique IntensityTechnique { get; set; }
        public virtual WorkUnitTemplate SecondWorkUnit { get; set; }
    }
}
