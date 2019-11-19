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
    internal class InMemoryDatabaseSeed : IDatabaseSeed
    {



        public GymContext Context { get; private set; }


        #region Seeding

        public IEnumerable<UserRoot> Users { get; protected set; }
        public IEnumerable<ExcerciseRoot> Excercises { get; protected set; }
        public IEnumerable<MuscleGroupRoot> Muscles { get; protected set; }
        public IEnumerable<TrainingPlanRoot> TrainingPlans { get; protected set; }
        public ICollection<TrainingScheduleRoot> TrainingSchedules { get; protected set; }
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





        /// <summary>
        /// Database seeding specific for in-memory DBs, which are supposed to be created every time.
        /// The DB is way smaller in order not to delay the tests execution.
        /// </summary>
        /// <param name="context">The In-memory DB Context</param>
        public InMemoryDatabaseSeed(GymContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(GymContext));
        }



        /// <summary>
        /// Check whether the Unit Test Database has the test cases loaded or it must be seeded in order to start the tests.
        /// This method just checks a sample query, hence the developer must ensure that all the queries have been seeded.
        /// </summary>
        /// <returns>Always returns true</returns>
        public bool IsDbReadyForUnitTesting() => true;
        


        public void SeedUser()
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
            Context.SaveAsync();
        }


        public void SeedMuscle()
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
            Context.SaveAsync();
        }


        public void SeedExcercise()
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
            Context.SaveAsync();
        }


        public void SeedIntensityTechnique()
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
            Context.SaveAsync();
        }


        public void SeedHashtags()
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
            Context.SaveAsync();


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
            Context.SaveAsync();


            TrainingProficiencies = new List<TrainingProficiencyRoot>()
            {
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Beginner", PersonalNoteValue.Write("A Beginner")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Intermediate", PersonalNoteValue.Write("An Intermediate")),
                TrainingProficiencyRoot.AddNativeTrainingProficiency(null, "Advanced", PersonalNoteValue.Write("An Advanced")),
                TrainingProficiencyRoot.CreatePublicTrainingProficiency("Pro", PersonalNoteValue.Write("Big Gainz!")),
            };

            Context.TrainingProficiencies.AddRange(TrainingProficiencies);
            Context.SaveAsync();
        }


        public void SeedNotes()
        {
            //WorkUnitTemplateNotes = new List<WorkUnitTemplateNoteRoot>()
            //{
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note1")),
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note2")),
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note3")),
            //    WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write("note4")),
            //};

            //Context.worknot.AddRange(WorkUnitTemplateNotes);
            //Context.SaveAsync();

            TrainingPlanNotes = new List<TrainingPlanNoteRoot>()
            {
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note1")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note2")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note3")),
                TrainingPlanNoteRoot.Write(null, PersonalNoteValue.Write("plan note4")),
            };

            Context.TrainingPlanNotes.AddRange(TrainingPlanNotes);
            Context.SaveAsync();

            WorkingSetNotes = new List<WorkingSetNoteRoot>()
            {
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note1")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note2")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note3")),
                WorkingSetNoteRoot.Write(null, PersonalNoteValue.Write("ws note4")),
            };

            Context.WorkingSetNotes.AddRange(WorkingSetNotes);
            Context.SaveAsync();

            TrainingPlanMessages = new List<TrainingPlanMessageRoot>()
            {
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 1")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 2")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 3")),
                TrainingPlanMessageRoot.Write(null, PersonalNoteValue.Write("Message 4")),
            };

            Context.TrainingPlanMessages.AddRange(TrainingPlanMessages);
            Context.SaveAsync();
        }


        public async Task SeedTrainingPlan()
        {
            // Seed Training Plans
            TrainingPlanRoot plan1 = TrainingPlanRoot.CreateTrainingPlan("Plan1 User1", true, 1);
            TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan("Plan2 User1 Variant of Plan1", false, 1);
            TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan("Plan3 User1 Variant of Plan2", false, 1);


            TrainingPlans = new List<TrainingPlanRoot>()
            {
                plan1, plan2, plan3
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

            foreach (var plan in TrainingPlans.Skip(1))
            {
                plan.TagAs(4);
                plan.LinkTargetProficiency(3);
            }

            // Ad-hoc plan - training weeks
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);

            // Other training weeks
            foreach(var plan in TrainingPlans.Skip(1))
            {
                plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
                plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            }

            Context.TrainingPlans.AddRange(TrainingPlans);
            await Context.SaveAsync();

            // Seed Workouts
            await SeedAdHocWorkoutTemplate(TrainingPlans.First());   // Make it different from the others

            foreach (TrainingPlanRoot plan in TrainingPlans.Skip(1))
                await SeedWorkoutTemplate(plan);

            // Save last changes
            Context.TrainingPlans.UpdateRange(TrainingPlans);
            Context.WorkoutTemplates.UpdateRange(WorkoutTemplates);
            await Context.SaveAsync();

            // Seed Training Plan Relations
            plan1.AttachChildPlan(plan3.Id, TrainingPlanTypeEnum.Variant);

            Context.Update(plan1);
            Context.Update(plan3);
            await Context.SaveAsync();
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
                    await Context.SaveAsync();

                    // Add to the plan
                    plan.PlanWorkout(week.ProgressiveNumber, workout.Id.Value);
                    Context.Update(plan);
                    await Context.SaveAsync();

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
                    await Context.SaveAsync();
                }
            }
        }


        private async Task SeedAdHocWorkoutTemplate(TrainingPlanRoot plan, float repetitions = 10)
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

                    if (iwo == 0)
                        workout.ScheduleToSpecificDay(WeekdayEnum.Monday);

                    WorkoutTemplates.Add(workout);
                    Context.WorkoutTemplates.Add(workout);

                    // Save now as we need the ID later in this method
                    await Context.SaveAsync();

                    // Add to the plan
                    plan.PlanWorkout(week.ProgressiveNumber, workout.Id.Value);
                    Context.Update(plan);
                    await Context.SaveAsync();


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

                            switch(iws)
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

                            switch(iexc)
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
                        }
                    }
                    workout.LinkWorkUnits(0, 5);

                    Context.Update(workout);
                    await Context.SaveAsync();
                }
            }
        }

        private async Task SeedTrainingSchedule()
        {
            DateRangeValue schedulePeriod;
            TrainingScheduleRoot schedule;
            TrainingScheduleFeedbackEntity feedback;
            TrainingPlanRoot adHocPlan;
            TrainingSchedules = new List<TrainingScheduleRoot>();

            // Plan1 schedule 1 -> 2 feedbacks
            adHocPlan = TrainingPlans.Single(x => x.Id == 1);

            schedulePeriod = DateRangeValue.RangeBetween(new DateTime(2018, 1, 1), new DateTime(2018, 2, 15));
            schedule = TrainingScheduleRoot.ScheduleTrainingPlan(null, TrainingPlans.First().Id, schedulePeriod);
            feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(4), null);
            schedule.ProvideFeedback(feedback);
            feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(5), PersonalNoteValue.Write("Perfect!"));
            schedule.ProvideFeedback(feedback);

            TrainingSchedules.Add(schedule);
            Context.Add(schedule);
            await Context.SaveAsync();

            adHocPlan.ScheduleTraining(schedule.Id);

            // Plan1 schedule 2 -> 1 feedback
            schedulePeriod = DateRangeValue.RangeBetween(new DateTime(2019, 1, 1), new DateTime(2019, 2, 15));
            schedule = TrainingScheduleRoot.ScheduleTrainingPlan(null, TrainingPlans.First().Id, schedulePeriod);
            feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 1, RatingValue.Rate(1), null);
            schedule.ProvideFeedback(feedback);

            TrainingSchedules.Add(schedule);
            Context.Update(schedule);
            await Context.SaveAsync();

            adHocPlan.ScheduleTraining(schedule.Id);
            Context.Update(adHocPlan);
            await Context.SaveAsync();

            // Other ones: 1 schedule, 1 feedback
            foreach (TrainingPlanRoot plan in TrainingPlans.Skip(1).Where(x => x.Id != 5 && x.Id != 7))
            {
                schedulePeriod = DateRangeValue.RangeBetween(new DateTime(2019, 1, 1), new DateTime(2019, 2, 15));  // Don't move it outside the loop or EF will go nuts!

                schedule = TrainingScheduleRoot.ScheduleTrainingPlan(null, plan.Id, schedulePeriod);
                feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(null, 2, RatingValue.Rate(3), PersonalNoteValue.Write("Comment"));

                TrainingSchedules.Add(schedule);
                schedule.ProvideFeedback(feedback);

                Context.Add(schedule);
                Context.Update(plan);
                await Context.SaveAsync();
            }
        }



        public async Task SeedTrainingDomain()
        {
            SeedUser();
            //SeedMuscle();
            SeedExcercise();
            SeedNotes();
            SeedIntensityTechnique();
            SeedHashtags();
            await SeedTrainingPlan();
            await SeedTrainingSchedule();
        }



        private string GetWorkoutName(int workoutProgressiveNumber)

            => ("Day " + UtilityLib.Alphabet[workoutProgressiveNumber]).ToUpper();

    }
}
