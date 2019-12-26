using GymProject.Domain.BodyDomain.MuscleGroupAggregate;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    internal static class DataSeeding
    {



        //internal static IEnumerable<TrainingPhaseRoot> GetTrainingPhaseNativeEntries()

        //    => new List<TrainingPhaseRoot>()
        //    {
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Conditioning"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Recomp"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Bulk"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Cut"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Strength"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Contest Preparation"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Peaking"),
        //        TrainingPhaseRoot.CreateNativeTrainingPhase("Tapering"),
        //    };


        //internal static IEnumerable<TrainingProficiencyRoot> GetTrainingProficiencyNativeEntries()

        //    => new List<TrainingProficiencyRoot>()
        //    {
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(1, "Newcomer", PersonalNoteValue.Write("Just stepped into the gym and/or very poor athletic capabilities")),
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(1, "Beginner", PersonalNoteValue.Write("Low training expirience and basic athletic capabilities")),
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(1, "Intermediate", PersonalNoteValue.Write("Intermediate training expirience in terms of time and skills")),
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(1, "Advanced", PersonalNoteValue.Write("High training expirience and solid skills")),
        //    };


        /// <summary>
        /// Muscle Group data seeding
        /// </summary>
        /// <returns>The data seeding to be used with HasData()</returns>
        internal static IEnumerable<MuscleGroupRoot> GetMuscleGroupNativeEntries()

            => new List<MuscleGroupRoot>()
            {
                MuscleGroupRoot.AddToMusclesLibrary(1, "Chest", "Chest"),
                MuscleGroupRoot.AddToMusclesLibrary(2, "Shoulders", "Delt"),
                MuscleGroupRoot.AddToMusclesLibrary(3, "Biceps", "Bis"),
                MuscleGroupRoot.AddToMusclesLibrary(4, "Triceps", "Tris"),
                MuscleGroupRoot.AddToMusclesLibrary(5, "Forearms", "FArm"),
                MuscleGroupRoot.AddToMusclesLibrary(6, "Trapezius", "Trap"),
                MuscleGroupRoot.AddToMusclesLibrary(7, "Back", "Back"),
                MuscleGroupRoot.AddToMusclesLibrary(8, "Abdomen", "Abs"),
                MuscleGroupRoot.AddToMusclesLibrary(9, "Glutes", "Glute"),
                MuscleGroupRoot.AddToMusclesLibrary(10, "Quadriceps", "Quad"),
                MuscleGroupRoot.AddToMusclesLibrary(11, "Hamstrings", "Hams"),
                MuscleGroupRoot.AddToMusclesLibrary(12, "Calves", "Calf"),
            };


        /// <summary>
        /// Muscle Group data seeding
        /// </summary>
        /// <returns>The data seeding to be used with HasData()</returns>
        internal static IEnumerable<UserRoot> GetUserNativeEntries()

            => new List<UserRoot>()
            {
                UserRoot.RegisterUser(1, "root@email.com", "root", "dummypwd", "dummysalt", DateTime.UtcNow, AccountStatusTypeEnum.Super),
            };


        /// <summary>
        /// Excercise data seeding
        /// </summary>
        /// <returns>The data seeding to be used with HasData()</returns>
        internal static IEnumerable<ExcerciseRoot> GetExcerciseNativeEntries()

            => new List<ExcerciseRoot>()
            {
                ExcerciseRoot.AddToExcerciseLibrary(1, "Bench Press", PersonalNoteValue.Write("Dummy"), 1, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(2, "Bench Press - Inclined", PersonalNoteValue.Write("Dummy"), 1, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(3, "Bench Press - Dumbell", PersonalNoteValue.Write("Dummy"), 1, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(4, "Cable Fyes", PersonalNoteValue.Write("Dummy"), 1, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(5, "Pec Dec", PersonalNoteValue.Write("Dummy"), 1, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(6, "Dips", PersonalNoteValue.Write("Dummy"), 1, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(7, "Squat", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(8, "Squat - Smith Machine", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(9, "Front Squat", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(10, "Overhead Squat", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(11, "Leg Extensions", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(12, "Sommersault Squat - Smith Machine", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(13, "Sissy Squat", PersonalNoteValue.Write("Dummy"), 10, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(14, "Military Press", PersonalNoteValue.Write("Dummy"), 2, null, EntryStatusTypeEnum.Native),
                ExcerciseRoot.AddToExcerciseLibrary(15, "Overhead Press - Dumbell", PersonalNoteValue.Write("Dummy"), 2, null, EntryStatusTypeEnum.Native),
                // A lot more
            };


    }
}
