using GymProject.Application.Queries.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Application.Queries.TrainingDomain
{



    public class TrainingPlanSummaryDto
    {

        public uint TrainingPlanId { get; set; }
        public string TrainingPlanName { get; set; }
        public bool IsBookmarked { get; set; }
        //public bool IsTemplate { get; set; }
        public ICollection<HashtagDto> Hashtags { get; set; }
        public ICollection<ProficiencyDto> TargetProficiencies { get; set; }
        public ICollection<PhaseDto> TargetPhases { get; set; }
        public float? AvgWorkoutDays { get; set; }
        public float? AvgWorkingSets { get; set; }
        public float? AvgIntensityPercentage { get; set; }
        public DateTime? LastWorkoutTimestamp { get; set; }
    }






    public class TrainingPlanDetailDto
    {


        //public uint TrainingPlanId { get; set; }
        //public string TrainingPlanName { get; set; }
        //public bool IsBookmarked { get; set; }
        ////public bool IsTemplate { get; set; }
        //public ICollection<HashtagDto> Hashtags { get; set; }
        //public ICollection<ProficiencyDto> TargetProficiencies { get; set; }
        //public ICollection<PhaseDto> TargetPhases { get; set; }
        //public float? AvgWorkoutDays { get; set; }
        //public float? AvgWorkingSets { get; set; }
        //public float? AvgIntensityPercentage { get; set; }
        //public DateTime? LastWorkoutTimestamp { get; set; }
    }





    public class WorkoutFullPlanDto : BaseIdentifiedDto
    {
        public uint TrainingWeekId { get; set; }
        public uint ProgressiveNumber { get; set; }
        //public string Name { get; set; }
        public int? SpecificWeekdayId { get; set; }

        public ICollection<WorkUnitDto> WorkUnits;
    }






    public class WorkUnitDto : BaseIdentifiedDto
    {
        public uint ProgressiveNumber { get; set; }

        public IntensityTechniqueDto LinkingIntensityTechnique { get; set; }
        public WorkUnitNoteDto Note { get; set; }
        public WorkUnitExcerciseDto Excercise { get; set; }

        public ICollection<WorkingSetDto> WorkingSets { get; set; }
    }





    public class WorkingSetDto : BaseIdentifiedDto
    {
        public uint ProgressiveNumber { get; set; }
        public int? Repetitions { get; set; }
        public int? Rest { get; set; }
        public string LiftingTempo { get; set; }
        public EffortDto Effort { get; set; }
        public ICollection<IntensityTechniqueDto> IntensityTechniques { get; set; }
    }








    #region Child DTOs

    public class HashtagDto : BaseIdentifiedDto
    {
        public string Body { get; set; }
    }


    public class ProficiencyDto : BaseIdentifiedDto
    {
        public string Body { get; set; }
    }


    public class PhaseDto : BaseIdentifiedDto
    {
        public string Body { get; set; }
    }


    public class MuscleFocusDto : BaseIdentifiedDto
    {
        public string Body { get; set; }
    }


    public class WorkUnitNoteDto : BaseIdentifiedDto
    {
        public string Body { get; set; }
    }


    public class WorkUnitExcerciseDto : BaseIdentifiedDto
    {
        public string Name { get; set; }
        //public string ImageUrl { get; set; }
    }


    public class IntensityTechniqueDto : BaseIdentifiedDto
    {
        public string Name { get; set; }
    }


    public class EffortDto : BaseIdentifiedDto
    {
        public int? Value { get; set; }
        public string Name { get; set; }
    }

    #endregion




}