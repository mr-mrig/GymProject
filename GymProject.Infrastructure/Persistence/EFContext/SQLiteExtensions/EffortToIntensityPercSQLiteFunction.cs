using System;
using System.Data.SQLite;

namespace GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions
{


    [SQLiteFunction(Name = "EffortToIntensityPerc", Arguments = 3, FuncType = FunctionType.Scalar)]
    public class EffortToIntensityPercSQLiteFunction : SQLiteFunction
    {


        public override object Invoke(object[] args)
        {
            try
            {
                int effort = Convert.ToInt32(args[0]);
                int effortType = Convert.ToInt32(args[1]);
                int targetReps = Convert.ToInt32(args[2]);

                switch (effortType)
                {

                    case 1:

                        // Intensity
                        return effort / 10.0;

                    case 2:

                        // Rm
                        return Math.Round(0.4167 * effort - 14.2831 * Math.Pow(effort, 0.5) + 115.6122, 1);

                    case 3:

                        // RPE
                        int rm = targetReps + (10 - effort);
                        return Math.Round(0.4167 * rm - 14.2831 * Math.Pow(rm, 0.5) + 115.6122, 1);

                    default:
                        return 0;

                }
            }
            catch
            {
                return null;
            }
        }
    }
}
