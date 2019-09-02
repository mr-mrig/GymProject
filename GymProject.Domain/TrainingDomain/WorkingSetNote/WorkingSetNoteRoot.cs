using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;

namespace GymProject.Domain.TrainingDomain.WorkingSetNote
{
    public class WorkingSetNoteRoot : TrainingNoteEntity, IAggregateRoot
    {



        #region Ctors

        private WorkingSetNoteRoot(IdTypeValue id, PersonalNoteValue note) : base(id, note)
        {

        }
        #endregion


        #region Factory

        public static WorkingSetNoteRoot WriteTransient(PersonalNoteValue note)

            => Write(null, note);

        public static WorkingSetNoteRoot Write(IdTypeValue id, PersonalNoteValue note)

            => new WorkingSetNoteRoot(id, note);

        #endregion
    }
}
