﻿using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.SharedKernel
{
    public class WeightUnitMeasureEnum : Enumeration
    {


        #region Consts

        private const float OuncesToGramsFactor = 28.35f;

        private const float KilogramsToPoundsFactor = 2.20462f;
        #endregion



        public static WeightUnitMeasureEnum Kilograms = new WeightUnitMeasureEnum(1, "Kilograms", "Kg", MeasurmentSystemEnum.Metric, PoundsToKilograms);
        public static WeightUnitMeasureEnum Grams = new WeightUnitMeasureEnum(2, "Grams", "g", MeasurmentSystemEnum.Metric, OuncesToGrams);
        public static WeightUnitMeasureEnum Pounds = new WeightUnitMeasureEnum(3, "Pounds", "lbs", MeasurmentSystemEnum.Imperial, KilogramsToPounds);
        public static WeightUnitMeasureEnum Ounces = new WeightUnitMeasureEnum(4, "Ounces", "oz", MeasurmentSystemEnum.Imperial, GramsToOunces);




        /// <summary>
        /// Meas unit abbreviation - Kg / g / lbs / oz
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure system - Metric Vs  Imperial
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }

        /// <summary>
        /// The measurment system: metric Vs imperial
        /// To be used when deciding how to display the measure
        /// </summary>
        public MeasurmentSystemEnum MeasureSystemType { get; private set; }





        #region Ctors


        private WeightUnitMeasureEnum() : base(0, null) { } 


        public WeightUnitMeasureEnum(int id, string name, string abbreviation, MeasurmentSystemEnum measSystem, Func<float, float> conversionFormula) : base(id, name)
        {
            Abbreviation = abbreviation;
            ApplyConversionFormula = conversionFormula;
            MeasureSystemType = measSystem;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<WeightUnitMeasureEnum> List() =>
            new[] { Kilograms, Grams, Pounds, Ounces, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static WeightUnitMeasureEnum FromName(string name)
        {
            WeightUnitMeasureEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static WeightUnitMeasureEnum From(int id)
        {
            WeightUnitMeasureEnum unit = List().SingleOrDefault(s => s.Id == id);

            return unit;
        }


        #region Converters

        private static float PoundsToKilograms(float pounds)
        {
            return pounds / KilogramsToPoundsFactor;
        }

        private static float OuncesToGrams(float ounces)
        {
            return ounces * OuncesToGramsFactor;
        }

        private static float GramsToOunces(float grams)
        {
            return grams / OuncesToGramsFactor;
        }

        private static float KilogramsToPounds(float kilos)
        {
            return kilos * KilogramsToPoundsFactor;
        }
        #endregion

    }
}
