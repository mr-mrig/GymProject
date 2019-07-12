using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class WorkUnitTemplate
    {
        public WorkUnitTemplate()
        {
            LinkedWorkUnitTemplateFirstWorkUnit = new HashSet<LinkedWorkUnitTemplate>();
            LinkedWorkUnitTemplateSecondWorkUnit = new HashSet<LinkedWorkUnitTemplate>();
            WorkingSetTemplate = new HashSet<WorkingSetTemplate>();
        }

        public long Id { get; set; }
        public long ProgressiveNumber { get; set; }
        public long WorkoutTemplateId { get; set; }
        public long ExcerciseId { get; set; }
        public long? WorkUnitTemplateNoteId { get; set; }

        public virtual Excercise Excercise { get; set; }
        public virtual WorkUnitTemplateNote WorkUnitTemplateNote { get; set; }
        public virtual WorkoutTemplate WorkoutTemplate { get; set; }
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplateFirstWorkUnit { get; set; }
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplateSecondWorkUnit { get; set; }
        public virtual ICollection<WorkingSetTemplate> WorkingSetTemplate { get; set; }
    }
}
