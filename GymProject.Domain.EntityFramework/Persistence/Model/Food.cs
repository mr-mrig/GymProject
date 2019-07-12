using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class Food
    {
        public Food()
        {
            MealExampleHasFood = new HashSet<MealExampleHasFood>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long EntryStatusTypeId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual ICollection<MealExampleHasFood> MealExampleHasFood { get; set; }
    }
}
