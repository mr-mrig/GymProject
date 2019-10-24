using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;

namespace GymProject.Domain.TrainingDomain.WorkUnitTemplateNote
{
    public class WorkUnitTemplateNoteRoot : TrainingNoteEntity, IAggregateRoot
    {



        #region Ctors

        private WorkUnitTemplateNoteRoot() : base (null, null) { }
        

        private WorkUnitTemplateNoteRoot(uint? id, PersonalNoteValue note) : base(id, note)
        {

        }
        #endregion


        #region Factory

        public static WorkUnitTemplateNoteRoot WriteTransient(PersonalNoteValue note)

            => Write(null, note);

        public static WorkUnitTemplateNoteRoot Write(uint? id, PersonalNoteValue note)

            => new WorkUnitTemplateNoteRoot(id, note);

        public static WorkUnitTemplateNoteRoot Write(PersonalNoteValue note)

            => new WorkUnitTemplateNoteRoot(null, note);

        #endregion
    }
}

