using GymProject.Domain.Base;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    public class IdTypeValueConverter : ValueConverter<IdTypeValue, long>
    {
        public IdTypeValueConverter(ConverterMappingHints mappingHints = null)
            : base
            (
                id => id.Id,
                value => IdTypeValue.Create(value),
                mappingHints
            )
        {

        }
    }
}