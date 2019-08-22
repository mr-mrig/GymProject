using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.Base
{
    public class ChangeTrackingEntity<TId> : Entity<TId>
    {



        public DateTime CreatedOn { get; protected set; }

        public DateTime? LastUpdate { get; protected set; } = null;



        public ChangeTrackingEntity(TId id) : base(id)
        {

        }

    }
}
