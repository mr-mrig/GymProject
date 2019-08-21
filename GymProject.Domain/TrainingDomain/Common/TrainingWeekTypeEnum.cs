using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingWeekTypeEnum : Enumeration
    {



        #region Consts
        #endregion



        public static TrainingWeekTypeEnum NotSet = new TrainingWeekTypeEnum(0, "Not Set", "Not specified");
        public static TrainingWeekTypeEnum Generic = new TrainingWeekTypeEnum(1, "Generic", "Generic week with no specific target");
        public static TrainingWeekTypeEnum ActiveDeload = new TrainingWeekTypeEnum(2, "Deload", "Active recovery week");
        public static TrainingWeekTypeEnum FullRest = new TrainingWeekTypeEnum(3, "Full Rest", "No training week");
        public static TrainingWeekTypeEnum Tapering = new TrainingWeekTypeEnum(4, "Tapering", "Relief phase before a test");
        public static TrainingWeekTypeEnum Overreach = new TrainingWeekTypeEnum(5, "Overreach", "High stress week");
        public static TrainingWeekTypeEnum Peak = new TrainingWeekTypeEnum(6, "Peak", "Peak performance oriented week");



        #region Properties

        /// <summary>
        /// The extensive description of the effort type
        /// </summary>
        public string Description { get; private set; }
        #endregion





        #region Ctors

        public TrainingWeekTypeEnum(int id, string name, string description) : base(id, name)
        {
            Description = description;
        }
        #endregion


        #region Enum Common Methods

        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<TrainingWeekTypeEnum> List() =>
            new[] { NotSet, Generic, ActiveDeload, FullRest, Tapering, Overreach, Peak };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static TrainingWeekTypeEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static TrainingWeekTypeEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }
        #endregion



    }
}