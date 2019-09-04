using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.Test.Util
{
    public static class RandomFieldGenerator
    {

        #region consts
        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public const string AlphabetWithSpaces = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        #endregion


        public static Random Rand = new Random();





        /// <summary>
        /// Generate a pseudo-random integer in the range specified.
        /// Boundary values are included.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <returns>A pseudo-generated random int</returns>
        public static int RandomInt(int from, int to)
        {
            try
            {
                return Rand.Next(from, to + 1);
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debugger.Break();
                throw exc;
            }
        }


        /// <summary>
        /// Generate a pseudo-random integer in the range specified that can be NULL.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="prob">Probability of the number to be NULL</param>
        /// <returns>A pseudo-generated random integer, or NULL </returns>
        public static int? RandomIntNullable(int from, int to, float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return null;
            else
                return Rand.Next(from, to);
        }


        /// <summary>
        /// Generate a pseudo-random integer selected from values in the range but the ones in the list.
        /// Boundary values are included.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="valuesExcluded"></param>
        /// <returns>A pseudo-generated random int</returns>
        public static int RandomIntValueExcluded(int from, int to, IEnumerable<int> valuesExcluded)
        {
            int val = -1;

            if (Enumerable.Range(from, to - from + 1).SequenceEqual(valuesExcluded))
                throw new ArgumentException($"No possible integers to choose from when trying to generate the random number");

            while (valuesExcluded.Contains(val = RandomInt(from, to)))
                ;

            return val;
        }


        /// <summary>
        /// Generate a pseudo-random integer selected from values in the range but the one specified.
        /// Boundary values are included.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="valueExcluded">The value to be excluded</param>
        /// <returns>A pseudo-generated random int</returns>
        public static int RandomIntValueExcluded(int from, int to, int valueExcluded)
        {
            List<int> excludeList = new List<int>() { valueExcluded };

            return RandomIntValueExcluded(from, to, excludeList);
        }


        /// <summary>
        /// Generates a psuedo-random sequence of chars as a string.
        /// </summary>
        /// <param name="length">The legth of the string to be generated</param>
        /// <param name="prob">Probability of null being returned</param>
        /// <returns>A pseudo-random string.</returns>
        public static string RandomTextValue(int length, bool areSpacesAllowed = false, float prob = 0)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            IEnumerable<char> chars;

            if (areSpacesAllowed)
            {
                chars = Enumerable.Range(0, length)
                    .Select(x => AlphabetWithSpaces[Rand.Next(0, Alphabet.Length)]);
            }
            else
            {
                chars = Enumerable.Range(0, length)
                    .Select(x => Alphabet[Rand.Next(0, Alphabet.Length)]);
            }


            return new string(chars.ToArray());
        }



        /// <summary>
        /// Generates a psuedo-random sequence of chars as a string.
        /// </summary>
        /// <param name="lengthMin">The minimum legth of the string to be generated</param>
        /// <param name="lengthMax">The maximum legth of the string to be generated</param>
        /// <param name="prob">Probability of null being returned</param>
        /// <returns>A pseudo-random string.</returns>
        public static string RandomTextValue(int lengthMin, int lengthMax, bool areSpacesAllowed = false, float prob = 0)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            IEnumerable<char> chars;

            if (areSpacesAllowed)
            {
                chars = Enumerable.Range(0, RandomInt(lengthMin, lengthMax))
                    .Select(x => AlphabetWithSpaces[Rand.Next(0, Alphabet.Length)]);
            }
            else
            {
                chars = Enumerable.Range(0, RandomInt(lengthMin, lengthMax))
                    .Select(x => Alphabet[Rand.Next(0, Alphabet.Length)]);
            }

            return new string(chars.ToArray());
        }




        /// <summary>
        /// Randomly choose an element among the choices provided.
        /// </summary>
        /// <param name="possibleChoices">The items list which to choose from</param>
        /// <returns>One of the possible items.</returns>
        public static T? ChooseAmongNullable<T>(IList<T?> possibleChoices, float prob = 0) where T : struct
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            return possibleChoices[RandomInt(0, possibleChoices.Count() - 1)];
        }


        /// <summary>
        /// Randomly choose an element among the choices provided.
        /// </summary>
        /// <param name="possibleChoices">The items list which to choose from</param>
        /// <returns>One of the possible items.</returns>
        public static T ChooseAmong<T>(IEnumerable<T> possibleChoices)
        {
            if (possibleChoices == null || possibleChoices.Count() == 0)
                return default;

            return possibleChoices.ElementAt(RandomInt(0, possibleChoices.Count() - 1));
        }


        /// <summary>
        /// Randomly choose a string among the choices provided.
        /// </summary>
        /// <param name="possibleChoices">The list of string which to choose from</param>
        /// <param name="prob">Probability of null being returned</param>
        /// <returns>One of the specified strings.</returns>
        public static string ChooseText(IList<string> possibleChoices, float prob = 0)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            return possibleChoices[RandomInt(0, possibleChoices.Count - 1)];
        }


        /// <summary>
        /// Generate a pseudo-random double (incapsuled in a string) in the range specifed that can be NULL.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="prob">Probability of the number to be NULL</param>
        /// <returns>A string storing a pseudo-generated random double, or NULL </returns>
        public static string RandomDoubleNullAllowed(double from, double to, int decimalPlaces = 2, float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return "NULL";

            return RandomDouble(from, to, decimalPlaces).ToString();
        }


        /// <summary>
        /// Generate a pseudo-random double  in the range specifed - lower boundary included, higher excluded.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <returns>A string storing a pseudo-generated random double </returns>
        public static double RandomDouble(double from, double to, int decimalPlaces = 2)
        {
            return Math.Round(Rand.NextDouble() * (to - from) + from, decimalPlaces);
        }


        /// <summary>
        /// Generate a pseudo-random float in the range specifed - lower boundary included, higher excluded.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <returns>A string storing a pseudo-generated random double </returns>
        public static float RandomFloat(float from, float to, int decimalPlaces = 2)
        {
            return (float)RandomDouble(from, to, decimalPlaces);

        }

        /// <summary>
        /// Returns true with the provided probability
        /// </summary>
        /// <param name="prob">Probability of True</param>
        /// <returns>Returns a boolean</returns>
        public static bool RandomBoolWithProbability(float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Simulate the chance of an event to occur
        /// </summary>
        /// <param name="probabilityToOccur">The probability for the event to happen</param>
        /// <returns>True if the event occurs, false otherwise</returns>
        public static bool RollEventWithProbability(double probabilityToOccur)

            => RandomDouble(0, 1) < probabilityToOccur;


        /// <summary>
        /// Generate a random DateTime in the interval specified
        /// </summary>
        /// <param name="fromDate">The lower bound</param>
        /// <param name="toDate">The upper bound</param>
        /// <returns>The randomic DateTime</returns>
        public static DateTime RandomDate(DateTime fromDate, DateTime toDate)
        {
            int range = (toDate - fromDate).Days;
            return fromDate.AddDays(RandomInt(0, range));
        }


        /// <summary>
        /// Generate a random DateTime in the interval between [centralDate - range/2, centralDate + range/2]
        /// </summary>
        /// <param name="daysRange">The timespan where the date will be picked from [days]</param>
        /// <param name="centralDate">The central date</param>
        /// <returns>The randomic DateTime</returns>
        public static DateTime RandomDate(DateTime centralDate, int daysRange)
        {
            bool inTheFuture = RollEventWithProbability(0.5f);

            if(inTheFuture)
                return centralDate.AddDays(RandomInt(0, daysRange / 2));
            else
                return centralDate.AddDays(-RandomInt(0, daysRange / 2));
        }


        /// <summary>
        /// Generate a random DateTime in the interval between [Today - range/2, Today + range/2]
        /// </summary>
        /// <param name="daysRange">The timespan where the date will be picked from [days]</param>
        /// <returns>The randomic DateTime</returns>
        public static DateTime RandomDate(int daysRange)

            => RandomDate(DateTime.Today, daysRange);


        /// <summary>
        /// Randomly choose the number of reps according to the selected intensity [%].
        /// </summary>
        /// <param name="intensityPercentage">The intensity in terms of RM percentage</param>
        /// <returns>A random number which respects the intensity boundaries.</returns>
        public static int? ValidRepsFromIntensity(float intensityPercentage, float prob = 0)
        {
            float step = 5f;

            if (intensityPercentage > 94)
                return 1;
            else if (intensityPercentage >= 83 && intensityPercentage <= 94)
                step = 3;
            else if (intensityPercentage >= 68)
                step = 2.5f;

            else if (intensityPercentage >= 63)
                step = 2.2f;

            else
                step = 2f;

            // Maximum valid reps to choose from
            int maxReps = (int)Math.Round((100f - intensityPercentage) / step);

            return RandomIntNullable((int)(maxReps * 0.5f), maxReps + 1, prob);
        }


        /// <summary>
        /// Randomly choose the number of reps according to the selected intensity [RM].
        /// </summary>
        /// <param name="intensityPercentage">The intensity in terms of RM weight</param>
        /// <returns>A random number which respects the intensity boundaries.</returns>
        public static int? ValidRepsFromRm(int intensityRM, float prob = 0)
        {
            return RandomIntNullable((int)(intensityRM * 0.7f), intensityRM + 1, prob);
        }
    }
}
