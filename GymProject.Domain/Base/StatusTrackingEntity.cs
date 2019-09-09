using GymProject.Domain.SharedKernel;
using System;

namespace GymProject.Domain.Base
{
    public abstract class StatusTrackingEntity<IdType> : Entity<IdType>
    {


        public virtual EntryStatusTypeEnum EntryStatus { get; protected set; }



        #region Ctors

        public StatusTrackingEntity(IdType id, EntryStatusTypeEnum status) : base(id)
        {
            EntryStatus = status;
        }
        #endregion


        /// <summary>
        /// Changes the status of the entry - Moderator
        /// </summary>
        /// <param name="newStatus">The new status</param>
        /// <param name="moderator">The moderator who changed the status</param>
        public virtual void ModerateEntryStatus(EntryStatusTypeEnum newStatus, Moderator moderator = null)
        {
            EntryStatus = newStatus;
            TestEntryStatusBusinessRules();
        }


        /// <summary>
        /// Changes the status of the entry - Author
        /// </summary>
        /// <param name="newStatus">The new status</param>
        public virtual void ChangeEntryStatus(EntryStatusTypeEnum newStatus)
        {
            EntryStatus = newStatus;
            TestEntryStatusBusinessRules();
        }

        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if business rules violation</exception>
        protected virtual void TestEntryStatusBusinessRules()
        {
            if (EntryStatus == null /*|| EntryStatus == EntryStatusTypeEnum.NotSet*/)
                throw new InvalidOperationException($"The Entry Status must be valid.");
        }
    }
}
