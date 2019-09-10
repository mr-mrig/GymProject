using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingEffortTypeEnum : Enumeration
    {



        #region Consts

        /// <summary>
        /// What a AMRAP set means on a RPE scale
        /// </summary>
        public const int AMRAPAsRPE = 10;

        /// <summary>
        /// Minimum RPE used for conversions, however the user can still prompt RPEs lower than this
        /// </summary>
        public const float MinRPE = 1;

        /// <summary>
        /// Maximum Intensity Percentage allowed - The user is forced to enter a value beneath it
        /// </summary>
        public const float MaxIntensityPercentage = 110f;

        /// <summary>
        /// Intensity Percentage of a 1RM attempt
        /// </summary>
        public const float OneRMIntensityPercentage = 100f;
        #endregion



        //private static TrainingEffortTypeEnum NotSet = new TrainingEffortTypeEnum(0, "NotSet", "Generic", "Effort not specified", (x) => true);
        //public static TrainingEffortTypeEnum IntensityPerc = new TrainingEffortTypeEnum(1, "Intensity", "%", "Percentage of 1RM", CheckIntensityPercentageConstraints);
        //public static TrainingEffortTypeEnum RM = new TrainingEffortTypeEnum(2, "RM", "RM", "The most weight you can lift for a defined number of exercise movements", CheckRMConstraints);
        //public static TrainingEffortTypeEnum RPE = new TrainingEffortTypeEnum(3, "RPE", "RPE", "Self-assessed measure of the difficulty of a training set", CheckRPEConstraints);

        //private static TrainingEffortTypeEnum NotSet = new TrainingEffortTypeEnum(0, "NotSet", "Generic", "Effort not specified");
        public static TrainingEffortTypeEnum IntensityPerc = new TrainingEffortTypeEnum(1, "Intensity", "%", "Percentage of 1RM");
        public static TrainingEffortTypeEnum RM = new TrainingEffortTypeEnum(2, "RM", "RM", "The most weight you can lift for a defined number of exercise movements");
        public static TrainingEffortTypeEnum RPE = new TrainingEffortTypeEnum(3, "RPE", "RPE", "Self-assessed measure of the difficulty of a training set");




        #region Properties

        /// <summary>
        /// The abbreviation for the effort type
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// The extensive description of the effort type
        /// </summary>
        public string Description { get; private set; }

        ///// <summary>
        ///// Checks wether an effort value satisfies the constrains imposed by its effort type
        ///// </summary>
        //public Predicate<float> CheckEffortTypeConstraints { get; private set; } = null;      // Not supported by EF3.0
        #endregion





        #region Ctors

        private TrainingEffortTypeEnum() : base(0, null) { }


        private TrainingEffortTypeEnum(int id, string name, string abbreviation, string description) : base(id, name)
        {
            Abbreviation = abbreviation;
            Description = description;
            //CheckEffortTypeConstraints = checkEffortTypeConstraints;
        }
        #endregion


        #region Enum Common Methods

        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<TrainingEffortTypeEnum> List() =>
            new[] { /*NotSet,*/ IntensityPerc, RM, RPE, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static TrainingEffortTypeEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static TrainingEffortTypeEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }

        #endregion



        /// <summary>
        /// Validate the input according to its Effort Type
        /// </summary>
        /// <param name="valueToCheck">The value to be checked</param>
        /// <returns>True if the input is valid</returns>
        public bool CheckEffortTypeConstraints(float valueToCheck)
        {
            if (this == IntensityPerc)
                return CheckIntensityPercentageConstraints(valueToCheck);
            else
            {
                if (this == RM)
                    return CheckRMConstraints(valueToCheck);
                else
                    return CheckRPEConstraints(valueToCheck);
            }
        }



        #region Private Methods

        /// <summary>
        /// Intensity as [0,100] float number
        /// </summary>
        /// <param name="effortValue">The input effort value</param>
        /// <returns>True if constraints are met</returns>
        private bool CheckIntensityPercentageConstraints(float effortValue) => effortValue > 0 && effortValue <= MaxIntensityPercentage;

        ///// <summary>
        ///// RM as positive integer number 
        ///// </summary>
        ///// <param name="effortValue">The input effort value</param>
        ///// <returns>True if constraints are met</returns>
        //private static bool CheckRMConstraints(float effortValue) => effortValue > 0 && unchecked (effortValue == (int)effortValue || effortValue == (float)Math.Ceiling(effortValue));

        /// <summary>
        /// RM as positive number 
        /// </summary>
        /// <param name="effortValue">The input effort value</param>
        /// <returns>True if constraints are met</returns>
        private bool CheckRMConstraints(float effortValue) => effortValue > 0;

        /// <summary>
        /// RPE as positive float number 
        /// </summary>
        /// <param name="effortValue">The input effort value</param>
        /// <returns>True if constraints are met</returns>
        private bool CheckRPEConstraints(float effortValue) => effortValue > 0;
        #endregion

    }
}