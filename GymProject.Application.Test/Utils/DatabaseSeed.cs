﻿using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Application.Test.Utils
{
    internal class DatabaseSeed
    {



        public GymContext Context;


        #region Seeding

        public IEnumerable<UserRoot> Users { get; protected set; }
        public IEnumerable<ExcerciseRoot> Excercises { get; protected set; }
        public IEnumerable<TrainingPlanRoot> TrainingPlans { get; protected set; }
        public IEnumerable<WorkoutTemplateRoot> WorkoutTemplates { get; protected set; }

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


        internal void SeedTrainingPlan()
        {
            int execrcisesNumber = 3;
            int workingSetsNumber = 3;

            TrainingPlanRoot plan1 = TrainingPlanRoot.CreateTrainingPlan("Plan1 User1", false, 1);
            TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan("Plan1 User2", false, 2);
            TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan("Plan2 User1", false, 1);

            TrainingPlans = new List<TrainingPlanRoot>()
            {
                plan1, plan2, plan3
            };

            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);
            plan1.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Generic, null);

            WorkoutTemplateRoot workout10 = WorkoutTemplateRoot.PlannedDraft(1, 0);
            WorkoutTemplateRoot workout11 = WorkoutTemplateRoot.PlannedDraft(1, 1);
            WorkoutTemplateRoot workout12 = WorkoutTemplateRoot.PlannedDraft(1, 2);

            WorkoutTemplateRoot workout20 = WorkoutTemplateRoot.PlannedDraft(2, 0);
            WorkoutTemplateRoot workout21 = WorkoutTemplateRoot.PlannedDraft(2, 1);
            WorkoutTemplateRoot workout22 = WorkoutTemplateRoot.PlannedDraft(2, 2);


            WorkoutTemplates = new List<WorkoutTemplateRoot>()
            {
                workout10, workout11, workout12,        // Plan1 Week1
                workout20, workout21, workout22,        // Plan1 Week2
            };

            Context.TrainingPlans.AddRange(TrainingPlans);
            Context.WorkoutTemplates.AddRange(WorkoutTemplates);
            Context.SaveChanges();

            // Add the same number of WSs and excercises for each workout
            foreach (WorkoutTemplateRoot wo in WorkoutTemplates)
            {
                for (uint iexc = 0; iexc < execrcisesNumber; iexc++)
                {
                    uint excerciseId = iexc == Context.Excercises.Count() - 1
                        ? 1
                        : iexc + 1;

                    wo.DraftExcercise(iexc);

                    for (uint iws = 0; iws < workingSetsNumber; iws++)
                    {
                        wo.AddTransientWorkingSet(iexc, WSRepetitionsValue.TrackRepetitionSerie(10));
                    }
                }

                // Add to the plan
                switch (wo.TrainingWeekId)
                {
                    case 1:

                        plan1.PlanWorkout(0, wo.Id.Value);
                        break;

                    case 2:

                        plan1.PlanWorkout(1, wo.Id.Value);
                        break;

                    default:

                        break;
                }

            }

            Context.TrainingPlans.UpdateRange(TrainingPlans);
            Context.WorkoutTemplates.UpdateRange(WorkoutTemplates);
            Context.SaveChanges();
        }


        internal void SeedTrainingDomain()
        {
            SeedUser();
            //SeedMuscle();
            SeedExcercise();
            SeedTrainingPlan();

        }


    }
}