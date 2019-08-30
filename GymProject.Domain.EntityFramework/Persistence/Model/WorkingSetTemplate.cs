using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class WorkingSetTemplate
    {
        public WorkingSetTemplate()
        {
            WorkingSetTemplateIntensityTechniqueLinkedSetTemplate = new HashSet<WorkingSetTemplateIntensityTechnique>();
            WorkingSetTemplateIntensityTechniqueSetTemplate = new HashSet<WorkingSetTemplateIntensityTechnique>();
        }

        public long Id { get; set; }
        public long WorkUnitTemplateId { get; set; }
        public long ProgressiveNumber { get; set; }
        public string TargetRepetitions { get; set; }
        public long? Rest { get; set; }
        public string Cadence { get; set; }
        public long? Effort { get; set; }
        public long? EffortTypeId { get; set; }

        public virtual EffortType EffortType { get; set; }
        public virtual WorkUnitTemplate WorkUnitTemplate { get; set; }
        public virtual ICollection<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechniqueLinkedSetTemplate { get; set; }
        public virtual ICollection<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechniqueSetTemplate { get; set; }
    }
}
