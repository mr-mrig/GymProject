using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.ModerationAggregate
{
    public class EntryStatusType : Enumeration
    {


        #region Class Enums
        public static EntryStatusType NotSet = new EntryStatusType(0, "NotSet");
        public static EntryStatusType Private = new EntryStatusType(1, "Private");
        public static EntryStatusType Pending = new EntryStatusType(2, "Pending");
        public static EntryStatusType Approved = new EntryStatusType(3, "Approved");
        public static EntryStatusType Banned = new EntryStatusType(4, "Banned");
        public static EntryStatusType Native = new EntryStatusType(5, "Native");
        #endregion


        #region Additional Properties

        public string Description { get; private set; } = "";
        #endregion


        public EntryStatusType(int id, string name) : base(id, name)
        {
        }

        public EntryStatusType(int id, string name, string description) : this(id, name)
        {
            Description = description;
        }
    }
}
