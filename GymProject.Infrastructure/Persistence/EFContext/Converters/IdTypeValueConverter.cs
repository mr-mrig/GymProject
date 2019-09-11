//using GymProject.Domain.Base;
//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
//using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

//namespace GymProject.Infrastructure.Persistence.Converters.EFContext
//{
//    public class IdTypeValueConverter : ValueConverter<IdTypeValue, long>
//    {
//        public IdTypeValueConverter(ConverterMappingHints mappingHints = null)
//            : base
//            (
//                id => id.Id,
//                value => IdTypeValue.Create(value),
//                 mappingHints
//            )
//        {

//        }
//    }
//}