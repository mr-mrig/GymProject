using System;
using System.Data.SQLite;
using System.Linq;

namespace GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions
{
    public static class SQLiteExtension
    {

        /// <summary>
        /// Binds the specific user-defined function to the connection.
        /// </summary>
        /// <param name="connection">The SQLite connection to the database</param>
        /// <param name="function">The user-defined function to be bound</param>
        public static void BindFunction(this SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }


        /// <summary>
        /// Open the SQLite connection to the GymApp local database.
        /// This should be used instead of the standard SQLite function as it Binds all the UDF needed.
        /// The other option is to provide methods that map the UDF calls to the actual SQL code.
        /// </summary>
        /// <param name="connection">THe SQLite connection</param>
        public static void OpenGymAppConnection(this SQLiteConnection connection)
        {
            connection.Open();
            connection.BindFunction(new RmToIntensityPercSQLiteFunction());
            connection.BindFunction(new IntensityPercToRmSQLiteFunction());
            connection.BindFunction(new EffortToRpeSQLiteFunction());
        }

    }
}
