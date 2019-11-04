using GymProject.Domain.BodyDomain.MuscleGroupAggregate;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Application.Test.Utils
{
    internal class DatabaseSeed
    {



        public GymContext Context;


        #region Seeding

        public IEnumerable<UserRoot> Users { get; protected set; }
        public IEnumerable<ExcerciseRoot> Excercises { get; protected set; }
        public IEnumerable<MuscleGroupRoot> Muscles { get; protected set; }
        public IEnumerable<TrainingPlanRoot> TrainingPlans { get; protected set; }
        public ICollection<WorkoutTemplateRoot> WorkoutTemplates { get; protected set; }


        //public IEnumerable<WorkUnitTemplateNoteRoot> WorkUnitTemplateNotes { get; protected set; }
        public IEnumerable<TrainingPlanNoteRoot> TrainingPlanNotes { get; protected set; }
        public IEnumerable<WorkingSetNoteRoot> WorkingSetNotes { get; protected set; }
        public IEnumerable<TrainingPlanMessageRoot> TrainingPlanMessages { get; protected set; }


        public IEnumerable<TrainingHashtagRoot> TrainingHashtags { get; protected set; }
        public IEnumerable<TrainingPhaseRoot> TrainingPhases { get; protected set; }
        public IEnumerable<TrainingProficiencyRoot> TrainingProficiencies { get; protected set; }
        public IEnumerable<TrainingPlanPhaseRelation> TrainingPlanPhases { get; protected set; }
        public IEnumerable<TrainingPlanHashtagRelation> TrainingPlanHashtags { get; protected set; }
        public IEnumerable<TrainingPlanProficiencyRelation> TrainingPlanProficiencies { get; protected set; }


        public IEnumerable<IntensityTechniqueRoot> IntensityTechniques { get; protected set; }

        #endregion





        public DatabaseSeed(GymContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(GymContext));
        }





        internal void SeedUser()
        {
            Users = new List<UserRoot>()
            {
                UserRoot.RegisterUser("admin@gymapp.com", "admin0", "pwd", "salt", DateTime.UtcNow, AccountStatusTypeEnum.Super),
                UserRoot.RegisterUser("user1@email.com", "user1", "pwd1", "salt1", DateTime.UtcNow, AccountStatusTypeEnum.Active),
                UserRoot.RegisterUser("user2@email.com", "user2", "pwd2", "salt2", DateTime.UtcNow, AccountStatusTypeEnum.Active),
                UserRoot.RegisterUser("user3@email.com", "user3", "pwd3", "salt3", DateTime.UtcNow, AccountStatusTypeEnum.Active),
                UserRoot.RegisterUser("banned@email.com", "banned_user", "pwdbanned", "saltbanned", DateTime.UtcNow, AccountStatusTypeEnum.Banned),
            };

            Context.Users.AddRange(Users);
            Context.SaveChanges();
        }


        internal void SeedMuscle()
        {
            Muscles = new List<MuscleGroupRoot>()
            {
                MuscleGroupRoot.AddToMusclesLibrary("Chest", "Chest"),
                MuscleGroupRoot.AddToMusclesLibrary("Shoulders", "Delts"),
                MuscleGroupRoot.AddToMusclesLibrary("Legs", "Leg"),
                MuscleGroupRoot.AddToMusclesLibrary("Back", "Back"),
                MuscleGroupRoot.AddToMusclesLibrary("Harmstrings", "Hams"),
            };

            Context.MuscleGroups.AddRange(Muscles);
            Context.SaveChanges();
        }


        internal void SeedExcercise()
        {
            Excercises = new List<ExcerciseRoot>()
            {
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise1", null, EntryStatusTypeEnum.Approved),
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise2", null, EntryStatusTypeEnum.Approved),
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise3", null, EntryStatusTypeEnum.Approved),
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise4", null, EntryStatusTypeEnum.Approved),
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise5", null, EntryStatusTypeEnum.Banned),
            };

            Context.Excercises.AddRange(Excercises);
            Context.SaveChanges();
        }


        internal void SeedIntensityTechnique()
        {
            IntensityTechniques = new List<IntensityTechniqueRoot>()
            {
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 1", "IT1", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 2", "IT2", null, false),
            };

            Context.IntensityTechniques.AddRange(IntensityTechniques);
            Context.SaveChanges();
        }


        internal void SeedHashtags()
        {
            TrainingHashtags = new List<TrainingHashtagRoot>()
            {
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag1")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag2")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag3")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag4")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag5")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag6")),
            };

            Context.TrainingHashtags.AddRange(TrainingHashtags);
            Context.SaveChanges();


            TrainingPhases = new List<TrainingPhaseRoot>()
            {
                TrainingPhaseRoot.CreateNativeTrainingPhase("Conditioning"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Recomp"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Cut"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Bulk"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Strength"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Peaking"),
                TrainingPhaseRoot.CreatePrivateTrainingPhase("My Private"),
                TrainingPhaseRoot.CreatePublicTrainingPhase("My Public"),
            };

            Context.TrainingPhases.AddRange(TrainingPhases);
            Context.SaveChanges();


            TrainingProficiencies = new List<TrainingProficiencyRoot>()
            {
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Beginner", PersonalNoteValue.Write("A Beginner")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Intermediate", PersonalNoteValue.Write("An Intermediate")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Advanced", PersonalNoteValue.Write("An Advanced")),
                TrainingProficiencyRoot.CreatePublicTrainingProficiency("Pro", PersonalNoteValue.Write("Big Gainz!")),
            };

            Context.TrainingProficiencies.AddRange(TrainingProficiencies);
            Context.SaveChanges();
        }


        internal void SeedNotes()
        {
            //WorkUnitTemplateNotes = new List<WorkUnitTemplateNoteRoot>()
            //{
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note1")),
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note2")),
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note3")),
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note4")),
            //};

            //Context.worknot.AddRange(WorkUnitTemplateNotes);
            //Context.SaveChanges();

            TrainingPlanNotes = new List<TrainingPlanNoteRoot>()
            {
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note1")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note2")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note3")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note4")),
            };

            Context.TrainingPlanNotes.AddRange(TrainingPlanNotes);
            Context.SaveChanges();

            WorkingSetNotes = new List<WorkingSetNoteRoot>()
            {
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note1")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note2")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note3")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note4")),
            };

            Context.WorkingSetNotes.AddRange(WorkingSetNotes);
            Context.SaveChanges();

            TrainingPlanMessages = new List<TrainingPlanMessageRoot>()
            {
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
            };

            Context.TrainingPlanMessages.AddRange(TrainingPlanMessages);
            Context.SaveChanges();
        }


        internal void SeedTrainingPlan()
        {
            // Seed Training Plans
            TrainingPlanRoot plan1 = TrainingPlanRoot.CreateTrainingPlan("Plan1 User1", false, 1);
            TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan("Plan1 User2", false, 2);
            TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan("Plan2 User1", false, 1);
            TrainingPlanRoot plan4 = TrainingPlanRoot.CreateTrainingPlan("Plan3 User1", true, 1);

            TrainingPlans = new List<TrainingPlanRoot>()
            {
                plan1, plan2, plan3, plan4,
            };

            plan1.TagAs(2);
            plan1.TagAs(3);
            plan1.TagAs(4);

            plan1.TagPhase(1);
            plan1.TagPhase(2);

            plan1.LinkTargetProficiency(1);
            plan1.LinkTargetProficiency(2);

            plan1.FocusOnMuscle(1);
            plan1.FocusOnMuscle(3);

            plan2.TagAs(1);
            plan3.TagAs(1);

            // Seed Training Weeks
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            
            plan2.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan2.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            
            plan3.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan3.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            
            plan4.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);

            Context.TrainingPlans.AddRange(TrainingPlans);
            Context.SaveChanges();

            // Seed Workouts
            foreach (TrainingPlanRoot plan in TrainingPlans)
                SeedWorkoutTemplate(plan);

            // Save last changes
            Context.TrainingPlans.UpdateRange(TrainingPlans);
            Context.WorkoutTemplates.UpdateRange(WorkoutTemplates);
            Context.SaveChanges();
        }

        private void SeedWorkoutTemplate(TrainingPlanRoot plan, 
            int workoutsNumber = 3, int workingSetsNumber = 3, int execrcisesNumber = 3)
        {
            WorkoutTemplates = new List<WorkoutTemplateRoot>();

            foreach (TrainingWeekEntity week in plan.TrainingWeeks)
            {
                uint weekId = week.Id ?? 1;

                for (int iwo = 0; iwo < workoutsNumber; iwo++)
                {
                    WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(weekId, (uint)iwo);
                    WorkoutTemplates.Add(workout);
                    Context.WorkoutTemplates.Add(workout);

                    // Save now as we need the ID later in this method
                    Context.SaveChanges();

                    // Add the same number of WSs and excercises for each workout
                    for (uint iexc = 0; iexc < execrcisesNumber; iexc++)
                    {
                        uint excerciseId = iexc == Context.Excercises.Count() - 1
                            ? 1
                            : iexc + 1;

                        workout.DraftExcercise(iexc);

                        for (uint iws = 0; iws < workingSetsNumber; iws++)
                        {
                            workout.AddTransientWorkingSet(iexc, WSRepetitionsValue.TrackRepetitionSerie(10));
                            workout.AddWorkingSetIntensityTechnique(iexc, iws, 1);
                        }
                    }
                    // Add to the plan
                    plan.PlanWorkout(week.ProgressiveNumber, workout.Id.Value);
                }
            }
        }


        internal void SeedTrainingDomain()
        {
            SeedUser();
            SeedMuscle();
            SeedExcercise();
            SeedIntensityTechnique();
            SeedHashtags();
            SeedTrainingPlan();
            SeedNotes();

        }


    }
}
