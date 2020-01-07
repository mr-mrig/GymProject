using GymProject.Application.Queries.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Application.Queries.TrainingDomain
{



    public class TrainingPlanSummaryDto
    {

        public uint? PlanId { get; set; }
        public uint? PlanUserLibraryId { get; set; }
        public uint? OwnerId { get; set; }
        public string PlanName { get; set; }
        public bool IsBookmarked { get; set; }
        // public uint? ParentPlanId { get; set; }
        public ICollection<HashtagDto> Hashtags { get; set; }
        public ICollection<TrainingProficiencyDto> TargetProficiencies { get; set; }
        public ICollection<TrainingPhaseDto> TargetPhases { get; set; }
        public int? TrainingWeeksCounter { get; set; }
        public float? AvgWorkoutDays { get; set; }
        public float? AvgWorkingSets { get; set; }
        public float? AvgIntensityPercentage { get; set; }
        public DateTime? LastWorkoutTimestamp { get; set; }
    }


    public class FullFeedbackDetailDto
    {
        public uint? PlanId { get; set; }
        public uint? PlanUserLibraryId { get; set; }
        public string PlanName { get; set; }
        public uint UserId { get; set; }
        public string UserName { get; set; }


        public ICollection<TrainingScheduleDto> TrainingSchedules { get; set; }

    }


    public class TrainingPlanDetailDto
    {
        public uint? PlanUserLibraryId { get; set; }
        public uint? ParentId { get; set; }
        public string ParentName { get; set; }
        public uint? RelationTypeId { get; set; }
        public uint? NoteId { get; set; }
        public string Note { get; set; }
        public uint? PlanOwnerId { get; set; }

        public ICollection<MuscleFocusDto> MusclesFocuses { get; set; }
    }


    public class TrainingPlanFullDetailDto
    {
        public uint TrainingPlanId { get; set; }
        public string TrainingPlanName { get; set; }
        public bool IsBookmarked { get; set; }
        //public bool IsTemplate { get; set; }
        public uint? TrainingPlanNoteId { get; set; }
        public string TrainingPlanNote { get; set; }
        public uint? ParentPlanId { get; set; }
        public uint? ParentPlanOwnerId { get; set; }
        public uint? RelationTypeId { get; set; }
        public int? TrainingWeeksCounter { get; set; }
        public ICollection<HashtagDto> Hashtags { get; set; }
        public ICollection<TrainingProficiencyDto> TargetProficiencies { get; set; }
        public ICollection<TrainingPhaseDto> TargetPhases { get; set; }
        public ICollection<MuscleFocusDto> MusclesFocuses { get; set; }
    }



    public class TrainingPlanWorkoutsScheduleDto
    {
        public uint WeekId { get; set; }
        public uint WeekProgressiveNumber { get; set; }
        public int WeekTypeId { get; set; }

        public ICollection<WorkoutScheduleInfoDto> Workouts { get; set; }
    }



    public class WorkoutFullPlanDto
    {
        public uint WeekId { get; set; }
        public uint WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public int? WeekdayId { get; set; }

        public ICollection<WorkUnitDto> WorkUnits { get; set; }
    }


    public class TrainingWeekWorkoutsDetailsDto
    {
        public uint? WorkoutId { get; set; }
        public uint WorkoutProgressiveNumber { get; set; }
        public string WorkoutName { get; set; }
        public int? WeekdayId { get; set; }
        public int? SessionId { get; set; }
        public string SessionStarted { get; set; }
        public string SessionEnded { get; set; }

        public ICollection<WorkUnitDto> WorkUnits { get; set; }
    }



    public class WorkUnitSessionDetailsDto
    {
        public uint WorkUnitId { get; set; }
        public uint WorkUnitProgressiveNumber { get; set; }
        public float? Rating { get; set; }
        public uint ExcerciseId { get; set; }
        public string Excercise { get; set; }

        public ICollection<WorkingSetDto> WorkingSets { get; set; }
    }











    #region Child DTOs

    public class WorkoutScheduleInfoDto
    {
        public uint? WorkoutId { get; set; }
        public uint? WorkoutProgressiveNumber { get; set; }
        public string WorkoutName { get; set; }
        public int SpecificWeekdayId { get; set; }
    }



    public class WorkUnitDto
    {
        public uint WuId { get; set; }
        public uint WuProgressiveNumber { get; set; }

        public uint? WuIntensityTechniqueId { get; set; }
        public string WuIntensityTechniqueAbbreviation { get; set; }

        public uint? NoteId { get; set; }
        public string Note { get; set; }

        public uint ExcerciseId { get; set; }
        public string ExcerciseName { get; set; }
        public uint PrimaryMuscleId { get; set; }

        public ICollection<WorkingSetTemplateDto> WorkingSets { get; set; }
    }



    public class WorkingSetTemplateDto
    {
        public uint WsId { get; set; }
        public uint WsProgressiveNumber { get; set; }
        public int? TargetRepetitions { get; set; }
        public int? Rest { get; set; }
        public string LiftingTempo { get; set; }

        public uint? EffortTypeId { get; set; }
        public float? Effort { get; set; }
        public string EffortName { get; set; }

        public ICollection<IntensityTechniqueDto> IntensityTechniques { get; set; }
    }


    public class IntensityTechniqueDto
    {
        public uint? TechniqueId { get; set; }
        public string TechniqueAbbreviation { get; set; }
    }


    public class EffortDto
    {
        public uint? EffortId { get; set; }
        public int? Value { get; set; }
        public string Name { get; set; }
    }


    public class WorkUnitNoteDto
    {
        public uint? NoteId { get; set; }
        public string NoteBody { get; set; }
    }


    public class WorkUnitExcerciseDto
    {
        public uint? ExcerciseId { get; set; }
        public string ExcerciseName { get; set; }
        //public string ImageUrl { get; set; }
    }


    public class TrainingScheduleDto
    {
        public uint? ScheduleId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public uint? PhaseId { get; set; }
        public string Phase { get; set; }
        public uint? ProficiencyId { get; set; }
        public string Proficiency { get; set; }

        public ICollection<FeedbackDto> Feedbacks { get; set; }
    }


    public class FeedbackDto
    {
        public uint? FeedbackId { get; set; }
        public string FeedbackNote { get; set; }
        public float? FeedbackRating { get; set; }
        public uint FeedbackOwnerId { get; set; }
    }


    public class TrainingPhaseDto
    {
        public uint? PhaseId { get; set; }
        public string Phase { get; set; }
    }

    public class TrainingProficiencyDto
    {
        public uint? ProficiencyId { get; set; }
        public string Proficiency { get; set; }
    }

    public class HashtagDto
    {
        public uint? HashtagId { get; set; }
        public string Hashtag { get; set; }
    }



    public class WorkingSetDto
    {
        public uint WsId { get; set; }
        public uint WsProgressiveNumber { get; set; }
        public int? Repetitions { get; set; }
        public float? Weight { get; set; }
    }

    public class MuscleFocusDto
    {
        public uint? MuscleId { get; set; }
        public string MuscleAbbreviation { get; set; }

    }

    #endregion

}
