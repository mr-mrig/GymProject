using GymProject.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.Converters
{
    public class WeekdayEnumToIntegerValueConverter : ValueConverter<WeekdayEnum, int?>
    {


        public WeekdayEnumToIntegerValueConverter(ConverterMappingHints mappingHints = null)

            : base
            (
                modelValue => modelValue.Id,

                databaseValue => databaseValue.HasValue
                    ? WeekdayEnum.From(databaseValue.Value)
                    : null
                ,
                mappingHints
            )
        {

        }
    }
}