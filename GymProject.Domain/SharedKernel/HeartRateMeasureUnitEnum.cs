using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class HeartRateMeasureUnitEnum : Enumeration
    {


            public static HeartRateMeasureUnitEnum Pulses = new HeartRateMeasureUnitEnum(1, "Pulses", "bpm");


            /// <summary>
            /// Meas unit abbreviation - bpm
            /// </summary>
            public string Abbreviation { get; private set; }



            #region Ctors

            public HeartRateMeasureUnitEnum(int id, string name, string abbreviation) : base(id, name)
            {
                Abbreviation = abbreviation;
            }
            #endregion



            /// <summary>
            /// Get the enumeration list
            /// </summary>
            /// <returns>The list storing the enumeration</returns>
            public static IEnumerable<HeartRateMeasureUnitEnum> List() =>
                new[] { Pulses };


            /// <summary>
            /// Creates a PictureType object with the selected name
            /// </summary>
            /// <param name="name">Enumeration name</param>
            /// <returns>The PictureType object instance</returns>
            public static HeartRateMeasureUnitEnum FromName(string name)
            {
                return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            }


            /// <summary>
            /// Creates a PictureType object with the selected id
            /// </summary>
            /// <param name="name">Enumeration id</param>
            /// <returns>The PictureType object instance</returns>
            public static HeartRateMeasureUnitEnum From(int id)
            {
                return List().SingleOrDefault(s => s.Id == id);
            }


    }
}
