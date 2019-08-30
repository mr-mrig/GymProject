using GymProject.Domain.SharedKernel;
using System;

namespace GymProject.Domain.Base
{
    public abstract class StatusTrackingEntity<IdType> : Entity<IdType>
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
            EntryStatusType = newStatus ?? EntryStatusTypeEnum.NotSet;
            TestEntryStatusBusinessRules();
        }


        /// <summary>
        /// Changes the status of the entry - Author
        /// </summary>
        /// <param name="newStatus">The new status</param>
        public virtual void ChangeEntryStatus(EntryStatusTypeEnum newStatus)
        {
            EntryStatusType = newStatus ?? EntryStatusTypeEnum.NotSet;
            TestEntryStatusBusinessRules();
        }

        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="Exception">Thrown if business rules violation</exception>
        protected virtual void TestEntryStatusBusinessRules()
        {
            if (EntryStatusType == null || EntryStatusType == EntryStatusTypeEnum.NotSet)
                throw new InvalidOperationException($"The Entry Status must be valid.");
        }
    }
}
