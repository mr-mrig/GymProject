using System;

namespace GymProject.Application.Queries.Base
{
    public static class Conversions
    {


        /// <summary>
        /// Converts from Unix timestamp - Used by SQLite DBs - to DateTime values
        /// </summary>
        /// <param name="value">The Unix Timestamp</param>
        /// <returns>The Datetime converion</returns>
        public static DateTime? GetDatetimeFromUnixTimestamp(long? value)
        {
            if (value.HasValue)
                return  DateTimeOffset.FromUnixTimeSeconds(value.Value).DateTime;
            else
                return null;
        }
    }

}
