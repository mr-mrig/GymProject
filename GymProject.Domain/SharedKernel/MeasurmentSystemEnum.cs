using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.SharedKernel
{
    public class MeasurmentSystemEnum : Enumeration
    {


        #region Consts

        #endregion



        public static MeasurmentSystemEnum NotSet = new MeasurmentSystemEnum(1, "NotSet");
        public static MeasurmentSystemEnum Metric = new MeasurmentSystemEnum(2, "Metric");
        public static MeasurmentSystemEnum Imperial = new MeasurmentSystemEnum(3, "Imperial");





        #region Ctors

        public MeasurmentSystemEnum(int id, string name) : base(id, name)
        {

        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<MeasurmentSystemEnum> List() =>
            new[] { NotSet, Metric, Imperial, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static MeasurmentSystemEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static MeasurmentSystemEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }


        /// <summary>
        /// Checks wether the meas unit is metric
        /// </summary>
        /// <returns>True if metric</returns>
        public bool IsMetric() => Equals(Metric);


        /// <summary>
        /// Checks wether the meas unit is imperial
        /// </summary>
        /// <returns>True if imperial</returns>
        public bool IsImperial() => Equals(Imperial);


    }
}