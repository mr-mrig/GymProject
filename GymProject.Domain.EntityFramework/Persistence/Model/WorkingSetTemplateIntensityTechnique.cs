using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class WorkingSetTemplateIntensityTechnique
    {
        public long SetTemplateId { get; set; }
        public long IntensityTechniqueId { get; set; }
        public long? LinkedSetTemplateId { get; set; }

        public virtual IntensityTechnique IntensityTechnique { get; set; }
        public virtual WorkingSetTemplate LinkedSetTemplate { get; set; }
        public virtual WorkingSetTemplate SetTemplate { get; set; }
    }
}
