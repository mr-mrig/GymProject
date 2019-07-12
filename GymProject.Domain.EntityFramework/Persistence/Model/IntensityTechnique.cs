using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class IntensityTechnique
    {
        public IntensityTechnique()
        {
            LinkedWorkUnitTemplate = new HashSet<LinkedWorkUnitTemplate>();
            WorkingSetTemplateIntensityTechnique = new HashSet<WorkingSetTemplateIntensityTechnique>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public long IsLinkingTechnique { get; set; }
        public long? Rpe { get; set; }
        public long EntryStatusTypeId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate { get; set; }
        public virtual ICollection<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechnique { get; set; }
    }
}
