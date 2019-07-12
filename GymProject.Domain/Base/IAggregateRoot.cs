using System;
using System.Collections.Generic;
using System.Text;


namespace GymProject.Domain.Base
{


    /// <summary>
    ///     Marker interface for aggregate roots.
    ///     It is useful to enforce DDD rules, for example preventing non-aggregates to have repositories.
    /// </summary>
    public interface IAggregateRoot
    {

    }
}
