using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.SharedKernel
{
    public class VolumeUnitMeasureEnum : Enumeration
    {


        #region Consts

        private const float OuncesToGramsFactor = 28.35f;

        private const float KilogramsToPoundsFactor = 2.20462f;
        #endregion


        public static VolumeUnitMeasureEnum Liters = new VolumeUnitMeasureEnum(1, "Liters", "l", MeasurmentSystemEnum.Metric);



        /// <summary>
        /// Meas unit abbreviation - l
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure system - Metric Vs  Imperial
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; } = null;

        /// <summary>
        /// The measurment system: metric Vs imperial
        /// To be used when deciding how to display the measure
        /// </summary>
        public MeasurmentSystemEnum MeasureSystemType { get; private set; }



        #region Ctors

        public VolumeUnitMeasureEnum(int id, string name, string abbreviation, MeasurmentSystemEnum measureSystemType) : base(id, name)
        {
            Abbreviation = abbreviation;
            MeasureSystemType = measureSystemType;

        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<VolumeUnitMeasureEnum> List() =>
            new[] { Liters,  };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static VolumeUnitMeasureEnum FromName(string name)
        {
            VolumeUnitMeasureEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static VolumeUnitMeasureEnum From(int id)
        {
            VolumeUnitMeasureEnum unit = List().SingleOrDefault(s => s.Id == id);

            return unit;
        }


        #region Converters
        #endregion

    }
}