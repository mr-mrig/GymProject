using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class EntryStatusTypeEnum : Enumeration
    {


        #region Class Enums
        //public static EntryStatusTypeEnum NotSet = new EntryStatusTypeEnum(0, "NotSet");
        public static EntryStatusTypeEnum Private = new EntryStatusTypeEnum(1, "Private", "The entry is visible only to the Owner");
        public static EntryStatusTypeEnum Pending = new EntryStatusTypeEnum(2, "Pending", "Public entry waiting for approval");
        public static EntryStatusTypeEnum Approved = new EntryStatusTypeEnum(3, "Approved", "Public entry visible to everyone");
        public static EntryStatusTypeEnum Banned = new EntryStatusTypeEnum(4, "Banned", "Banned entry, visible to nobody");
        public static EntryStatusTypeEnum Native = new EntryStatusTypeEnum(5, "Native", "Entry belonging to the DB release");
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


        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<EntryStatusTypeEnum> List() =>
            new[] { /*NotSet, */Private, Pending, Approved, Banned, Native,};


        /// <summary>
        /// Creates a EntryStatusType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The EntryStatusType object instance</returns>
        public static EntryStatusTypeEnum FromName(string name)
        {
            EntryStatusTypeEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a EntryStatusType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The EntryStatusType object instance</returns>
        public static EntryStatusTypeEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }


    }
}
