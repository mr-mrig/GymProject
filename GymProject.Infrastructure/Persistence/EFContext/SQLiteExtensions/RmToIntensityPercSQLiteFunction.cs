using System;
using System.Data.SQLite;


namespace GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions
{

    [SQLiteFunction(Name = "RmToIntensityPerc", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class RmToIntensityPercSQLiteFunction : SQLiteFunction
    {

        public override object Invoke(object[] args)
        {
            return 0.4167 * Convert.ToInt32(args[0]) - 14.2831 * Math.Pow(Convert.ToInt32(args[0]), 0.5) + 115.6122;
        }

    }
}
