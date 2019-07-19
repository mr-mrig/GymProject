using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class LengthMeasureUnitEnum : Enumeration
    {



        #region Consts

        private static readonly float InchesToMillimetersMultplierFactor = 25.4f;
        private static readonly float MetersToYardsMultplierFactor = 1.094f;
        private static readonly float MilesToKilometersMultplierFactor = 1.609f;
        #endregion



        public static LengthMeasureUnitEnum Millimeters = new LengthMeasureUnitEnum(1, "Millimeters", "mm", MeasurmentSystemEnum.Metric, 0);
        public static LengthMeasureUnitEnum Centimeters = new LengthMeasureUnitEnum(2, "Centimeters", "cm", MeasurmentSystemEnum.Metric, 1);
        public static LengthMeasureUnitEnum Meters = new LengthMeasureUnitEnum(3, "Meters", "m", MeasurmentSystemEnum.Metric, 1);
        public static LengthMeasureUnitEnum Kilometers = new LengthMeasureUnitEnum(4, "Kilometers", "Km", MeasurmentSystemEnum.Metric, 1);
        public static LengthMeasureUnitEnum Inches = new LengthMeasureUnitEnum(5, "Inches", "Inch", MeasurmentSystemEnum.Imperial, 2);
        public static LengthMeasureUnitEnum Yards = new LengthMeasureUnitEnum(6, "Yard", "yd", MeasurmentSystemEnum.Imperial, 1);
        public static LengthMeasureUnitEnum Miles = new LengthMeasureUnitEnum(7, "Miles", "m", MeasurmentSystemEnum.Imperial, 1);




        /// <summary>
        /// Meas unit abbreviation - mg/dL Vs mmol/L
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Wether the unit allows decimal places or not.
        /// To be used when deciding how to display the measure
        /// </summary>
        public byte MinimumDecimals { get; private set; }

        /// <summary>
        /// The measurment system: metric Vs imperial
        /// To be used when deciding how to display the measure
        /// </summary>
        public MeasurmentSystemEnum MeasureSystemType { get; private set; }



        #region Ctors

        public LengthMeasureUnitEnum(int id, string name, string abbreviation, MeasurmentSystemEnum measSytem, byte minimumDecimals) : base(id, name)
        {
            Abbreviation = abbreviation;
            MinimumDecimals = minimumDecimals;
            MeasureSystemType = measSytem;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<LengthMeasureUnitEnum> List() =>
            new[] { Millimeters, Centimeters, Meters, Kilometers, Inches, Yards, Miles, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static LengthMeasureUnitEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static LengthMeasureUnitEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }



        #region Converters

        /// <summary>
        /// Metric Vs Imperial conversion.
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="fromUnit">The orgiginal measure unit</param>
        /// <param name="toUnit">The destination measure unit</param>
        /// <returns>The converted value</returns>
        public static float ApplyConversionFormula(float value, LengthMeasureUnitEnum fromUnit, LengthMeasureUnitEnum toUnit)
        {

            // Check for non-meaningful conversion
            if (toUnit.Equals(fromUnit))
                return value;

            switch (fromUnit)
            {

                case var _ when fromUnit.Equals(Millimeters):

                    if (toUnit.Equals(Inches))
                        return value / InchesToMillimetersMultplierFactor;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);


                case var _ when fromUnit.Equals(Centimeters):

                    if (toUnit.Equals(Inches))
                        return value / InchesToMillimetersMultplierFactor * 10;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);


                case var _ when fromUnit.Equals(Meters):
                    
                    if (toUnit.Equals(Yards))
                        return value * MetersToYardsMultplierFactor;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);


                case var _ when fromUnit.Equals(Kilometers):

                    if (toUnit.Equals(Miles))
                        return value / MilesToKilometersMultplierFactor;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);


                case var _ when fromUnit.Equals(Inches):

                    if (toUnit.Equals(Millimeters))
                        return value * InchesToMillimetersMultplierFactor;

                    else if (toUnit.Equals(Centimeters))
                        return value * InchesToMillimetersMultplierFactor / 10;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);


                case var _ when fromUnit.Equals(Meters):

                    if (toUnit.Equals(Yards))
                        return value / MetersToYardsMultplierFactor;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);


                case var _ when fromUnit.Equals(Miles):

                    if (toUnit.Equals(Kilometers))
                        return value * MilesToKilometersMultplierFactor;

                    else
                        throw new UnsupportedMeasureException(fromUnit.Abbreviation, toUnit.Abbreviation);

                default:
                    return value;
            }
        }

        #endregion

    }
}
