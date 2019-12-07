using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using System;
using System.Linq;

namespace GymProject.Application.Validator.StaticRule
{
    internal static class TrainingDomainBasicRules
    {

        internal static bool IsValidWorkType(int? workTypeId)

            => workTypeId == null
                || WSWorkTypeEnum.List().Select(e => e.Id).Contains((int)workTypeId.Value);

        internal static bool IsValidEffortType(int? effortTypeId)

            => effortTypeId == null
                || TrainingEffortTypeEnum.List().Select(e => e.Id).Contains((int)effortTypeId.Value);


        internal static bool IsValidRestMeasUnit(int? effortTypeId)

            => effortTypeId == null
                || TimeMeasureUnitEnum.List().Select(e => e.Id).Contains((int)effortTypeId.Value);
        

        internal static bool IsValidWeekType(uint? weekTypeEnumId)

            => weekTypeEnumId == null
                || TrainingWeekTypeEnum.List().Select(e => e.Id).Contains((int)weekTypeEnumId.Value);        

        internal static bool IsValidEntryStatusType(uint? entryStatusId)

            => entryStatusId == null
                || EntryStatusTypeEnum.List().Select(e => e.Id).Contains((int)entryStatusId.Value);
    }
}
