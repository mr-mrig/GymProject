using GymProject.Domain.BodyDomain.MuscleGroupAggregate;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    internal class DatabaseSeed : IDatabaseSeed
    {



        public GymContext Context { get; private set; }
        public DatabaseSeedService SeedingService { get; private set; }


        #region Seeding

        public IEnumerable<UserRoot> Users { get; protected set; }
        public ICollection<AthleteRoot> Athletes { get; protected set; }
        public IEnumerable<ExcerciseRoot> Excercises { get; protected set; }
        public IEnumerable<MuscleGroupRoot> Muscles { get; protected set; }
        public IEnumerable<TrainingPlanRoot> TrainingPlans { get; protected set; }
        public ICollection<TrainingScheduleRoot> TrainingSchedules { get; protected set; }
        public ICollection<WorkoutTemplateRoot> WorkoutTemplates { get; protected set; }


        //public IEnumerable<WorkUnitTemplateNoteRoot> WorkUnitTemplateNotes { get; protected set; }
        public IEnumerable<TrainingPlanNoteRoot> TrainingPlanNotes { get; protected set; }
        public IEnumerable<WorkingSetNoteRoot> WorkingSetNotes { get; protected set; }
        public IEnumerable<TrainingPlanMessageRoot> TrainingPlanMessages { get; protected set; }
        public IEnumerable<WorkUnitTemplateNoteRoot> WorkUnitTemplateNotes { get; private set; }


        public IEnumerable<TrainingHashtagRoot> TrainingHashtags { get; protected set; }
        public IEnumerable<TrainingPhaseRoot> TrainingPhases { get; protected set; }
        public IEnumerable<TrainingProficiencyRoot> TrainingProficiencies { get; protected set; }
        public IEnumerable<TrainingPlanPhaseRelation> TrainingPlanPhases { get; protected set; }
        public IEnumerable<TrainingPlanHashtagRelation> TrainingPlanHashtags { get; protected set; }
        public IEnumerable<TrainingPlanProficiencyRelation> TrainingPlanProficiencies { get; protected set; }


        public IEnumerable<IntensityTechniqueRoot> IntensityTechniques { get; protected set; }

        #endregion




        /// <summary>
        /// Database seeding for the physical Application Test DB, which might require more articulated test cases.
        /// </summary>
        /// <param name="context">The DB Context</param>
        public DatabaseSeed(GymContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(GymContext));
            SeedingService = new DatabaseSeedService(context);
        }



        /// <summary>
        /// Check whether the Unit Test Database has the test cases loaded or it must be seeded in order to start the tests.
        /// This method just checks a sample query, hence the developer must ensure that all the queries have been seeded.
        /// </summary>
        /// <returns>True if Db ready, false if seeding is needed</returns>
        public bool IsDbReadyForUnitTesting()

            => Context.TrainingPlans.Count() > 0
                && Context.WorkoutTemplates.Count() > 0;
                // Insert other checks here....
        


        public async Task SeedUser()
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
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public async Task SeedMuscle()
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
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public async Task SeedExcercise()
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
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public async Task SeedIntensityTechnique()
        {
            IntensityTechniques = new List<IntensityTechniqueRoot>()
            {
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 1", "IT1", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 2", "IT2", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 3", "IT3", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 4", "IT4", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 5", "IT5", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 6", "IT6", null, false),
            };

            Context.IntensityTechniques.AddRange(IntensityTechniques);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public async Task SeedHashtags()
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
            await Context.SaveAsync().ConfigureAwait(false);


            TrainingPhases = new List<TrainingPhaseRoot>()
            {
                TrainingPhaseRoot.CreateNativeTrainingPhase("Conditioning"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Recomp"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Cut"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Bulk"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Strength"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Peaking"),
                TrainingPhaseRoot.CreatePrivateTrainingPhase("My Private"),
                TrainingPhaseRoot.CreatePublicTrainingPhase("My public"),
            };

            Context.TrainingPhases.AddRange(TrainingPhases);
            await Context.SaveAsync().ConfigureAwait(false);


            TrainingProficiencies = new List<TrainingProficiencyRoot>()
            {
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Beginner", PersonalNoteValue.Write("A Beginner")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Intermediate", PersonalNoteValue.Write("An Intermediate")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Advanced", PersonalNoteValue.Write("An Advanced")),
                TrainingProficiencyRoot.CreatePublicTrainingProficiency("Pro", PersonalNoteValue.Write("Big Gainz!")),
            };

            Context.TrainingProficiencies.AddRange(TrainingProficiencies);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public async Task SeedNotes()
        {
            WorkUnitTemplateNotes = new List<WorkUnitTemplateNoteRoot>()
            {
                WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note1")),
                WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note2")),
                WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note3")),
                WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note4")),
            };

            Context.WorkUnitTemplateNotes.AddRange(WorkUnitTemplateNotes);
            await Context.SaveAsync().ConfigureAwait(false);

            TrainingPlanNotes = new List<TrainingPlanNoteRoot>()
            {
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note1")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note2")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note3")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note4")),
            };

            Context.TrainingPlanNotes.AddRange(TrainingPlanNotes);
            await Context.SaveAsync().ConfigureAwait(false);

            WorkingSetNotes = new List<WorkingSetNoteRoot>()
            {
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note1")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note2")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note3")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note4")),
            };

            Context.WorkingSetNotes.AddRange(WorkingSetNotes);
            await Context.SaveAsync().ConfigureAwait(false);

            TrainingPlanMessages = new List<TrainingPlanMessageRoot>()
            {
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 2")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 3")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 4")),
            };

            Context.TrainingPlanMessages.AddRange(TrainingPlanMessages);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public async Task SeedAthlete()
        {
            try
            {
                AthleteRoot athlete;

                // Ad-hoc athlete
                uint adHocAthleteId = 1;
                athlete = AthleteRoot.RegisterAthlete(adHocAthleteId,
                    new List<UserTrainingPhaseRelation>
                    {
                    UserTrainingPhaseRelation.PlanPhasePublic(1, new DateTime(2018,1,1), new DateTime(2018,6,1)),
                    UserTrainingPhaseRelation.PlanPhasePrivate(2, new DateTime(2018,6,2), new DateTime(2019,1,1)),
                    },
                    new List<UserTrainingProficiencyRelation>
                    {
                    UserTrainingProficiencyRelation.AssignTrainingProficiency(1, new DateTime(2019,1,1), DateTime.UtcNow)
                    });
                Athletes.Add(athlete);
                Context.Add(athlete);
                await Context.SaveAsync().ConfigureAwait(false);

                // Other Athletes
                foreach (UserRoot user in Users.Where(x => x.Id != adHocAthleteId))
                {
                    athlete = AthleteRoot.RegisterAthlete(user.Id.Value);
                    Athletes.Add(athlete);
                    Context.Add(athlete);
                    await Context.SaveAsync().ConfigureAwait(false);
                }

                List<uint> hashtags1 = new List<uint> { 2, 3, 4 };
                List<uint> phases1 = new List<uint> { 1, 2 };
                List<uint> proficiencies1 = new List<uint> { 1, 2 };
                List<uint> focuses1 = new List<uint> { 1, 3 };

                List<uint> hashtags = new List<uint> { 4 };
                List<uint> proficiencies = new List<uint> { 3 };

                // User Training Plans
                athlete = Athletes.Single(x => x.Id == 1);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 1, "Plan1 User1", true, null, 1, hashtags1, proficiencies1, phases1, focuses1).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 2, "Plan2 User1 Variant of Plan1", false, 1, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 3, "Plan3 User1 Variant of Plan2", false, 1, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 4, "Plan4 User1", true, null, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 5, "Plan5 User1 Variant of Plan1 - never scheduled", true, 1, null, hashtags, proficiencies).ConfigureAwait(false);

                athlete = Athletes.Single(x => x.Id == 2);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 6, "Plan6 User2", false, null, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 1, "Plan7 User2 Inherited from 1", false, null, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 2, "Plan8 User2 Inherited from 2", false, null, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 3, "Plan9 User2 Inherited from 3", false, null, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 7, "Plan10 User2 Variant of Plan9", false, 3, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 8, "Plan11 User2", false, null, null, hashtags, proficiencies).ConfigureAwait(false);

                athlete = Athletes.Single(x => x.Id == 3);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 2, "Plan12 User3 Inherited from 2", false, null, null, hashtags, proficiencies).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 8, "Plan12 User3 Inherited from 8", false, null, null, hashtags, proficiencies).ConfigureAwait(false);

                // User Phases
                uint athleteId = 1;
                uint phaseId = 1;
                athlete = Athletes.Single(x => x.Id == athleteId);
                //athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Private, 
                //    DateRangeValue.RangeStartingFrom(DateTime.Today.AddDays(-100)), 
                //    PersonalNoteValue.Write($"Athlete{athleteId.ToString()} - Phase{phaseId.ToString()}"));
                athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Private);

                Context.Update(athlete);
                await Context.SaveAsync().ConfigureAwait(false);

                athleteId = 2;
                phaseId = 1;
                athlete = Athletes.Single(x => x.Id == athleteId);
                athlete.StartTrainingPhase(athleteId, EntryStatusTypeEnum.Pending);

                Context.Update(athlete);
                await Context.SaveAsync().ConfigureAwait(false);

                // User Proficiencies
                athleteId = 1;
                uint proficiencyId = 3;
                athlete = Athletes.Single(x => x.Id == athleteId);
                athlete.AchieveTrainingProficiency(proficiencyId);

                Context.Update(athlete);
                await Context.SaveAsync().ConfigureAwait(false);

                athleteId = 2;
                proficiencyId = 1;
                athlete = Athletes.Single(x => x.Id == athleteId);
                athlete.AchieveTrainingProficiency(proficiencyId);

                Context.Update(athlete);
                await Context.SaveAsync().ConfigureAwait(false);
            }
            catch(Exception exc)
            {
                System.Diagnostics.Debugger.Break();
            }
        }



        public async Task SeedTrainingPlan()
        {
            // Seed Training Plans
            TrainingPlanRoot plan1 = TrainingPlanRoot.CreateTrainingPlan(1);
            TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan(1);
            TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan(1);
            TrainingPlanRoot plan4 = TrainingPlanRoot.CreateTrainingPlan(1);
            TrainingPlanRoot plan5 = TrainingPlanRoot.CreateTrainingPlan(1);
            TrainingPlanRoot plan6 = TrainingPlanRoot.CreateTrainingPlan(2);
            //TrainingPlanRoot plan7 = TrainingPlanRoot.CreateTrainingPlan("Plan7 User2 Inherited from 1", false, 2);
            //TrainingPlanRoot plan8 = TrainingPlanRoot.CreateTrainingPlan("Plan8 User2 Inherited from 2", false, 2);
            //TrainingPlanRoot plan9 = TrainingPlanRoot.CreateTrainingPlan("Plan9 User2 Inherited from 3", false, 2);
            TrainingPlanRoot plan10 = TrainingPlanRoot.CreateTrainingPlan(2);
            TrainingPlanRoot plan11 = TrainingPlanRoot.CreateTrainingPlan(2);
            //TrainingPlanRoot plan12 = TrainingPlanRoot.CreateTrainingPlan("Plan12 User3 Inherited from 2", false, 3);
            //TrainingPlanRoot plan13 = TrainingPlanRoot.CreateTrainingPlan("Plan12 User3 Inherited from 8", false, 3);



            TrainingPlans = new List<TrainingPlanRoot>()
            {
                plan1, plan2, plan3, plan4, plan5, plan6, plan10, plan11,
            };

            // Ad-hoc plans - training weeks
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);

            //plan2.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);

            // Other training weeks
            foreach(var plan in TrainingPlans.Skip(2))
            {
                plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
                plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            }

            Context.TrainingPlans.AddRange(TrainingPlans);
            await Context.SaveAsync().ConfigureAwait(false);

            // Seed Workouts
            await SeedStandardTestCaseWorkoutTemplate(TrainingPlans.First()).ConfigureAwait(false);   // Make it different from the others
            await SeedDraftPlanWorkoutTemplate(TrainingPlans.Skip(1).First()).ConfigureAwait(false);   // Make it different from the others

            foreach (TrainingPlanRoot plan in TrainingPlans.Skip(2))
                await SeedWorkoutTemplate(plan);

            // Save last changes
            Context.TrainingPlans.UpdateRange(TrainingPlans);
            Context.WorkoutTemplates.UpdateRange(WorkoutTemplates);
            await Context.SaveAsync().ConfigureAwait(false);

            //// Seed Training Plan Relations
            //plan1.AttachChildPlan(plan3.Id, TrainingPlanTypeEnum.Variant);
            //plan1.AttachChildPlan(plan6.Id, TrainingPlanTypeEnum.Inherited);
            //plan3.AttachChildPlan(plan7.Id, TrainingPlanTypeEnum.Inherited);
            //plan3.AttachChildPlan(plan5.Id, TrainingPlanTypeEnum.Variant);
            //plan5.AttachChildPlan(plan8.Id, TrainingPlanTypeEnum.Inherited);
            //plan8.AttachChildPlan(plan9.Id, TrainingPlanTypeEnum.Variant);

            //Context.Update(plan1);
            //await Context.SaveAsync().ConfigureAwait(false);
            //Context.Update(plan3);
            //await Context.SaveAsync().ConfigureAwait(false);
            //Context.Update(plan5);
            //await Context.SaveAsync().ConfigureAwait(false);
            //Context.Update(plan8);
            //await Context.SaveAsync().ConfigureAwait(false);
        }


        private async Task SeedWorkoutTemplate(TrainingPlanRoot plan, 
            float workoutsNumber = 3, float workingSetsNumber = 3, float exercisesNumber = 3, float repetitions = 10)
        {
            WorkoutTemplates = new List<WorkoutTemplateRoot>();

            foreach (TrainingWeekEntity week in plan.TrainingWeeks)
            {
                uint weekId = week.Id ?? 1;

                for (int iwo = 0; iwo < workoutsNumber; iwo++)
                {
                    WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(weekId, (uint)iwo);
                    workout.GiveName(GetWorkoutName(iwo));

                    WorkoutTemplates.Add(workout);
                    Context.WorkoutTemplates.Add(workout);

                    // Save now as we need the ID later in this method
                    await Context.SaveAsync().ConfigureAwait(false);

                    // Add to the plan
                    plan.PlanWorkout(week.ProgressiveNumber, workout.Id.Value);
                    Context.Update(plan);
                    await Context.SaveAsync().ConfigureAwait(false);

                    // Add the same number of WSs and excercises for each workout
                    for (uint iexc = 0; iexc < exercisesNumber; iexc++)
                    {
                        uint excerciseId = iexc >= Context.Excercises.Count()
                            ? 1
                            : iexc + 1;

                        workout.DraftExcercise(excerciseId);

                        for (uint iws = 0; iws < workingSetsNumber; iws++)
                        {
                            workout.AddTransientWorkingSet(iexc, WSRepetitionsValue.TrackRepetitionSerie((uint)repetitions));
                            //workout.AddWorkingSetIntensityTechnique(iexc, iws, 1);
                        }
                    }

                    Context.Update(workout);
                    await Context.SaveAsync().ConfigureAwait(false);
                }
            }
        }


        private async Task SeedStandardTestCaseWorkoutTemplate(TrainingPlanRoot plan, float repetitions = 10)
        {
            WorkoutTemplates = new List<WorkoutTemplateRoot>();

            for(int iweek = 0; iweek < plan.TrainingWeeks.Count; iweek++)
            {
                TrainingWeekEntity week = plan.TrainingWeeks.ElementAt(iweek);

                uint weekId = week.Id ?? 1;
                int workoutsNumber = iweek % 2 == 0 ? 5 : 4;   // Expected avg WO number = 4.5

                for (int iwo = 0; iwo < workoutsNumber; iwo++)
                {
                    int exercisesNumber = 3;    // Expected avg WU number = 3
                    WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(weekId, (uint)iwo);
                    workout.GiveName(GetWorkoutName(iwo));

                    //if (iwo == 0)
                    //    workout.ScheduleToSpecificDay(WeekdayEnum.Monday);

                    WorkoutTemplates.Add(workout);
                    Context.WorkoutTemplates.Add(workout);

                    // Save now as we need the ID later in this method
                    await Context.SaveAsync().ConfigureAwait(false);

                    // Add to the plan
                    plan.PlanWorkout(week.ProgressiveNumber, workout.Id.Value);
                    Context.Update(plan);
                    await Context.SaveAsync().ConfigureAwait(false);


                    // Add the same number of WSs and excercises for each workout
                    for (uint iexc = 0; iexc < exercisesNumber; iexc++)
                    {
                        int workingSetsNumber = iexc % 2 == 0 ? 2 : 4;   // Expected avg WS number = 2.66

                        uint excerciseId = iexc >= Context.Excercises.Count()
                            ? 1
                            : iexc + 1;

                        workout.DraftExcercise(excerciseId);

                        for (uint iws = 0; iws < workingSetsNumber; iws++)
                        {
                            workout.AddTransientWorkingSet(iexc, WSRepetitionsValue.TrackRepetitionSerie((uint)repetitions));

                            switch (iws)
                            {
                                case 0:
                                    workout.AddWorkingSetIntensityTechnique(iexc, iws, 1);
                                    if(iexc == 0)
                                        workout.AddWorkingSetIntensityTechnique(iexc, iws, 4);
                                    break;

                                case 2:
                                    workout.AddWorkingSetIntensityTechnique(iexc, iws, 2);
                                    break;

                                default:
                                    break;
                            }

                            switch (iexc)
                            {
                                case 0:
                                    workout.AttachWorkUnitNote(iexc, 1);
                                    workout.ReviseWorkingSetEffort(iexc, iws, TrainingEffortValue.AsRM(12));
                                    workout.ReviseWorkingSetRestPeriod(iexc, iws, RestPeriodValue.SetRest(120, TimeMeasureUnitEnum.Seconds));
                                    break;

                                case 2:
                                    workout.ReviseWorkingSetEffort(iexc, iws, TrainingEffortValue.AsRM(15));
                                    workout.ReviseWorkingSetRestPeriod(iexc, iws, RestPeriodValue.SetRest(90, TimeMeasureUnitEnum.Seconds));
                                    break;

                                default:
                                    workout.ReviseWorkingSetLiftingTempo(iexc, iws, TUTValue.PlanTUT("3030"));
                                    break;
                            }
                            Context.Update(workout);
                            await Context.SaveAsync().ConfigureAwait(false);
                        }
                    }
                    workout.LinkWorkUnits(0, 5);

                    Context.Update(workout);
                    await Context.SaveAsync().ConfigureAwait(false);
                }
            }
        }
        

        private async Task SeedDraftPlanWorkoutTemplate(TrainingPlanRoot plan)
        {
            uint weekId = plan.CloneTrainingWeek(0).Id.Value;
            uint weekProgressiveNumber = 0;

            // Workout1 -> It's a draft!
            WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(weekId, 0);
            workout.GiveName(GetWorkoutName(0));
            WorkoutTemplates.Add(workout);
            Context.WorkoutTemplates.Add(workout);

            await Context.SaveAsync().ConfigureAwait(false);

            plan.PlanWorkout(weekProgressiveNumber, workout.Id.Value);
            Context.Update(plan);
            await Context.SaveAsync().ConfigureAwait(false);

            // Workout2 -> One WU which is a draft
            workout = WorkoutTemplateRoot.PlannedDraft(weekId, 1);
            workout.GiveName(GetWorkoutName(1));
            WorkoutTemplates.Add(workout);
            Context.WorkoutTemplates.Add(workout);

            await Context.SaveAsync().ConfigureAwait(false);

            plan.PlanWorkout(weekProgressiveNumber, workout.Id.Value);
            Context.Update(plan);
            await Context.SaveAsync().ConfigureAwait(false);

            workout.DraftExcercise(4);
            Context.Update(workout);
            await Context.SaveAsync().ConfigureAwait(false);
        }

        private async Task SeedTrainingSchedule()
        {
            TrainingSchedules = new List<TrainingScheduleRoot>();

            //uint userId = 1;
            //uint planId = 1;
            //athlete = Athletes.Single(a => a.Id == userId);

            //schedulePeriod = DateRangeValue.RangeBetween(new DateTime(2018, 1, 1), new DateTime(2018, 2, 15));
            //schedule = TrainingScheduleRoot.ScheduleTrainingPlan(null, TrainingPlans.First().Id, schedulePeriod);
            //feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(4), null);
            //schedule.ProvideFeedback(feedback);
            //feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(5), PersonalNoteValue.Write("Perfect!"));
            //schedule.ProvideFeedback(feedback);

            //TrainingSchedules.Add(schedule);
            //Context.Add(schedule);
            //await Context.SaveAsync().ConfigureAwait(false);

            //athlete.ScheduleTraining(planId, schedule.Id.Value);


            // User1 - Plan1 - Schedule1 -> 2 feedbacks
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(1, 1, new DateTime(2018, 1, 1), new DateTime(2018, 2, 15),
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(4), null),
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(5),  PersonalNoteValue.Write("Perfect!")),
                    })
                .ConfigureAwait(false));

            // User1 - Plan1 - Schedule2 -> 1 feedback -> ongoing, no end date
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(1, 1, new DateTime(2019, 1, 1), null,
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(1), null),
                    })
                .ConfigureAwait(false));

            // User2 - Plan1 - Schedule1 -> 2 feedbacks
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(2, 7, new DateTime(2018, 1, 1), new DateTime(2018, 2, 15),
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(1), PersonalNoteValue.Write("Comment User1")),
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(2), PersonalNoteValue.Write("Comment User2")),
                    })
                .ConfigureAwait(false));


            // User3 - Plan 2 -> ongoing
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(3, 2, new DateTime(2020, 1, 1), DateTime.UtcNow.AddDays(30).Date,
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(1), PersonalNoteValue.Write("Comment User2")),
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 3, RatingValue.Rate(2), PersonalNoteValue.Write("Comment User3")),
                    })
                .ConfigureAwait(false));

            // User3 - Plan 8
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(3, 8, new DateTime(2019, 1, 1), new DateTime(2019, 6, 6), null)
                .ConfigureAwait(false));

            // Other ones not scheduled yet
            // ...

            // Other ones: 1 schedule, 1 feedback -> ongoing plans
            //foreach (UserTrainingPlanEntity plan in Athletes.SelectMany(x => x.TrainingPlans).Where(x => x.Id != 1 && x.Id != 7))
            //{
            //    TrainingSchedules.Add(
            //        await SeedingService.ScheduleTrainingPlan(Athletes.Single(a => a.Id == 2), plan.Id.Value, new DateTime(2020, 1, 1), DateTime.UtcNow.AddDays(30).Date,
            //            new List<TrainingScheduleFeedbackEntity>
            //            {
            //                TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(1), PersonalNoteValue.Write("Comment User1")),
            //                TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(2), PersonalNoteValue.Write("Comment User2")),
            //            })
            //        .ConfigureAwait(false));
            //}
        }



        public async Task SeedTrainingDomain()
        {
            await SeedUser().ConfigureAwait(false);
            await SeedAthlete().ConfigureAwait(false);
            // await SeedMuscle();
            await SeedExcercise().ConfigureAwait(false);
            await SeedNotes().ConfigureAwait(false);
            await SeedIntensityTechnique().ConfigureAwait(false);
            await SeedHashtags().ConfigureAwait(false);
            await SeedTrainingPlan().ConfigureAwait(false);
            await SeedTrainingSchedule().ConfigureAwait(false);
        }



        private string GetWorkoutName(int workoutProgressiveNumber)

            => ("Day " + UtilityLib.Alphabet[workoutProgressiveNumber]).ToUpper();

    }
}
