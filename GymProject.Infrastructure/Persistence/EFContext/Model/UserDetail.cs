using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class UserDetail
    {
        public long Id { get; set; }
        public long? Birthday { get; set; }
        public long? Height { get; set; }
        public string About { get; set; }
        public long? GenderTypeId { get; set; }

        public virtual GenderType GenderType { get; set; }
        public virtual User IdNavigation { get; set; }
    }
}
