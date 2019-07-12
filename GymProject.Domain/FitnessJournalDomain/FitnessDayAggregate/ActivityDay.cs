using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class ActivityDay
    {

        public uint? Steps { get; set; }
        public uint? CaloriesOut { get; set; }
        public uint? Stairs { get; set; }
        public ushort? SleepMinutes { get; set; }
        public byte? SleepQuality { get; set; }
        public byte? HeartRateRest { get; set; }
        public byte? HeartRateMax { get; set; }


    }
}
