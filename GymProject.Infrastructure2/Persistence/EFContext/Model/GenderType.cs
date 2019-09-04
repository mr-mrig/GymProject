using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class GenderType
    {
        public GenderType()
        {
            UserDetail = new HashSet<UserDetail>();
        }

        public long Id { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public virtual ICollection<UserDetail> UserDetail { get; set; }
    }
}
