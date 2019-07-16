using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.SharedKernel
{
    public class DietDayTypeEnum : Enumeration
    {


        #region Class Enums
        public static DietDayTypeEnum NotSet = new DietDayTypeEnum(0, "NotSet");
        public static DietDayTypeEnum On = new DietDayTypeEnum(1, "On");
        public static DietDayTypeEnum Off = new DietDayTypeEnum(2, "Off");
        public static DietDayTypeEnum Refeed = new DietDayTypeEnum(3, "Refeed");
        public static DietDayTypeEnum Fast = new DietDayTypeEnum(4, "Fast");
        #endregion


        #region Additional Properties

        public string Description { get; private set; } = "";
        #endregion


        public DietDayTypeEnum(int id, string name) : base(id, name)
        {
        }

        public DietDayTypeEnum(int id, string name, string description) : this(id, name)
        {
            Description = description;
        }




        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<DietDayTypeEnum> List() =>
            new[] { NotSet, On, Off, Refeed, Fast, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static DietDayTypeEnum FromName(string name)
        {
            DietDayTypeEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }

    }
}