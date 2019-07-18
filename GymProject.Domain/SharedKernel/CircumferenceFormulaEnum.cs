using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.SharedKernel
{
    public class CircumferenceFormulaEnum : Enumeration
    {




        public static CircumferenceFormulaEnum NotSet = new CircumferenceFormulaEnum(0, "NotSet", WrapDummyFormula);
        public static CircumferenceFormulaEnum HodgonAndBeckett = new CircumferenceFormulaEnum(1, "HodgonAndBeckett", WrapHodgonAndBeckettFormula);
        //public static CircumferenceFormulaEnum HodgonAndBeckett = new CircumferenceFormulaEnum(1, "HodgonAndBeckett", WrapHodgonAndBeckettFormula);



        /// <summary>
        /// Apply the formula 
        /// </summary>
        public Func<GenderTypeEnum, IList<BodyCircumferenceValue>, BodyFatValue> ApplyFormula { get; private set; } = null;





        #region Ctors

        public CircumferenceFormulaEnum(int id, string name, Func<GenderTypeEnum, IList<BodyCircumferenceValue>, BodyFatValue> formula) : base(id, name)
        {
            ApplyFormula = formula;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<CircumferenceFormulaEnum> List() =>
            new[] { NotSet, HodgonAndBeckett, };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static CircumferenceFormulaEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static CircumferenceFormulaEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }



        #region Converters

        private static BodyFatValue WrapDummyFormula(GenderTypeEnum gender, IList<BodyCircumferenceValue> measures)
        {
            return null;
        }


        private static BodyFatValue WrapHodgonAndBeckettFormula(GenderTypeEnum gender, IList<BodyCircumferenceValue> measures)
        {
            // Check for valid input
            if (!ValidParams(gender, measures))
                return null;

            // Switch between Metric Vs Imperial version
            if (measures[0].Unit.MeasureSystemType.IsMetric())
            {
                IList<BodyCircumferenceValue> convertedMeasures = measures.Select(x => x.Convert(LengthMeasureUnitEnum.Inches)).ToList();
                return ImperialUnitFormula(gender, convertedMeasures);
            }
            else
                return ImperialUnitFormula(gender, measures);
        }



        private static BodyFatValue ImperialUnitFormula(GenderTypeEnum gender, IList<BodyCircumferenceValue> measures)
        {
            if (gender.IsMale())

                return BodyFatValue.MeasureBodyFat(
                    (float)(86.010 * Math.Log10(measures[0].Value - measures[1].Value) - 70.041 * Math.Log10(measures[2].Value) + 36.76));

            else if (gender.IsFemale())

                return BodyFatValue.MeasureBodyFat(
                    (float)(163.205 * Math.Log10(measures[0].Value + measures[1].Value - measures[2].Value) - 97.684 * Math.Log10(measures[3].Value) - 78.387));

            else
                return null;
        }



        private static bool ValidParams(GenderTypeEnum gender, IList<BodyCircumferenceValue> measures)
        {
            return measures.Where(x => !x.Unit.Equals(measures[0].Unit)).Count() == 0       // All meas with the same meas unit
                && !gender.Equals(GenderTypeEnum.NotSet)                                    // Valid gender
                && measures.All(x => x != null && x?.Value > 0)                             // Valid measures
                && gender.IsMale() ? measures.Count == 3 : measures.Count == 4;             // Correct number of measures
        }
        #endregion

    }
}