using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;

namespace GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate
{
    public class TrainingPlanMessageRoot : TrainingNoteEntity, IAggregateRoot
    {



        #region Ctors

        private TrainingPlanMessageRoot() : base(null, null) { }


        private TrainingPlanMessageRoot(uint? id, PersonalNoteValue note) : base(id, note)
        {

        }
        #endregion


        #region Factory

        public static TrainingPlanMessageRoot WriteTransient(PersonalNoteValue note)

            => Write(null, note);

        public static TrainingPlanMessageRoot Write(uint? id, PersonalNoteValue note)

            => new TrainingPlanMessageRoot(id, note);

        #endregion
    }
}

