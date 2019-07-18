using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class PlicometryFormulaEnum : Enumeration
    {


        #region Consts
        #endregion


        public static PlicometryFormulaEnum NotSet = new PlicometryFormulaEnum(0, "NotSet", WrapDummyFormula);
        public static PlicometryFormulaEnum JacksonPollock3 = new PlicometryFormulaEnum(1, "JacksonPollock3", WrapJacksonPollockFormula3);
        public static PlicometryFormulaEnum JacksonPollock4 = new PlicometryFormulaEnum(2, "JacksonPollock4", WrapJacksonPollockFormula4);
        public static PlicometryFormulaEnum JacksonPollock7 = new PlicometryFormulaEnum(3, "JacksonPollock7", WrapJacksonPollockFormula7);


        /// <summary>
        /// Apply the formula 
        /// </summary>
        public Func<GenderTypeEnum, ushort, IEnumerable<CaliperSkinfoldValue>, BodyFatValue> ApplyFormula { get; private set; } = null;





        #region Ctors

        public PlicometryFormulaEnum(int id, string name, Func<GenderTypeEnum, ushort, IEnumerable<CaliperSkinfoldValue>, BodyFatValue> formula) : base(id, name)
        {
            ApplyFormula = formula;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<PlicometryFormulaEnum> List() =>
            new[] { NotSet, JacksonPollock3, JacksonPollock4, JacksonPollock7, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static PlicometryFormulaEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static PlicometryFormulaEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }



        #region Formulas

        private static BodyFatValue WrapDummyFormula(GenderTypeEnum gender, ushort age, IEnumerable<CaliperSkinfoldValue> caliperMeasures)
        {
            return null;
        }


        private static BodyFatValue WrapJacksonPollockFormula3(GenderTypeEnum gender, ushort age, IEnumerable<CaliperSkinfoldValue> caliperMeasures)
        {
            double density;
            float skinfoldsSum = caliperMeasures.Sum(x => x.Value);

            // Check for invalid parameters
            if (!ValidParams(gender, age, caliperMeasures))
                return null;

            // Convert to [cm] if needed
            if (!caliperMeasures.FirstOrDefault().Unit.MeasureSystemType.Equals(LengthMeasureUnitEnum.Centimeters))
                caliperMeasures = caliperMeasures.Select(x => x.Convert(LengthMeasureUnitEnum.Centimeters));


            // Apply [cm] formula
            switch (gender)
            {

                case var _ when gender.IsFemale():

                    density = 1.0994921 - (0.0009929 * skinfoldsSum) + (0.0000023 * Math.Pow(skinfoldsSum, 2)) - (0.0001392 * age);
                    break;


                case var _ when gender.IsMale():

                    density = 1.10938 - (0.0008267 * skinfoldsSum) + (0.0000016 * Math.Pow(skinfoldsSum, 2)) - (0.0002574 * age);
                    break;

                default:
                    return null;
            }

            return BodyFatValue.MeasureBodyFat((495f / (float)density) - 450f);
        }


        private static BodyFatValue WrapJacksonPollockFormula4(GenderTypeEnum gender, ushort age, IEnumerable<CaliperSkinfoldValue> caliperMeasures)
        {
            double density;
            float skinfoldsSum = caliperMeasures.Sum(x => x.Value);

            // Check for invalid parameters
            if (!ValidParams(gender, age, caliperMeasures))
                return null;

            // Convert to [cm] if needed
            if (!caliperMeasures.FirstOrDefault().Unit.MeasureSystemType.Equals(LengthMeasureUnitEnum.Centimeters))
                caliperMeasures = caliperMeasures.Select(x => x.Convert(LengthMeasureUnitEnum.Centimeters));


            // Apply [cm] formula
            switch (gender)
            {

                case var _ when gender.IsFemale():

                    density = (0.29669 * skinfoldsSum) - (0.00043 * Math.Sqrt(skinfoldsSum)) + (0.02963 * age) + 1.4072;
                    break;


                case var _ when gender.IsMale():

                    density = (0.29288 * skinfoldsSum) - (0.0005 * Math.Sqrt(skinfoldsSum)) + (0.15845 * age) - 5.76377;
                    break;

                default:
                    return null;
            }

            return BodyFatValue.MeasureBodyFat((495f / (float)density) - 450f);
        }


        private static BodyFatValue WrapJacksonPollockFormula7(GenderTypeEnum gender, ushort age, IEnumerable<CaliperSkinfoldValue> caliperMeasures)
        {
            double density;
            float skinfoldsSum = caliperMeasures.Sum(x => x.Value);

            // Check for invalid parameters
            if (!ValidParams(gender, age, caliperMeasures))
                return null;

            // Convert to [cm] if needed
            if (!caliperMeasures.FirstOrDefault().Unit.MeasureSystemType.Equals(LengthMeasureUnitEnum.Centimeters))
                caliperMeasures = caliperMeasures.Select(x => x.Convert(LengthMeasureUnitEnum.Centimeters));


            // Apply [cm] formula
            switch (gender)
            {

                case var _ when gender.IsFemale():

                    density = 1.097 - (0.00046971 * skinfoldsSum) + (0.00000056 * Math.Pow(skinfoldsSum, 2)) - (0.00012828 * age);
                    break;


                case var _ when gender.IsMale():

                    density = 1.112 - (0.00043499 * skinfoldsSum) + (0.00000055 * Math.Pow(skinfoldsSum, 2)) - (0.00028826 * age);
                    break;

                default:
                    return null;
            }

            return BodyFatValue.MeasureBodyFat((495f / (float)density) - 450f);
        }



        private static bool ValidParams(GenderTypeEnum gender, ushort age, IEnumerable<CaliperSkinfoldValue> caliperMeasures)
        {
            return age > 0
                && !gender.Equals(GenderTypeEnum.NotSet)
                && caliperMeasures.All(x => x != null && x?.Value > 0)
                && caliperMeasures.Where(x => !x.Unit.Equals(caliperMeasures.FirstOrDefault().Unit)).Count() == 0;      // All meas with the same meas unit
            // No check on params number
        }
        #endregion

    }
}
