using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class EntryStatusTypeEnum : Enumeration
    {


        #region Class Enums
        public static EntryStatusTypeEnum NotSet = new EntryStatusTypeEnum(0, "NotSet");
        public static EntryStatusTypeEnum Private = new EntryStatusTypeEnum(1, "Private");
        public static EntryStatusTypeEnum Pending = new EntryStatusTypeEnum(2, "Pending");
        public static EntryStatusTypeEnum Approved = new EntryStatusTypeEnum(3, "Approved");
        public static EntryStatusTypeEnum Banned = new EntryStatusTypeEnum(4, "Banned");
        public static EntryStatusTypeEnum Native = new EntryStatusTypeEnum(5, "Native");
        #endregion


        #region Additional Properties

        public string Description { get; private set; } = "";
        #endregion


        public EntryStatusTypeEnum(int id, string name) : base(id, name)
        {
        }

        public EntryStatusTypeEnum(int id, string name, string description) : this(id, name)
        {
            Description = description;
        }
    }
}
