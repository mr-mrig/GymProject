using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class WSWorkTypeEnum : Enumeration
    {





        public static WSWorkTypeEnum NotSet = new WSWorkTypeEnum(1, "NotSet", "");
        public static WSWorkTypeEnum RepetitionBasedSerie = new WSWorkTypeEnum(2, "Repetitions", "reps");
        public static WSWorkTypeEnum TimeBasedSerie = new WSWorkTypeEnum(3, "TimedSerie", "''");



        public string MeasUnit { get; private set; } = string.Empty;



        #region Ctors

        public WSWorkTypeEnum(int id, string name, string measUnit) : base(id, name)
        {
            MeasUnit = measUnit;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<WSWorkTypeEnum> List() =>
            new[] { NotSet, RepetitionBasedSerie, TimeBasedSerie, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static WSWorkTypeEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static WSWorkTypeEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }

    }
}