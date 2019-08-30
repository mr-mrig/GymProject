using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class DietHasHashtag
    {
        public long DietPlanId { get; set; }
        public long DietHashtagId { get; set; }

        public virtual DietHashtag DietHashtag { get; set; }
        public virtual DietPlan DietPlan { get; set; }
    }
}
