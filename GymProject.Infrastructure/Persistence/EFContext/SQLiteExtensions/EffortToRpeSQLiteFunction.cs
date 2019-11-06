using System;
using System.Data.SQLite;

namespace GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions
{

    [SQLiteFunction(Name = "EffortToRpe", Arguments = 3, FuncType = FunctionType.Scalar)]
    public class EffortToRpeSQLiteFunction : SQLiteFunction
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
                        double rm = 0.4167 * effort / 10.0f - 14.2831 * Math.Pow(effort / 10.0f, 0.5) + 115.6122;
                        return Math.Max(Math.Round(10 - (rm - targetReps), 0), 4);

                    case 2:

                        // Rm
                        return Math.Max(10 - (effort - targetReps), 4);

                    case 3:

                        // RPE
                        return effort;

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
