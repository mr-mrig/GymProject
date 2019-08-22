//using GymProject.Domain.Base;
//using GymProject.Domain.DietDomain.DietPlanAggregate;
//using GymProject.Domain.Test.Util;
//using GymProject.Domain.TrainingDomain.Common;
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


//        [Fact]
//        public static void TrainingPlanFullTest()
//        {
//            WSRepetitionValue reps = WSRepetitionValue.TrackRepetitionSerie(10);
//            WSRepetitionValue reps2 = WSRepetitionValue.TrackRepetitionSerie(2);
//            WorkingSetTemplate ws = WorkingSetTemplate.LoadWorkingSet(null, 0, reps);
//            WorkingSetTemplate ws1 = WorkingSetTemplate.LoadWorkingSet(null, 0, reps);
//            WorkingSetTemplate ws2 = WorkingSetTemplate.LoadWorkingSet(null, 0, reps);
//            WorkingSetTemplate ws3 = WorkingSetTemplate.LoadWorkingSet(IdTypeValue.Create(1), 0, reps);
//            WorkingSetTemplate ws4 = WorkingSetTemplate.LoadWorkingSet(ws3.Id, 0, reps2);
//            WorkingSetTemplate ws5 = ws;

//            Assert.Null(ws.Id);
//            Assert.Null(ws1.Id);
//            Assert.True(ws.IsTransient());
//            Assert.True(ws1.IsTransient());
//            Assert.True(ws2.IsTransient());
//            Assert.False(ws3.IsTransient());
//            Assert.False(ws4.IsTransient());
//            Assert.True(ws5.IsTransient());
//            Assert.NotEqual(ws, ws1);
//            Assert.NotEqual(ws, ws3);
//            Assert.Equal(ws4, ws3);

//            Assert.False(ws.Equals(ws1));

//            List<WorkingSetTemplate> wslist = new List<WorkingSetTemplate>() { ws, ws1, ws4 };
//            Assert.Contains(ws, wslist);
//            Assert.DoesNotContain(ws2, wslist);
//            Assert.Contains(ws1, wslist);
//            Assert.Contains(ws3, wslist);
//            Assert.Contains(ws4, wslist);

//            ws2 = WorkingSetTemplate.LoadWorkingSet(ws3.Id, 0, reps2);
//            Assert.Equal(ws2, ws4);
//            Assert.Equal(ws, ws5);
//            Assert.Contains(ws5, wslist);

//            // Demonstrate that Equals manages correctly transient - IE: with NULL Ids - entities 
//            IdTypeValue excerciseId = IdTypeValue.Create(22);
//            List<IdTypeValue> workUnitTechniqueIds = new List<IdTypeValue>()
//            {
//                IdTypeValue.Create(1),
//                IdTypeValue.Create(22),
//            };

//            WorkUnitTemplate wu = WorkUnitTemplate.PlanWorkUnit(null, 0, excerciseId, new List<WorkingSetTemplate>(), workUnitTechniqueIds);    // Empty

//            WorkingSetTemplate toAdd1 = WorkingSetTemplate.LoadWorkingSet(null, 0, reps, effort: TrainingEffortValue.AsRPE(1));        // Transient
//            wu.LoadWorkingSet(toAdd1);
//            Assert.Equal(TrainingEffortTypeEnum.RPE, wu.FindWorkingSetByProgressiveNumber(0).Effort.EffortType);

//            toAdd1.ChangeEffort(TrainingEffortValue.AsRM(22f));
//            Assert.Equal(TrainingEffortTypeEnum.RM, wu.FindWorkingSetByProgressiveNumber(0).Effort.EffortType);

//            WorkingSetTemplate toAdd2 = toAdd1;
//            wu.LoadWorkingSet(toAdd2);

//            Assert.Single(wu.WorkingSets);      // Element not added as toAdd2 is Equal to toAdd1, even if transient




//            Assert.Equal(workUnitTechniqueIds.Count, wu.FindWorkingSetByProgressiveNumber(0).IntensityTechniqueIds.Count);
//            // Invalid invariant!
//            toAdd1.RemoveIntensityTechnique(workUnitTechniqueIds[0]);   // Doing this through the WU would raise an exception
//            Assert.Equal(workUnitTechniqueIds.Count - 1, wu.FindWorkingSetByProgressiveNumber(0).IntensityTechniqueIds.Count);

//            wu.FindWorkingSetByProgressiveNumber(0).RemoveIntensityTechnique(IdTypeValue.Create(22));
//            Assert.Equal(workUnitTechniqueIds.Count - 2, wu.FindWorkingSetByProgressiveNumber(0).IntensityTechniqueIds.Count);



//            // -----------------------------------------------------
//            // Application Logic Example
//            // -----------------------------------------------------


//            IdTypeValue wuId = IdTypeValue.Create(7);
//            WorkoutTemplate wo = WorkoutTemplate.PlanWorkout(null, 0, new List<WorkUnitTemplate>(), "My WO");
//            wo.AddWorkUnit(excerciseId, new List<WorkingSetTemplate>());
//            wo.LoadWorkingSet(wuId, WSRepetitionValue.TrackAMRAP());
//            wo.LoadWorkingSet(wuId, WSRepetitionValue.TrackAMRAP());
//            wo.LoadWorkingSet(wuId, WSRepetitionValue.TrackAMRAP());
//            wo.RemoveWorkingSet(wu.ProgressiveNumber, ws.ProgressiveNumber);

//            IdTypeValue wsId = IdTypeValue.Create(8);
//            wo.ChangeWorkingSetLiftingTempo(wsId, TUTValue.PlanTUT("2222"));

//            wo.AddWorkUnit(excerciseId + 1, new List<WorkingSetTemplate>());
//            wo.AddWorkUnitIntensityTechnique(wuId + 1, IdTypeValue.Create(3333));

//            TrainingIntensityParametersValue workoutIntensity = wo.TrainingIntensity;  // UI-side

//            WorkoutTemplate wo2 = WorkoutTemplate.PlanWorkout(null, 0, new List<WorkUnitTemplate>(), "My WO");
//            // Replicate operations from WO1

//            IdTypeValue weekId = IdTypeValue.Create(9173);
//            TrainingPlan plan = TrainingPlan.CreateTrainingPlan(null, "My Plan", false, false, IdTypeValue.Create(666));
//            plan.AddTrainingWeek(new List<WorkoutTemplate>(), TrainingWeekTypeEnum.Generic);
//            plan.AddWorkout(weekId, wo.Id);
//            plan.AddWorkout(weekId, wo2.Id);

//            List<WorkingSetTemplate> workingSets = wo.GetAllWorkingSets().Union(wo2.GetAllWorkingSets()).ToList();

//            plan.FindTrainingWeekById(weekId).TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(workingSets);

//            TrainingIntensityParametersValue planIntensity = plan.TrainingIntensity;  // UI-side

//            IdTypeValue id1 = IdTypeValue.Create(1);
//            IdTypeValue id2 = IdTypeValue.Create(2);
//            IdTypeValue idRes = id1 + id2;
//        }



//    }
//}
