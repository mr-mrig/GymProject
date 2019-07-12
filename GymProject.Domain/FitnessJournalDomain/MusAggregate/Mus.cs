using GymProject.Domain.Base;
using GymProject.Domain.FitnessJournalDomain.ModerationAggregate;
using GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate;
using System.Collections.Generic;


namespace GymProject.Domain.FitnessJournalDomain.MusAggregate
{
    public class Mus : Entity, IAggregateRoot
    {


        private string _name = "";
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string _description;
        public string Description
        {
            get => _description;
            set => _description = value;
        }


        public EntryStatusType EntryStatusType { get; set; }


        public IReadOnlyCollection<FitnessDay> FitnessDayList { get; private set; }
    }
}