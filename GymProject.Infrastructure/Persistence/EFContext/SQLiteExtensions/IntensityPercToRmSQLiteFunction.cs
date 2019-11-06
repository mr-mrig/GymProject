using System;
using System.Data.SQLite;

namespace GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions
{

    [SQLiteFunction(Name = "IntensityPercToRm", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class IntensityPercToRmSQLiteFunction : SQLiteFunction
    {

        public override object Invoke(object[] args)
        {
            return 324.206809067032 - 18.0137586362208 * Convert.ToDouble(args[0]) + 0.722425494099458 * Math.Pow(Convert.ToDouble(args[0]), 2) - 0.018674659779516 * Math.Pow(Convert.ToDouble(args[0]), 3) + 0.00025787003728422 * Math.Pow(Convert.ToDouble(args[0]), 04)
                - 1.65095582844966E-06 * Math.Pow(Convert.ToDouble(args[0]), 5) + 2.75225269851 * Math.Pow(10, -9) * Math.Pow(Convert.ToDouble(args[0]), 6) + 8.99097867 * Math.Pow(10, -12) * Math.Pow(Convert.ToDouble(args[0]), 7);
        }
    }

}
