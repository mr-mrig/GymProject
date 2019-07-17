﻿using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.SharedKernel
{
    public class GlycemiaMeasureUnitEnum : Enumeration
    {

        public static GlycemiaMeasureUnitEnum Milligrams = new GlycemiaMeasureUnitEnum(1, "Milligrams", "mg/dL", MillimolesToMilligrams);
        //public static GlycemiaMeasureUnitEnum Millimoles = new GlycemiaMeasureUnitEnum(2, "Millimoles", "mmol/L");

        /// <summary>
        /// Meas unit abbreviation - mg/dL Vs mmol/L
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure unit
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }


        #region Ctors

        public GlycemiaMeasureUnitEnum(int id, string name, string abbreviation, Func<float, float> conversionFormula ) : base(id, name)
        {
            Abbreviation = abbreviation;
            ApplyConversionFormula = conversionFormula;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<GlycemiaMeasureUnitEnum> List() =>
            new[] { Milligrams };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static GlycemiaMeasureUnitEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static GlycemiaMeasureUnitEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }


        private static float MillimolesToMilligrams(float mmols)
        {
            throw new NotImplementedException();
        }


        private static float MilligramsToMillimoles(float milligrams)
        {
            throw new NotImplementedException();
        }
    }
}