using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using ConsoleTest.DataSeed;
using System.Collections.Generic;
using GymProject.Domain.SharedKernel;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //bool seedDataRequired = false;
            
            // Check if seeding is needed
            if(TestDataSeed.IsSeedingRequired())
            {
                SeedData();
            }


            using (GymContext context = new GymContext())
            {
                uint weekProgressiveNumber = 0;
                uint excerciseId;
                uint wunitProgressiveNumber;

                UserRoot owner = context.Users.Single(x => x.Id == 2);

                TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan("Test plan", true, owner.Id);
                plan.LinkTargetProficiency(2);
                plan.TagAs(1);
                plan.TagAs(2);

                context.Update(plan);
                context.SaveChanges();

                ////List<TrainingWeekEntity> weeks = plan.TrainingWeeks as List<TrainingWeekEntity>;
                ////weeks.Add(TrainingWeekEntity.PlanTrainingWeek(null, 0));



                plan.PlanWorkout(weekProgressiveNumber, new List<WorkingSetTemplateEntity>());

                context.Update(plan);
                context.SaveChanges();

                WorkoutTemplateRoot wo = WorkoutTemplateRoot.PlanWorkout(plan.Id, new List<WorkUnitTemplateEntity>(), "WO1", WeekdayEnum.Monday);
                context.Update(wo);
                context.SaveChanges();

                excerciseId = 1;
                wo.PlanTransientExcercise(excerciseId, new List<WorkingSetTemplateEntity>());
                context.Update(wo);
                context.SaveChanges();


                TestServiceLayer service = new TestServiceLayer(context);

                wunitProgressiveNumber = 0;
                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wunitProgressiveNumber = 0;
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10));
                ////context.Entry(wo.CloneLastWorkingSet(wunitProgressiveNumber)).State = EntityState.Unchanged;
                //context.Update(wo);
                //context.SaveChanges();

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10));
                //context.Update(wo);
                //context.SaveChanges();

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10));
                //context.Update(wo);
                //context.SaveChanges();

                excerciseId = 10;
                wo.PlanTransientExcercise(excerciseId, new List<WorkingSetTemplateEntity>());
                context.Update(wo);
                context.SaveChanges();

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wunitProgressiveNumber = 1;
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //context.Update(wo);
                //context.SaveChanges();

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wunitProgressiveNumber = 1;
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //context.Update(wo);
                //context.SaveChanges();

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wunitProgressiveNumber = 1;
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //context.Update(wo);
                //context.SaveChanges();

                service.AddWorkingSet(wo.Id.Value, wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wunitProgressiveNumber = 1;
                //wo.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //context.Update(wo);
                //context.SaveChanges();


                // WO2
                plan.PlanWorkout(weekProgressiveNumber, new List<WorkingSetTemplateEntity>());
                context.Update(plan);
                context.SaveChanges();

                WorkoutTemplateRoot wo1 = WorkoutTemplateRoot.PlanWorkout(plan.Id, new List<WorkUnitTemplateEntity>(), "WO2");
                context.Update(wo1);
                context.SaveChanges();

                excerciseId = 1;
                wo1.PlanTransientExcercise(excerciseId, null);
                context.Update(wo1);
                context.SaveChanges();

                wunitProgressiveNumber = 0;
                wo1.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(20));
                context.Update(wo1);
                context.SaveChanges();

                wo1.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(15));
                context.Update(wo1);
                context.SaveChanges();

                wo1.AddTransientWorkingSet(wunitProgressiveNumber, WSRepetitionsValue.TrackRepetitionSerie(10));
                context.Update(wo1);
                context.SaveChanges();



                //WorkoutTemplateRoot wo = WorkoutTemplateRoot.PlanTransientWorkout(null, "WO1", WeekdayEnum.Monday);

                //wo.PlanTransientExcercise(1, null);
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10));

                //wo.PlanTransientExcercise(10, null);
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));
                //wo.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(8), RestPeriodValue.SetFullRecoveryRest(), TrainingEffortValue.AsRM(10));

                //plan.PlanWorkout(0, wo);
                //context.Update(plan);
                //context.Update(wo);
                //context.SaveChanges();


                //WorkoutTemplateRoot wo1 = WorkoutTemplateRoot.PlanTransientWorkout(null, "WO2");
                //wo1.PlanTransientExcercise(1, null);
                //wo1.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(20));
                //wo1.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(15));
                //wo1.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10));

                //plan.PlanWorkout(0, wo1);

                //context.Update(wo1);
                //context.Update(plan);
                //context.SaveChanges();

                //TrainingWeekTypeEnum overreach = context.TrainingWeekTypes.Single(x => x.Id == (uint)TrainingWeekTypeEnum.Overreach.Id);

                //WorkoutTemplateRoot wo2 = WorkoutTemplateRoot.PlanTransientWorkout(null, "WO1_1", WeekdayEnum.Monday);
                //wo2.PlanTransientExcercise(1, null);
                //wo2.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10), tempo: TUTValue.PlanTUT("3030"));
                //wo2.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10), tempo: TUTValue.PlanTUT("3030"));
                //wo2.AddTransientWorkingSet(0, WSRepetitionsValue.TrackRepetitionSerie(10), tempo: TUTValue.PlanTUT("3030"));

                //plan.PlanTransientTrainingWeek(overreach, new List<WorkoutTemplateReferenceValue>() { WorkoutTemplateReferenceValue.FromWorkoutTemplate(0, wo2) });
                ////plan.PlanWorkout(1, wo1.CloneAllWorkingSets());

                //context.Update(wo2);
                //context.Update(plan);
                //context.SaveChanges();
            }




            //using (GymContext context = new GymContext())
            //{
            //    TrainingPlanRoot plan = context.TrainingPlans.Where(x => x.Id == 1)
            //        .Include(x => x.RelationsWithChildPlans)
            //        .Single();

            //    TrainingPlanTypeEnum variant = context.TrainingPlanTypes.Find(1);


            //    plan.AttachChildPlan(2, variant);
            //    plan.AttachChildPlan(3, variant);


            //    //plan.AttachChildPlan(IdTypeValue.Create(2), TrainingPlanTypeEnum.Variant);
            //    //plan.AttachChildPlan(IdTypeValue.Create(3), TrainingPlanTypeEnum.Variant);

            //    //foreach (var x in plan.RelationsWithChildPlans)
            //    //    context.Entry(x).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

            //    context.Update(plan);
            //    context.SaveChanges();
            //}
        }


        private static void SeedData()
        {
            using (GymContext context = new GymContext())
            {
                AccountStatusTypeEnum active = context.AccountStatusTypes.Single(x => x.Id == AccountStatusTypeEnum.Active.Id);

                UserRoot user1 = UserRoot.RegisterUser("user1@email.com", "user1", "pwd1", "salt1", DateTime.UtcNow, active);
                context.Add(user1);
                context.SaveChanges();

                UserRoot user2 = UserRoot.RegisterUser("user2@email.com", "user2", "pwd2", "salt2", DateTime.UtcNow, active);
                context.Add(user2);
                context.SaveChanges();

                TestDataSeed.WorkUnitNoteDataSeed();
                TestDataSeed.HashtagDataSeed();
                TestDataSeed.ProficiencyDataSeed();
                TestDataSeed.ExcerciseDataSeed();
            }
        }




    }
}
