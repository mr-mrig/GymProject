using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.Base
{
    public class ChangeTrackingEntity : Entity
    {



        public DateTime CreatedOn { get; protected set; }

        public DateTime? LastUpdate { get; protected set; } = null;



    }
}
