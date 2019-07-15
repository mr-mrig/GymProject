using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class ActivityDayValue : ValueObject
    {

        public uint? Steps { get; set; }
        public uint? CaloriesOut { get; set; }
        public uint? Stairs { get; set; }
        public ushort? SleepMinutes { get; set; }
        public byte? SleepQuality { get; set; }
        public byte? HeartRateRest { get; set; }
        public byte? HeartRateMax { get; set; }



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Steps;
            yield return CaloriesOut;
            yield return Stairs;
            yield return SleepMinutes;
            yield return SleepQuality;
            yield return HeartRateRest;
            yield return HeartRateMax;
        }
    }
}
