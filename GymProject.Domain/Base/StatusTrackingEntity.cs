using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.Base
{
    public class StatusTrackingEntity<IdType> : Entity<IdType>
    {


        public virtual EntryStatusTypeEnum EntryStatusType { get; protected set; }



        #region Ctors

        public StatusTrackingEntity(IdType id) : base(id)
        {

        }
        #endregion


        /// <summary>
        /// Changes the status of the entry - Moderator
        /// </summary>
        /// <param name="newStatus">The new status</param>
        /// <param name="moderator">The moderator who changed the status</param>
        public virtual void ModerateEntryStatus(EntryStatusTypeEnum newStatus, Moderator moderator = null)
        {
            EntryStatusType = newStatus;
        }


        /// <summary>
        /// Changes the status of the entry - Author
        /// </summary>
        /// <param name="newStatus">The new status</param>
        public virtual void ChangeEntryStatus(EntryStatusTypeEnum newStatus)
        {
            EntryStatusType = newStatus;
        }
    }
}
