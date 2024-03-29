﻿using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class WorkUnitTemplateNote
    {
        public WorkUnitTemplateNote()
        {
            WorkUnitTemplate = new HashSet<WorkUnitTemplate>();
        }

        public long Id { get; set; }
        public string Body { get; set; }

        public virtual ICollection<WorkUnitTemplate> WorkUnitTemplate { get; set; }
    }
}
