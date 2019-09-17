using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace GymProject.Infrastructure.Persistence.Converters
{
    public class UnixTimestampValueConverter : ValueConverter<DateTime, long>
    {
        public UnixTimestampValueConverter(ConverterMappingHints mappingHints = null)

            : base
            (
                datetime => new DateTimeOffset(datetime).ToUnixTimeSeconds(),
                timestamp => DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime,
                mappingHints
            )
        {

        }
    }
}