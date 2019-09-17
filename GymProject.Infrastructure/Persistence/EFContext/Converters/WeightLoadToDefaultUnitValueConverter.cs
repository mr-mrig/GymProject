using GymProject.Domain.TrainingDomain.Common;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.Converters
{
    public class WeightLoadToDefaultUnitValueConverter : ValueConverter<WeightPlatesValue, float?>
    {


        public WeightLoadToDefaultUnitValueConverter(ConverterMappingHints mappingHints = null)

            : base
            (
                modelValue => (float?)modelValue.Convert(DatabaseEnvironmentConfiguration.DefaultWeightUnitMeasure).Value,

                databaseValue => databaseValue.HasValue
                    ? WeightPlatesValue.Measure(databaseValue.Value, DatabaseEnvironmentConfiguration.DefaultWeightUnitMeasure)
                    : null
                ,
                mappingHints
            )
        {

        }
    }
}