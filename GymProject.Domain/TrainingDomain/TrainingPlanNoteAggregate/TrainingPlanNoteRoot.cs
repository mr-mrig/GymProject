using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;

namespace GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate
{
    public class TrainingPlanNoteRoot : TrainingNoteEntity, IAggregateRoot
    {



        #region Ctors

        private TrainingPlanNoteRoot(uint? id, PersonalNoteValue note) : base(id, note)
        {

        }
        #endregion


        #region Factory

        public static TrainingPlanNoteRoot WriteTransient(PersonalNoteValue note)

            => Write(null, note);

        public static TrainingPlanNoteRoot Write(uint? id, PersonalNoteValue note)

            => new TrainingPlanNoteRoot(id, note);

        #endregion
    }
}