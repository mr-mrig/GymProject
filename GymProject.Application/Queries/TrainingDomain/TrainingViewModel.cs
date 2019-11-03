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
        public float AvgWorkoutDays{ get; set; }
        public float AvgWorkingSets{ get; set; }
        public float AvgIntensityPercentage { get; set; }
        public DateTime LastWorkoutTimestamp { get; set; }
    }



    public class HashtagDto
    {
        public uint Id { get; set; }
        public string Body { get; set; }
    }


    public class ProficiencyDto
    {
        public uint Id { get; set; }
        public string Body { get; set; }
    }


    public class PhaseDto
    {
        public uint Id { get; set; }
        public string Body { get; set; }
    }



}
