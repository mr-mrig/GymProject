//using GymProject.Application.Queries.Base;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace GymProject.Application.Queries.TrainingDomain
//{



//    public class TrainingPlanSummaryDto
//    {

//        public uint TrainingPlanId { get; set; }
//        public string TrainingPlanName { get; set; }
//        public bool IsBookmarked { get; set; }
//        //public bool IsTemplate { get; set; }
//        public ICollection<HashtagDto> Hashtags { get; set; }
//        public ICollection<ProficiencyDto> TargetProficiencies { get; set; }
//        public ICollection<PhaseDto> TargetPhases { get; set; }
//        public float? AvgWorkoutDays { get; set; }
//        public float? AvgWorkingSets { get; set; }
//        public float? AvgIntensityPercentage { get; set; }
//        public DateTime? LastWorkoutTimestamp { get; set; }
//    }








//    public class TrainingPlanDetailDto
//    {
//        // public uint TrainingPlanId { get; set; }
//        public uint? TrainingPlanNoteId { get; set; }
//        public string TrainingPlanNote { get; set; }

//        public uint? ParentPlanId { get; set; }
//        public uint? ParentPlanOwnerId { get; set; }
//        public uint? RelationTypeId { get; set; }
//        public uint? TrainingWeeksNumber { get; set; }

//        public ICollection<MuscleFocusDto> MusclesFocuses { get; set; }
//    }





//    public class FullFeedbackDetailDto
//    {
//        public uint TrainingPlanId { get; set; }
//        public string TrainingPlanName { get; set; }
//        public uint TraineeId { get; set; }
//        public string TraineeName { get; set; }


//        public ICollection<TrainingScheduleDto> TrainingSchedules { get; set; }

//    }



//    public class WorkoutFullPlanDto
//    {
//        public uint TrainingWeekId { get; set; }
//        public uint WorkoutId { get; set; }
//        public string WorkoutName { get; set; }
//        public int? SpecificWeekdayId { get; set; }

//        public ICollection<WorkUnitDto> WorkUnits { get; set; }
//    }






//    public class WorkUnitDto : BaseIdentifiedDto
//    {
//        public uint WorkUnitProgressiveNumber { get; set; }

//        public IntensityTechniqueDto LinkingIntensityTechnique { get; set; }
//        public WorkUnitNoteDto Note { get; set; }
//        public WorkUnitExcerciseDto Excercise { get; set; }

//        public ICollection<WorkingSetDto> WorkingSets { get; set; }
//    }





//    public class WorkingSetDto : BaseIdentifiedDto
//    {
//        public uint WsProgressiveNumber { get; set; }
//        public int? TargetRepetitions { get; set; }
//        public int? Rest { get; set; }
//        public string LiftingTempo { get; set; }
//        public EffortDto Effort { get; set; }
//        public ICollection<IntensityTechniqueDto> IntensityTechniques { get; set; }
//    }








//    #region Child DTOs

//    public class HashtagDto : BaseIdentifiedDto
//    {
//        public string Body { get; set; }
//    }


//    public class ProficiencyDto : BaseIdentifiedDto
//    {
//        public string Name { get; set; }
//    }


//    public class PhaseDto : BaseIdentifiedDto
//    {
//        public string Name { get; set; }
//    }


//    public class MuscleFocusDto : BaseIdentifiedDto
//    {
//        public string MuscleName { get; set; }
//        public string MuscleAbbreviation { get; set; }
//    }


//    public class WorkUnitNoteDto : BaseIdentifiedDto
//    {
//        public string NoteBody { get; set; }
//    }


//    public class WorkUnitExcerciseDto : BaseIdentifiedDto
//    {
//        public string ExcerciseName { get; set; }
//        //public string ImageUrl { get; set; }
//    }


//    public class IntensityTechniqueDto : BaseIdentifiedDto
//    {
//        public string IntensityTechniqueAbbreviation { get; set; }
//    }


//    public class EffortDto : BaseIdentifiedDto
//    {
//        public int? Value { get; set; }
//        public string Name { get; set; }
//    }


//    public class TrainingScheduleDto : BaseIdentifiedDto
//    {
//        public DateTime StartDate { get; set; }
//        public DateTime? EndDate { get; set; }

//        public PhaseDto TrainingPhase { get; set; }
//        public ProficiencyDto TrainingProficiency { get; set; }

//        public ICollection<FeedbackDto> Feedbacks { get; set; }
//    }



//    public class FeedbackDto : BaseIdentifiedDto
//    {
//        public string Body { get; set; }
//        public float? Rating { get; set; }
//        public uint OwnerId { get; set; }
//    }





//    #endregion




//}