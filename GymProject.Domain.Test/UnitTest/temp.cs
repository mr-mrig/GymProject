//using GymProject.Domain.Base;
//using GymProject.Domain.DietDomain.DietPlanAggregate;
//using GymProject.Domain.Test.Util;
//using GymProject.Domain.TrainingDomain.Common;
//using GymProject.Domain.TrainingDomain.Exceptions;
//using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
//using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Xunit;

//namespace GymProject.Domain.Test.UnitTest
//{
//    public class temp
//    {


//        //        [Fact]
//        //        public static void TrainingPlanFullTest()
//        //        {
//        //            WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie(10);
//        //            WSRepetitionValue reps2 = WSRepetitionValue.TrackRepetitionSerie(2);
//        //            WorkingSetTemplate ws = WorkingSetTemplate.LoadWorkingSet(null, 0, reps);
//        //            WorkingSetTemplate ws1 = WorkingSetTemplate.LoadWorkingSet(null, 0, reps);
//        //            WorkingSetTemplate ws2 = WorkingSetTemplate.LoadWorkingSet(null, 0, reps);
//        //            WorkingSetTemplate ws3 = WorkingSetTemplate.LoadWorkingSet(1, 0, reps);
//        //            WorkingSetTemplate ws4 = WorkingSetTemplate.LoadWorkingSet(ws3.Id, 0, reps2);
//        //            WorkingSetTemplate ws5 = ws;

//        //            Assert.Null(ws.Id);
//        //            Assert.Null(ws1.Id);
//        //            Assert.True(ws.IsTransient());
//        //            Assert.True(ws1.IsTransient());
//        //            Assert.True(ws2.IsTransient());
//        //            Assert.False(ws3.IsTransient());
//        //            Assert.False(ws4.IsTransient());
//        //            Assert.True(ws5.IsTransient());
//        //            Assert.NotEqual(ws, ws1);
//        //            Assert.NotEqual(ws, ws3);
//        //            Assert.Equal(ws4, ws3);

//        //            Assert.False(ws.Equals(ws1));

//        //            List<WorkingSetTemplate> wslist = new List<WorkingSetTemplate>() { ws, ws1, ws4 };
//        //            Assert.Contains(ws, wslist);
//        //            Assert.DoesNotContain(ws2, wslist);
//        //            Assert.Contains(ws1, wslist);
//        //            Assert.Contains(ws3, wslist);
//        //            Assert.Contains(ws4, wslist);

//        //            ws2 = WorkingSetTemplate.LoadWorkingSet(ws3.Id, 0, reps2);
//        //            Assert.Equal(ws2, ws4);
//        //            Assert.Equal(ws, ws5);
//        //            Assert.Contains(ws5, wslist);

//        //            // Demonstrate that Equals manages correctly transient - IE: with NULL Ids - entities 
//        //            uint? excerciseId = (uint?)(22);
//        //            List<uint?> workUnitTechniqueIds = new List<uint?>()
//        //            {
//        //                1,
//        //                (uint?)(22),
//        //            };

//        //            WorkUnitTemplate wu = WorkUnitTemplate.PlanWorkUnit(null, 0, excerciseId, new List<WorkingSetTemplate>(), workUnitTechniqueIds);    // Empty

//        //            WorkingSetTemplate toAdd1 = WorkingSetTemplate.LoadWorkingSet(null, 0, reps, effort: TrainingEffortValue.AsRPE(1));        // Transient
//        //            wu.LoadWorkingSet(toAdd1);
//        //            Assert.Equal(TrainingEffortTypeEnum.RPE, wu.FindWorkingSetByProgressiveNumber(0).Effort.EffortType);

//        //            toAdd1.ChangeEffort(TrainingEffortValue.AsRM(22f));
//        //            Assert.Equal(TrainingEffortTypeEnum.RM, wu.FindWorkingSetByProgressiveNumber(0).Effort.EffortType);

//        //            WorkingSetTemplate toAdd2 = toAdd1;
//        //            wu.LoadWorkingSet(toAdd2);

//        //            Assert.Single(wu.WorkingSets);      // Element not added as toAdd2 is Equal to toAdd1, even if transient




//        //            Assert.Equal(workUnitTechniqueIds.Count, wu.FindWorkingSetByProgressiveNumber(0).IntensityTechniqueIds.Count);
//        //            // Invalid invariant!
//        //            toAdd1.RemoveIntensityTechnique(workUnitTechniqueIds[0]);   // Doing this through the WU would raise an exception
//        //            Assert.Equal(workUnitTechniqueIds.Count - 1, wu.FindWorkingSetByProgressiveNumber(0).IntensityTechniqueIds.Count);

//        //            wu.FindWorkingSetByProgressiveNumber(0).RemoveIntensityTechnique((uint?)(22));
//        //            Assert.Equal(workUnitTechniqueIds.Count - 2, wu.FindWorkingSetByProgressiveNumber(0).IntensityTechniqueIds.Count);

//        // }

//        [Fact]
//        public static void logicExample()
//        {
//            // -----------------------------------------------------
//            // Application Logic Example
//            // -----------------------------------------------------


//            //uint? wuId = (uint?)(7);
//            //WorkoutTemplate wo = WorkoutTemplate.PlanWorkout(null, 0, new List<WorkUnitTemplate>(), "My WO");
//            //wo.AddWorkUnit(excerciseId, new List<WorkingSetTemplate>());
//            //wo.LoadWorkingSet(wuId, WSRepetitionValue.TrackAMRAP());
//            //wo.LoadWorkingSet(wuId, WSRepetitionValue.TrackAMRAP());
//            //wo.LoadWorkingSet(wuId, WSRepetitionValue.TrackAMRAP());
//            //wo.RemoveWorkingSet(wu.ProgressiveNumber, ws.ProgressiveNumber);

//            //uint? wsId = (uint?)(8);
//            //wo.ChangeWorkingSetLiftingTempo(wsId, TUTValue.PlanTUT("2222"));

//            //wo.AddWorkUnit(excerciseId + 1, new List<WorkingSetTemplate>());
//            //wo.AddWorkUnitIntensityTechnique(wuId + 1, (uint?)(3333));

//            //TrainingIntensityParametersValue workoutIntensity = wo.TrainingIntensity;  // UI-side

//            //WorkoutTemplate wo2 = WorkoutTemplate.PlanWorkout(null, 0, new List<WorkUnitTemplate>(), "My WO");
//            //// Replicate operations from WO1

//            //uint? weekId = (uint?)(9173);
//            //TrainingPlan plan = TrainingPlan.CreateTrainingPlan(null, "My Plan", false, false, (uint?)(666));
//            //plan.AddTrainingWeek(new List<WorkoutTemplate>(), TrainingWeekTypeEnum.Generic);
//            //plan.AddWorkout(weekId, wo.Id);
//            //plan.AddWorkout(weekId, wo2.Id);

//            //List<WorkingSetTemplate> workingSets = wo.GetAllWorkingSets().Union(wo2.GetAllWorkingSets()).ToList();

//            //plan.FindTrainingWeekById(weekId).TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(workingSets);

//            //TrainingIntensityParametersValue planIntensity = plan.TrainingIntensity;  // UI-side

//            //uint? id1 = 1;
//            //uint? id2 = (uint?)(2);
//            //uint? idRes = id1 + id2;

//            //IList<uint?> intTechniquesintTechniques = new List<uint?>
//            //{
//            //    1, (uint?)(2),
//            //};

//            //WorkoutTemplate wo = WorkoutTemplate.PlanWorkout((uint?)(22), null, "WO");

//            //WorkUnitTemplate wu = WorkUnitTemplate.PlanWorkUnit((uint?)(22), 0, (uint?)(2345),
//            //    new List<WorkingSetTemplate>() { WorkingSetTemplate.PlanWorkingSet((uint?)(12), 0, WSRepetitionValue.TrackRepetitionSerie(22)), },
//            //    intTechniquesintTechniques);

//            //wo.AddWorkUnit(wu);

//            ////WorkingSetTemplate ws = WorkingSetTemplate.PlanWorkingSet((uint?)(88), 0, WSRepetitionValue.TrackRepetitionSerie(22));

//            ////// Invalid domain detected -> wrong progn
//            ////Assert.Throws<TrainingDomainInvariantViolationException>(() => wo.AddWorkingSet(0, ws));

//            //WorkingSetTemplate ws = WorkingSetTemplate.PlanWorkingSet((uint?)(5701), 10, WSRepetitionValue.TrackRepetitionSerie(22));

//            //// Invalid domain detected -> wrong progn
//            //Assert.Throws<TrainingDomainInvariantViolationException>(() => wo.AddWorkingSet(0, ws));

//            //ws = WorkingSetTemplate.PlanWorkingSet((uint?)(88), 1, WSRepetitionValue.TrackRepetitionSerie(22));

//            //// Invalid domain detected -> invalid int technique
//            //Assert.Throws<TrainingDomainInvariantViolationException>(() => wo.AddWorkingSet(0, ws));


//            List<TrainingWeekTemplate> weeks = new List<TrainingWeekTemplate>()
//            {
//                TrainingWeekTemplate.PlanTransientTrainingWeek(0),
//                TrainingWeekTemplate.PlanTransientTrainingWeek(1),
//                TrainingWeekTemplate.PlanTransientTrainingWeek(2),
//            };

//            TrainingPlan plan = TrainingPlan.CreateTrainingPlan(1, "", false, false, (uint?)(2),trainingWeeks: weeks);

//            Assert.Equal(3, plan.TrainingWeeks.Count);

//            weeks.Add(TrainingWeekTemplate.PlanTransientTrainingWeek(3));
//            weeks.Add(TrainingWeekTemplate.PlanTransientTrainingWeek(3));
//            weeks.Add(TrainingWeekTemplate.PlanTransientTrainingWeek(3));

//            Assert.Equal(3, plan.TrainingWeeks.Count);

//            List<WorkoutTemplateReferenceValue> workouts = new List<WorkoutTemplateReferenceValue>()
//            {
//                WorkoutTemplateReferenceValue.BuildLinkToWorkout(0, null),
//                WorkoutTemplateReferenceValue.BuildLinkToWorkout(1, null),
//            };

//            List<WorkingSetTemplate> wss = new List<WorkingSetTemplate>()
//            {
//                WorkingSetTemplate.PlanWorkingSet(1, 0, WSRepetitionValue.TrackRepetitionSerie(10)),
//                WorkingSetTemplate.PlanWorkingSet((uint?)(2), 1, WSRepetitionValue.TrackRepetitionSerie(10)),
//                WorkingSetTemplate.PlanWorkingSet((uint?)(3), 2, WSRepetitionValue.TrackRepetitionSerie(10)),
//                WorkingSetTemplate.PlanWorkingSet((uint?)(4), 3, WSRepetitionValue.TrackRepetitionSerie(10)),
//            };

//            plan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.Overreach, workouts);

//            plan.AddWorkingSets((uint)(plan.TrainingWeeks.Count() - 1), 0, wss);
//            Assert.Equal(4, plan.CloneAllWorkingSets().Count());

//            wss.Add(WorkingSetTemplate.PlanWorkingSet(1, 0, WSRepetitionValue.TrackRepetitionSerie(10)));
//            wss.Add(WorkingSetTemplate.PlanWorkingSet(1, 0, WSRepetitionValue.TrackRepetitionSerie(10)));
//            wss.Add(WorkingSetTemplate.PlanWorkingSet(1, 0, WSRepetitionValue.TrackRepetitionSerie(10)));

//            Assert.Equal(4, plan.CloneAllWorkingSets().Count());



//        }



//    }
//}
