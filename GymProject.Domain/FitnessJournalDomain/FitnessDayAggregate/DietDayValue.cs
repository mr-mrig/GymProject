


using System.Collections.Generic;
using GymProject.Domain.Base;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class DietDayValue : ValueObject
    {


        public float? CarbGrams { get; set; }
        public float? FatGrams { get; set; }
        public float? ProteinGrams { get; set; }
        public float? SaltGrams { get; set; }
        public float? WaterLiters { get; set; }
        public bool? IsFreeMeal { get; set; }

        //public DietDayType DietDayType { get; set; } = null;



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CarbGrams;
            yield return FatGrams;
            yield return ProteinGrams;
            yield return SaltGrams;
            yield return WaterLiters;
            yield return IsFreeMeal;
        }


    }
}
