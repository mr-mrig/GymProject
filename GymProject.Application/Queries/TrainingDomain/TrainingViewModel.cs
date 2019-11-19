using GymProject.Application.Queries.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Application.Queries.TrainingDomain
{




    public class WorkoutFullPlanDto
    {
        public uint TrainingWeekId { get; set; }
        public uint WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public int? SpecificWeekdayId { get; set; }

        public ICollection<WorkUnitDto> WorkUnits { get; set; }
    }

    public class FullFeedbackDetailDto
    {
        public uint TrainingPlanId { get; set; }
        public string TrainingPlanName { get; set; }
        public uint UserId { get; set; }
        public string UserName { get; set; }


        public ICollection<TrainingScheduleDto> TrainingSchedules { get; set; }

    }


    public class TrainingPlanSummaryDto
    {

        public uint TrainingPlanId { get; set; }
        public string TrainingPlanName { get; set; }
        public bool IsBookmarked { get; set; }
        //public bool IsTemplate { get; set; }
        public float? AvgWorkoutDays { get; set; }
        public float? AvgWorkingSets { get; set; }
        public float? AvgIntensityPercentage { get; set; }
        public string LastWorkoutTimestamp { get; set; }
        public int? TrainingWeeksCounter { get; set; }
        public ICollection<HashtagDto> Hashtags { get; set; }
        public ICollection<TrainingProficiencyDto> TargetProficiencies { get; set; }
        public ICollection<TrainingPhaseDto> TargetPhases { get; set; }
    }


    public class TrainingPlanDetailDto
    {
        public uint TrainingPlanId { get; set; }
        public uint? TrainingPlanNoteId { get; set; }
        public string TrainingPlanNote { get; set; }

        public uint? ParentPlanId { get; set; }
        public uint? ParentPlanOwnerId { get; set; }
        public uint? RelationTypeId { get; set; }
        public uint? TrainingWeeksCounter  { get; set; }

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






    #region Child DTOs
    public class WorkUnitDto
    {
        public uint WorkUnitId { get; set; }
        public uint WorkUnitProgressiveNumber { get; set; }
        public uint? WuIntensityTechniqueId { get; set; }
        public string WuIntensityTechniqueAbbreviation { get; set; }
        public uint? NoteId { get; set; }
        public string NoteBody { get; set; }
        public uint? ExcerciseId { get; set; }
        public string ExcerciseName { get; set; }

        public ICollection<WorkingSetDto> WorkingSets { get; set; }
    }



    public class WorkingSetDto
    {
        public uint WorkingSetId { get; set; }
        public uint WsProgressiveNumber { get; set; }
        public int? TargetRepetitions { get; set; }
        public int? Rest { get; set; }
        public string LiftingTempo { get; set; }

        public uint? EffortTypeId { get; set; }
        public int? Effort { get; set; }
        public string EffortName { get; set; }

        public ICollection<IntensityTechniqueDto> IntensityTechniques { get; set; }
    }


    public class IntensityTechniqueDto
    {
        public uint? IntensityTechniqueId { get; set; }
        public string IntensityTechniqueAbbreviation { get; set; }
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

        public uint? TrainingPhaseId { get; set; }
        public string TrainingPhase { get; set; }
        public uint? TrainingProficiencyId { get; set; }
        public string TrainingProficiency { get; set; }

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
        public uint? TrainingPhaseId { get; set; }
        public string TrainingPhase { get; set; }
    }

    public class TrainingProficiencyDto
    {
        public uint? TrainingProficiencyId { get; set; }
        public string TrainingProficiency { get; set; }
    }

    public class HashtagDto
    {
        public uint? HashtagId { get; set; }
        public string Hashtag { get; set; }
    }

    public class MuscleFocusDto
    {
        public uint? FocusedMuscleId { get; set; }
        public string FocusedMuscle { get; set; }
        public string FocusedMuscleAbbreviation { get; set; }
    }

    #endregion









}
