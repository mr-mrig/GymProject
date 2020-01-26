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


    /// <summary>
    /// Way smaller GymContext so it can be seeded on-the-fly without slowing the Unit Test process
    /// </summary>
    internal class InMemoryDatabaseSeed : DatabaseSeed
    {




        #region Seeding

        public IEnumerable<UserRoot> Users { get; protected set; }
        public ICollection<AthleteRoot> Athletes { get; protected set; }
        public IEnumerable<ExcerciseRoot> Excercises { get; protected set; }
        public IEnumerable<MuscleGroupRoot> Muscles { get; protected set; }
        public IEnumerable<TrainingPlanRoot> TrainingPlans { get; protected set; }
        public ICollection<TrainingScheduleRoot> TrainingSchedules { get; protected set; }
        public ICollection<WorkoutTemplateRoot> WorkoutTemplates { get; protected set; }


        public IEnumerable<WorkUnitTemplateNoteRoot> WorkUnitTemplateNotes { get; protected set; }
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





        /// <summary>
        /// Database seeding specific for in-memory DBs, which are supposed to be created every time.
        /// The DB is way smaller in order not to delay the tests execution.
        /// </summary>
        /// <param name="context">The In-memory DB Context</param>
        public InMemoryDatabaseSeed(GymContext context) : base(context) { }



        /// <summary>
        /// Check whether the Unit Test Database has the test cases loaded or it must be seeded in order to start the tests.
        /// This method just checks a sample query, hence the developer must ensure that all the queries have been seeded.
        /// </summary>
        /// <returns>Always returns true</returns>
        public override bool IsDbReadyForUnitTesting() => false;




        public override async Task SeedMuscle()
        {
            Muscles = new List<MuscleGroupRoot>()
            {
                MuscleGroupRoot.AddToMusclesLibrary("Chest", "Chest"),
                MuscleGroupRoot.AddToMusclesLibrary("Shoulders", "Delts"),
                MuscleGroupRoot.AddToMusclesLibrary("Legs", "Leg"),
            };

            Context.MuscleGroups.AddRange(Muscles);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public override async Task SeedExcercise()
        {
            Excercises = new List<ExcerciseRoot>()
            {
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise1", null, 1, null, EntryStatusTypeEnum.Approved),
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise2", null, 1, null, EntryStatusTypeEnum.Approved),
                ExcerciseRoot.AddToExcerciseLibrary(null, "Excercise3", null, 3, null, EntryStatusTypeEnum.Banned),
            };

            Context.Excercises.AddRange(Excercises);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public override async Task SeedIntensityTechnique()
        {
            IntensityTechniques = new List<IntensityTechniqueRoot>()
            {
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 1", "IT1", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 2", "IT2", null, false),
                IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, 1, "Int Tech 3", "IT3", null, false),
            };

            Context.IntensityTechniques.AddRange(IntensityTechniques);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public override async Task SeedHashtagsPhasesProficiencies()
        {
            TrainingHashtags = new List<TrainingHashtagRoot>()
            {
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag1")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag2")),
                TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith("MyHashtag3")),
            };

            Context.TrainingHashtags.AddRange(TrainingHashtags);


            TrainingPhases = new List<TrainingPhaseRoot>()
            {
                TrainingPhaseRoot.CreateNativeTrainingPhase("Conditioning"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Recomp"),
                TrainingPhaseRoot.CreateNativeTrainingPhase("Cut"),
            };

            Context.TrainingPhases.AddRange(TrainingPhases);


            TrainingProficiencies = new List<TrainingProficiencyRoot>()
            {
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Beginner", PersonalNoteValue.Write("A Beginner")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Intermediate", PersonalNoteValue.Write("An Intermediate")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Advanced", PersonalNoteValue.Write("An Advanced")),
            };

            Context.TrainingProficiencies.AddRange(TrainingProficiencies);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public override async Task SeedNotes()
        {
            TrainingPlanNotes = new List<TrainingPlanNoteRoot>()
            {
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note1")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note2")),
            };

            Context.TrainingPlanNotes.AddRange(TrainingPlanNotes);

            WorkingSetNotes = new List<WorkingSetNoteRoot>()
            {
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note1")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note2")),
            };

            Context.WorkingSetNotes.AddRange(WorkingSetNotes);

            TrainingPlanMessages = new List<TrainingPlanMessageRoot>()
            {
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 2")),
            };

            Context.TrainingPlanMessages.AddRange(TrainingPlanMessages);

            WorkUnitTemplateNotes = new List<WorkUnitTemplateNoteRoot>()
            {
                WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("WU note1")),
                WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("WU note2")),
            };

            Context.WorkUnitTemplateNotes.AddRange(WorkUnitTemplateNotes);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public override async Task SeedUser()
        {
            Users = new List<UserRoot>()
            {
                UserRoot.RegisterUser("admin@gymapp.com", "admin0", "pwd", "salt", DateTime.UtcNow, AccountStatusTypeEnum.Super),
                UserRoot.RegisterUser("user1@email.com", "user1", "pwd1", "salt1", DateTime.UtcNow, AccountStatusTypeEnum.Active),
                UserRoot.RegisterUser("user2@email.com", "user2", "pwd2", "salt2", DateTime.UtcNow, AccountStatusTypeEnum.Active),
            };

            Context.Users.AddRange(Users);
            await Context.SaveAsync().ConfigureAwait(false);
        }


        public override async Task SeedAthlete()
        {
            try
            {
                AthleteRoot athlete;
                Athletes = new List<AthleteRoot>();

                foreach (UserRoot user in Users)
                {
                    uint phaseId = 3;
                    uint proficiencyId = 3;
                    AthleteRoot dummyAthlete = AthleteRoot.RegisterAthlete(user.Id);

                    athlete = AthleteRoot.RegisterAthlete(user.Id.Value);

                    Athletes.Add(athlete);
                    Context.Update(athlete);
                    await Context.SaveAsync().ConfigureAwait(false);

                    // User Phases
                    if(user.Id != 3)
                    {
                        athlete = Athletes.Single(x => x.Id == user.Id);
                        athlete.StartTrainingPhase(phaseId, EntryStatusTypeEnum.Private, DateTime.UtcNow.AddDays(50));

                        if (user.Id == 1)
                            athlete.StartTrainingPhase(phaseId - 2, EntryStatusTypeEnum.Approved, DateTime.UtcNow);

                        Context.Update(athlete);
                    }

                    // User Proficiencies
                    if(user.Id.Value != 2)
                    {
                        athlete = Athletes.Single(x => x.Id == user.Id.Value);
                        athlete.AchieveTrainingProficiency(proficiencyId);
                        Context.Update(athlete);
                    }

                    await Context.SaveAsync().ConfigureAwait(false);
                }

            }
            catch(Exception exc)
            {
                System.Diagnostics.Debugger.Break();
            }
        }


        public override async Task SeedUserTrainingPlanLibrary()
        {
            try
            {
                AthleteRoot athlete;
                List<uint> hashtags1 = new List<uint> { 2, 3 };
                List<uint> phases1 = new List<uint> { 1, 2 };
                List<uint> proficiencies1 = new List<uint> { 1, 2 };
                List<uint> focuses1 = new List<uint> { 1, 3 };

                List<uint> hashtags = new List<uint> { 1 };
                List<uint> proficiencies = new List<uint> { 3 };

                athlete = Athletes.Single(x => x.Id == 1);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 1, "Plan1 User1", true, null, 1, hashtags1, proficiencies1, phases1, focuses1).ConfigureAwait(false);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 2, "Plan2 User1 Variant of Plan1", false, 1, null, hashtags, proficiencies).ConfigureAwait(false);

                athlete = Athletes.Single(x => x.Id == 2);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 6, "Plan6 User2", false, null, null, hashtags, proficiencies).ConfigureAwait(false);

                athlete = Athletes.Single(x => x.Id == 3);
                await SeedingService.AddTrainingPlanToUserLibrary(athlete, 2, "Plan32 User3 Inherited from 2", false, null, null, hashtags, proficiencies).ConfigureAwait(false);

            }
            catch(Exception exc)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public override async Task SeedTrainingPlan()
        {
            // Seed Training Plans
            TrainingPlanRoot plan1 = TrainingPlanRoot.CreateTrainingPlan(1);
            TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan(1);

            TrainingPlans = new List<TrainingPlanRoot>()
            {
                plan1, plan2, 
            };


            // Ad-hoc plan - training weeks
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);

            // Other training weeks
            //foreach (var plan in TrainingPlans.Skip(1))
            //{
            //    plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            //    plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            //}

            Context.TrainingPlans.AddRange(TrainingPlans);
            await Context.SaveAsync().ConfigureAwait(false);

            // Seed Workouts
            await SeedAdHocWorkoutTemplate(TrainingPlans.First());   // Make it different from the others

            foreach (TrainingPlanRoot plan in TrainingPlans.Skip(1))
                await SeedWorkoutTemplate(plan, 2, 2, 2, 100);

            // Save last changes
            Context.TrainingPlans.UpdateRange(TrainingPlans);
            Context.WorkoutTemplates.UpdateRange(WorkoutTemplates);
            await Context.SaveAsync().ConfigureAwait(false);
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
                    //await Context.SaveAsync().ConfigureAwait(false);

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


        private async Task SeedAdHocWorkoutTemplate(TrainingPlanRoot plan, float repetitions = 10)
        {
            WorkoutTemplates = new List<WorkoutTemplateRoot>();

            for (int iweek = 0; iweek < plan.TrainingWeeks.Count; iweek++)
            {
                TrainingWeekEntity week = plan.TrainingWeeks.ElementAt(iweek);

                uint weekId = week.Id ?? 1;
                int workoutsNumber = iweek % 2 == 0 ? 5 : 4;   // Expected avg WO number = 4.5

                for (int iwo = 0; iwo < workoutsNumber; iwo++)
                {
                    int exercisesNumber = 3;    // Expected avg WU number = 3
                    WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(weekId, (uint)iwo);
                    workout.GiveName(GetWorkoutName(iwo));

                    if (iwo == 0)
                        workout.ScheduleToSpecificDay(WeekdayEnum.Monday);

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
                                    if (iexc == 0)
                                        workout.AddWorkingSetIntensityTechnique(iexc, iws, 3);
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
                    workout.LinkWorkUnits(0, 1);

                    Context.Update(workout);
                    await Context.SaveAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task SeedTrainingSchedule()
        {
            TrainingSchedules = new List<TrainingScheduleRoot>();

            // User1 - Plan1 - Schedule1 -> 2 feedbacks
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(1, 1, new DateTime(2018, 1, 1), new DateTime(2018, 2, 15),
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(4), null),
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(5),  PersonalNoteValue.Write("Perfect!")),
                    }));

            // User1 - Plan1 - Schedule2 -> 1 feedback -> currently ongoing
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(1, 1, new DateTime(2019, 1, 1), DateTime.UtcNow.AddDays(1),
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(1), null),
                    }));

            // User2 - Plan1 - Schedule1 -> 2 feedbacks
            TrainingSchedules.Add(
                await SeedingService.ScheduleTrainingPlan(2, 1, new DateTime(2018, 1, 1), new DateTime(2018, 2, 15),
                    new List<TrainingScheduleFeedbackEntity>
                    {
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(1), PersonalNoteValue.Write("Comment User1")),
                        TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(2), PersonalNoteValue.Write("Comment User2")),
                    }));

            // Other ones: no shcedules
        }




        private string GetWorkoutName(int workoutProgressiveNumber)

            => ("Day " + UtilityLib.Alphabet[workoutProgressiveNumber]).ToUpper();

    }
}
