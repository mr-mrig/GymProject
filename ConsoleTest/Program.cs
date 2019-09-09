using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (GymContext context = new GymContext())
            {

                TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan("Test plan", true, 363678);
                TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan("Variant 1", false, 363678);
                TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan("Variant 2", false, 363678);


                context.Add(plan);
                context.Add(plan2);
                context.Add(plan3);

                context.SaveChanges();

            }

            using (GymContext context = new GymContext())
            {
                TrainingPlanRoot plan = context.TrainingPlans.Where(x => x.Id == 1)
                    .Include(x => x.RelationsWithChildPlans)
                    .Single();

                TrainingPlanTypeEnum variant = context.TrainingPlanTypes.Find(1);


                plan.AttachChildPlan(2, variant);
                plan.AttachChildPlan(3, variant);


                //plan.AttachChildPlan(IdTypeValue.Create(2), TrainingPlanTypeEnum.Variant);
                //plan.AttachChildPlan(IdTypeValue.Create(3), TrainingPlanTypeEnum.Variant);

                //foreach (var x in plan.RelationsWithChildPlans)
                //    context.Entry(x).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

                context.Update(plan);
                context.SaveChanges();
            }
        }
    }
}
