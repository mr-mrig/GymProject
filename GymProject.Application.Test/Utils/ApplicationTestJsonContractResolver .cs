using GymProject.Application.Queries.TrainingDomain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace GymProject.Application.Test.Utils
{
    internal class ApplicationTestJsonContractResolver : DefaultContractResolver
    {

        public static ApplicationTestJsonContractResolver Instance { get; } = new ApplicationTestJsonContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (IsIgnorableId(member))
                property.Ignored = true;


            return property;
        }


        private bool IsIgnorableId(MemberInfo member)

            => 
            (typeof(TrainingPlanDetailDto).IsAssignableFrom(member.DeclaringType)
              && (member.Name == nameof(TrainingPlanDetailDto.PlanUserLibraryId) || member.Name == nameof(TrainingPlanDetailDto.ParentId))) ||
            
            (typeof(WorkoutFullPlanDto).IsAssignableFrom(member.DeclaringType)
              && (member.Name == nameof(WorkoutFullPlanDto.WeekId) || member.Name == nameof(WorkoutFullPlanDto.WorkoutId))) ||
            
            (typeof(WorkUnitDto).IsAssignableFrom(member.DeclaringType)
              && (member.Name == nameof(WorkUnitDto.WuId) || member.Name == nameof(WorkUnitDto.NoteId))) ||
            
            (typeof(WorkingSetDto).IsAssignableFrom(member.DeclaringType)
              && (member.Name == nameof(WorkingSetDto.WsId)))


            ;


    }

}
