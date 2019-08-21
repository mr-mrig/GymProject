using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TUTValue : ValueObject
    {


        #region Const

        /// <summary>
        /// Valid non numeric characters
        /// </summary>
        public readonly char[] ValidNonNumericChars = new char[] { 'X', '-' };

        /// <summary>
        /// Default tempo representation
        /// </summary>
        //public const string GenericTempo = "2020";
        public const string DefaultTempo = "----";

        /// <summary>
        /// If tempo not specified (DefaultTempo), then assume this tempo when doing calculations
        /// </summary>
        public const string GenericTempo = "1020";

        // Private Indexes
        private const int ConcentricValueIndex = 0;
        private const int StopValueIndex1 = 1;
        private const int EccentricValueIndex = 2;
        private const int StopValueIndex2 = 3;
        #endregion



        /// <summary>
        /// The TUT as 4 values: concentric-stop-eccentric-stop 
        /// </summary>
        public string TUT { get; private set; } = DefaultTempo;




        #region Ctors

        private TUTValue(string tempo)
        {
            if (string.IsNullOrWhiteSpace(tempo))
                TUT = DefaultTempo;
            else
                TUT = tempo.ToUpper();

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="tempo">The value</param>
        /// <returns>The TUTValue instance</returns>
        public static TUTValue PlanTUT(string tempo)

            => new TUTValue(tempo);

        /// <summary>
        /// Factory method - Generic TUT
        /// </summary>
        /// <param name="tempo">The value</param>
        /// <returns>The TUTValue instance</returns>
        public static TUTValue SetGenericTUT()

            => new TUTValue(GenericTempo);

        #endregion


        #region Business Methods

        /// <summary>
        /// Check whether the TUT has been specified
        /// </summary>
        /// <returns>True if Tempo specified</returns>
        public bool IsTempoSpecified() => TUT != DefaultTempo;


        /// <summary>
        /// Get the concentric value of the TUT
        /// </summary>
        /// <returns>The concentric value [s]</returns>
        public int GetConcentric() => GetTutComponent(ConcentricValueIndex);


        /// <summary>
        /// Get the eccentric value of the TUT
        /// </summary>
        /// <returns>The eccentric value [s]</returns>
        public int GetEccentric() => GetTutComponent(EccentricValueIndex);


        /// <summary>
        /// Get the stop1 value of the TUT
        /// </summary>
        /// <returns>The stop value [s]</returns>
        public int GetStop1() => GetTutComponent(StopValueIndex1);


        /// <summary>
        /// Get the stop2 value of the TUT
        /// </summary>
        /// <returns>The stop2 value [s]</returns>
        public int GetStop2() => GetTutComponent(StopValueIndex2);


        /// <summary>
        /// Computes the total seconds of the repetition according to the TUT
        /// </summary>
        /// <returns>The repetition seconds</returns>
        public int ToSeconds()
        {
            if(IsTempoSpecified())
                return TUT.ToCharArray().Sum(x => char.IsDigit(x) ? int.Parse(x.ToString()) : 0);
            else
                return GenericTempo.ToCharArray().Sum(x => int.Parse(x.ToString()));
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Get the TUT component at the index specified
        /// </summary>
        /// <param name="atIndex">The index of the TUT component to be retrieved</param>
        /// <returns>The TUT component as a number [s]</returns>
        private int GetTutComponent(int atIndex)
        {
            string tempoLocal = IsTempoSpecified() ? TUT : GenericTempo;

            char tutComponent = tempoLocal.ToCharArray()[atIndex];

            if (ValidNonNumericChars.Contains(tutComponent))
                return 0;

            return int.Parse(tutComponent.ToString());
        }
        #endregion


        #region Business Rules Validation


        /// <summary>
        /// TUT must be represented as a 4-chars string.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool TempoAsFourCharString() => TUT.Length == GenericTempo.Length;


        /// <summary>
        /// TUT is made up of digits and allowed chars.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool TempoHasValidChars() => TUT.ToCharArray().All(x => ValidNonNumericChars.Contains(x) || Char.IsDigit(x));


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!TempoAsFourCharString())
                throw new TrainingDomainInvariantViolationException($"TUT must be represented as a 4-chars string.");

            if (!TempoHasValidChars())
                throw new TrainingDomainInvariantViolationException($"TUT is made up of digits and allowed chars: [{string.Join(",", ValidNonNumericChars)}]");
        }
        #endregion



        #region Operators

        //public static TUTValue operator +(TUTValue left, int right)
        //{
        //    char[] newTempo = DefaultTempo.ToCharArray();

        //    if (right > 999)
        //    {
        //        newTempo[ConcentricValueIndex] = (char)(left.GetConcentric() + right / 1000);
        //    }

        //    if (right > 999)
        //        newTempo[ConcentricValueIndex] = (char)(left.GetConcentric() + right / 1000);

        //}
        #endregion




        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TUT;
        }
    }
}