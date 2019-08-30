using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class Excercise
    {
        public Excercise()
        {
            ExcercisePersonalLibrary = new HashSet<ExcercisePersonalLibrary>();
            ExcerciseRelationChildExcercise = new HashSet<ExcerciseRelation>();
            ExcerciseRelationParentExcercise = new HashSet<ExcerciseRelation>();
            ExcerciseSecondaryTarget = new HashSet<ExcerciseSecondaryTarget>();
            ExerciseFocus = new HashSet<ExerciseFocus>();
            PersonalRecord = new HashSet<PersonalRecord>();
            WorkUnit = new HashSet<WorkUnit>();
            WorkUnitTemplate = new HashSet<WorkUnitTemplate>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExecutionGuide { get; set; }
        public string CriticalPointsDescription { get; set; }
        public string ImageUrl { get; set; }
        public long MuscleId { get; set; }
        public long? TrainingEquipmentId { get; set; }
        public long? ExcerciseDifficultyId { get; set; }
        public long PerformanceTypeId { get; set; }
        public long EntryStatusTypeId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual ExerciseDifficulty ExcerciseDifficulty { get; set; }
        public virtual Muscle Muscle { get; set; }
        public virtual PerformanceType PerformanceType { get; set; }
        public virtual TrainingEquipment TrainingEquipment { get; set; }
        public virtual ICollection<ExcercisePersonalLibrary> ExcercisePersonalLibrary { get; set; }
        public virtual ICollection<ExcerciseRelation> ExcerciseRelationChildExcercise { get; set; }
        public virtual ICollection<ExcerciseRelation> ExcerciseRelationParentExcercise { get; set; }
        public virtual ICollection<ExcerciseSecondaryTarget> ExcerciseSecondaryTarget { get; set; }
        public virtual ICollection<ExerciseFocus> ExerciseFocus { get; set; }
        public virtual ICollection<PersonalRecord> PersonalRecord { get; set; }
        public virtual ICollection<WorkUnit> WorkUnit { get; set; }
        public virtual ICollection<WorkUnitTemplate> WorkUnitTemplate { get; set; }
    }
}
