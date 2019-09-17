
using GymProject.Domain.SharedKernel;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    public class DatabaseEnvironmentConfiguration
    {


        public static readonly WeightUnitMeasureEnum DefaultWeightUnitMeasure = WeightUnitMeasureEnum.Kilograms;

        internal const string SqlLiteNowTimestamp = @"strftime('%s', 'now')";

        internal const int AbbreviationDefaultMaxLength = 4;
        internal const int MuscleAbbreviationMaxLength = 5;
    }
}
