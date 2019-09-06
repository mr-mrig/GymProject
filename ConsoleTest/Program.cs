using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Collections.Generic;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using(GymContext context = new GymContext())
            {

                TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan(IdTypeValue.Create(11), "Test plan", true, IdTypeValue.Create(363678));
                TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan(IdTypeValue.Create(1999), "Variant 1", false, IdTypeValue.Create(363678));
                TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan(IdTypeValue.Create(19999), "Variant 2", false, IdTypeValue.Create(363678));


                //TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan("Test plan", true, IdTypeValue.Create(363678));
                //TrainingPlanRoot plan2 = TrainingPlanRoot.CreateTrainingPlan("Variant 1", false, IdTypeValue.Create(363678));
                //TrainingPlanRoot plan3 = TrainingPlanRoot.CreateTrainingPlan("Variant 2", false, IdTypeValue.Create(363678));


                plan.AttachChildPlan(plan2.Id, TrainingPlanTypeEnum.Variant);
                plan.AttachChildPlan(plan3.Id, TrainingPlanTypeEnum.Variant);

                //plan2.AttachChildPlan(plan3.Id, TrainingPlanTypeEnum.Variant);
                //plan3.AttachParentPlan(plan.Id, TrainingPlanTypeEnum.Variant);

                //context.TrainingPlans.Add(plan);
                //context.TrainingPlans.Add(plan2);
                //context.TrainingPlans.Add(plan3);

                context.Add(plan);
                context.Add(plan2);
                context.Add(plan3);
                //context.TrainingPlanRelations.AddRange(plan.RelationsWithChildPlans);

                context.SaveChanges();
            }
        }
    }
}
