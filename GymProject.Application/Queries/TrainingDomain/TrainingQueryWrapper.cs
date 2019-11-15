using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions;
using GymProject.Application.Queries.Base;
using System.Data;

namespace GymProject.Application.Queries.TrainingDomain
{
    public class TrainingQueryWrapper : ITrainingQueryWrapper
    {


        private string _connectionString = string.Empty;

        /// <summary>
        /// Shared DB Connection, always keep it opened.
        /// </summary>
        private SQLiteConnection _db;



        public TrainingQueryWrapper(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));


            _db = new SQLiteConnection(_connectionString);
            _db.OpenGymAppConnection();
        }





        /// <summary>
        /// Get the summaries of all the Training Plans belonging to the specified user
        /// </summary>
        /// <param name="userId">The ID of the user to be searched</param>
        /// <returns>The Training Plan Summary DTO list</returns>
        public async Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummaries(uint userId)
        {
            string sql = @"SELECT TP.Id As TrainingPlanId, TP.Name as TrainingPlanName, TP.IsBookmarked, 
                	--Average Workout Days per plan
                    (
                        SELECT AVG(WoCount.Counter)
                        FROM
                        (
                            SELECT TrainingPlanId, count(1) as Counter
                            FROM TrainingWeek
                            JOIN WorkoutTemplate
                            ON TrainingWeek.Id = TrainingWeekId
                            WHERE TrainingPlanId = TP.Id
                            GROUP BY TrainingWeek.Id
		                ) WoCount
	                ) AS AvgWorkoutDays,
                    --Average weekly working sets
                    (
                        SELECT round(Avg(WsCount.Counter), 1)
                        FROM
                        (
                            SELECT TrainingWeek.Id, count(1) as Counter
                            FROM TrainingWeek
                            JOIN WorkoutTemplate
                            ON TrainingWeek.Id = TrainingWeekId
                            JOIN WorkUnitTemplate
                            ON WorkoutTemplate.Id = WorkoutTemplateId
                            JOIN WorkingSetTemplate
                            ON WorkUnitTemplate.Id = WorkUnitTemplateId
                            WHERE TrainingPlanId = TP.Id
                            GROUP BY TrainingWeek.Id
                        ) WsCount
                    ) As AvgWorkingSets,
                    --Average Intensity
                    (
                        SELECT Round(Avg(
                        CASE
                            WHEN Effort_EffortTypeId = 1 THEN Effort / 10.0-- Intensity[%]
                            WHEN Effort_EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)-- RM
                            WHEN Effort_EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)-- RPE
                            ELSE null
                        END), 1) as AvgIntensity
                        FROM TrainingWeek
                        JOIN WorkoutTemplate
                        ON TrainingWeek.Id = TrainingWeekId
                        JOIN WorkUnitTemplate
                        ON WorkoutTemplate.Id = WorkoutTemplateId
                        JOIN WorkingSetTemplate
                        ON WorkUnitTemplate.Id = WorkUnitTemplateId
                        WHERE TrainingPlanId = TP.Id
                    ) As AvgIntensityPercentage,
                    --Last workout date
                    (
                        SELECT Max(StartTime)
                        FROM TrainingWeek
                        JOIN WorkoutTemplate
                        ON TrainingWeek.Id = TrainingWeekId
                        JOIN WorkoutSession
                        ON WorkoutTemplate.Id = WorkoutTemplateId
                        WHERE TrainingPlanId = TP.Id
                    ) As LastWorkoutTimestamp,
                    --Number of training weeks
                    (
                        SELECT Count(1)
                        FROM TrainingWeek
                        WHERE TrainingPlanId = TP.Id
                    ) As TrainingWeeksCounter,
	                TH.Id As HashtagId, TH.Body As Hashtag, 
                    TProf.Id as TrainingProficiencyId, TProf.Name as TrainingProficiency, 
                    Pha.Id As TrainingPhaseId, Pha.Name as TrainingPhase

                    FROM TrainingPlan TP
                    JOIN User U
                    ON TP.OwnerId = U.Id
                    -- Notes and hashtags
                    LEFT JOIN TrainingPlanHashtag TPH
                    ON TPH.TrainingPlanId = TP.Id
                    LEFT JOIN TrainingHashtag TH
                    ON TPH.HashtagId = TH.Id
                    LEFT JOIN TrainingPlanProficiency TPP
                    ON TPP.TrainingPlanId = TP.Id
                    LEFT JOIN TrainingProficiency TProf
                    ON TProf.Id = TPP.TrainingProficiencyId
                    LEFT JOIN TrainingPlanPhase TPPh
                    ON TPPh.TrainingPlanId = TP.Id
                    LEFT JOIN TrainingPhase Pha
                    ON TPPh.TrainingPhaseId = Pha.Id
                    WHERE U.Id = @userId
                    ORDER BY TP.IsBookmarked DESC";

            Dictionary<uint?, TrainingPlanSummaryDto> plans = new Dictionary<uint?, TrainingPlanSummaryDto>();

            IEnumerable<TrainingPlanSummaryDto> query = await _db.QueryAsync<TrainingPlanSummaryDto, HashtagDto, TrainingProficiencyDto, TrainingPhaseDto, TrainingPlanSummaryDto>(sql,
                map: (plan, hash, prof, phase) =>
                {
                    TrainingPlanSummaryDto trainingPlan;

                    // Training Plans
                    if (!plans.TryGetValue(plan.TrainingPlanId, out trainingPlan))
                        plans.Add(plan.TrainingPlanId, trainingPlan = plan);

                    if (trainingPlan.Hashtags == null)
                        trainingPlan.Hashtags = new List<HashtagDto>();

                    if (trainingPlan.TargetProficiencies == null)
                        trainingPlan.TargetProficiencies = new List<TrainingProficiencyDto>();

                    if (trainingPlan.TargetPhases == null)
                        trainingPlan.TargetPhases = new List<TrainingPhaseDto>();

                    // Hashtags
                    if (hash != null)
                    {
                        if (trainingPlan.Hashtags.Count(x => x.HashtagId == hash.HashtagId) == 0)
                            trainingPlan.Hashtags.Add(hash);
                    }

                    // Proficiencies
                    if (prof != null)
                    {
                        if (trainingPlan.TargetProficiencies.Count(x => x.TrainingProficiencyId == prof.TrainingProficiencyId) == 0)
                            trainingPlan.TargetProficiencies.Add(prof);
                    }

                    // Phases
                    if (phase != null)
                    {
                        if (trainingPlan.TargetPhases.Count(x => x.TrainingPhaseId == phase.TrainingPhaseId) == 0)
                            trainingPlan.TargetPhases.Add(phase);
                    }

                    return trainingPlan;

                },
                param: new { userId },
                splitOn: "HashtagId, TrainingProficiencyId, TrainingPhaseId");

            return plans.Values;
        }


        /// <summary>
        /// Get the plan over the training weeks for the specified workout day.
        /// In order to query for a specific training plan, the caller should already have all its training weeks and workouts names fetched, if not another query is needed.
        /// Furthermore, notice that the weeks IDs should be ordered according to the progressive number, otherwise further post-processing is needed.
        /// </summary>
        /// <param name="trainingWeekIds">The IDs of the Training Weeks belonging to the Training Plan to be queried, ordered by their progressive number.</param>
        /// <param name="workoutName">The name of the workout to be queried, which is unique with respect to the training week</param>
        /// <returns>The WorkoutFullPlanDto list</returns>
        public async Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutDays(List<uint> trainingWeekIds, string workoutName)
        {
            string sql = @"SELECT 
                WT.TrainingWeekId, WT.Id as WorkoutId,
                WT.Name as WorkoutName, WT.SpecificWeekday AS SpecificWeekdayId,
                WUTN.Id as NoteId, WUTN.Body as NoteBody,
                WUT.Id as WorkUnitId, WUT.ProgressiveNumber as WorkUnitProgressiveNumber, 
                IT1.Id as WuIntensityTechniqueId, IT1.Abbreviation as WuIntensityTechniqueAbbreviation,
                E.Id as ExcerciseId, E.Name as ExcerciseName,
                WST.Id as WorkingSetId, WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence AS LiftingTempo, WST.Effort,
                ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
                IT.Id as IntensityTechniqueId, IT.Abbreviation as IntensityTechniqueAbbreviation

                FROM WorkoutTemplate WT
                LEFT JOIN WorkUnitTemplate WUT
                ON WT.Id = WUT.WorkoutTemplateId
                LEFT JOIN WorkUnitTemplateNote WUTN
                ON WUTN.Id = WUT.WorkUnitNoteId
                LEFT JOIN Excercise E
                ON E.Id = WUT.ExcerciseId
                LEFT JOIN WorkingSetTemplate WST
                ON WUT.Id = WST.WorkUnitTemplateId
                LEFT JOIN TrainingEffortType ET
                ON WST.Effort_EffortTypeId = ET.Id
                LEFT JOIN IntensityTechnique IT1
                ON IT1.Id = WUT.LinkingIntensityTechniqueId

                -- Fetch all the Techniques of the sets
                LEFT JOIN
                (
                    SELECT WorkingSetId, WSTIT2.IntensityTechniqueId as Id, Abbreviation
                    FROM WorkingSetTemplate WST
                    JOIN WorkingSetIntensityTechnique WSTIT2
                    ON WSTIT2.WorkingSetId = WST.Id
                    LEFT JOIN IntensityTechnique IT2
                    ON IT2.Id = WSTIT2.IntensityTechniqueId

                ) IT
                ON IT.WorkingSetId = WST.Id

                WHERE WT.TrainingWeekId IN @trainingWeekIds
                AND WT.Name = @workoutName

                ORDER BY WT.TrainingWeekId, WT.ProgressiveNumber, WUT.ProgressiveNumber, WST.ProgressiveNumber";


            Dictionary<uint?, WorkoutFullPlanDto> workouts = new Dictionary<uint?, WorkoutFullPlanDto>();
            Dictionary<uint?, WorkUnitDto> workUnits = new Dictionary<uint?, WorkUnitDto>();

            IEnumerable<WorkoutFullPlanDto> query = await _db.QueryAsync<WorkoutFullPlanDto, WorkUnitDto, WorkingSetDto, IntensityTechniqueDto, WorkoutFullPlanDto>(sql,
                map: (wo, wu, ws, it) =>
                {
                    WorkoutFullPlanDto workout;
                    WorkUnitDto workUnit;

                    // Workouts
                    if (!workouts.TryGetValue(wo.WorkoutId, out workout))
                        workouts.Add(wo.WorkoutId, workout = wo);

                    if (workout.WorkUnits == null)
                        workout.WorkUnits = new List<WorkUnitDto>();

                    if (wu != null)
                    {
                        if (workout.WorkUnits.Count(x => x.WorkUnitId == wu.WorkUnitId) == 0)
                            workout.WorkUnits.Add(wu);

                        // Work Units
                        if (!workUnits.TryGetValue(wu.WorkUnitId, out workUnit))
                            workUnits.Add(wu.WorkUnitId, workUnit = wu);

                        if (workUnit.WorkingSets == null)
                            workUnit.WorkingSets = new List<WorkingSetDto>();

                        if (ws != null)
                        {
                            // WorkingSets
                            if (workUnit.WorkingSets.Count(x => x.WorkingSetId == ws.WorkingSetId) == 0)
                                workUnit.WorkingSets.Add(ws);

                            // WS Intensity Techniques - Allow duplicates: the same IT can be linked to different WSs
                            WorkingSetDto currentWs = workUnit.WorkingSets.Single(x => x.WorkingSetId == ws.WorkingSetId);

                            if (currentWs?.IntensityTechniques == null)
                                currentWs.IntensityTechniques = new List<IntensityTechniqueDto>();

                            if (it != null)
                                currentWs.IntensityTechniques.Add(it);
                        }
                    }

                    return workout;

                },
                param: new { trainingWeekIds, workoutName},
                splitOn: "WorkUnitId, WorkingSetId, IntensityTechniqueId");

            // HERE OR CLIENT-SIDE?
            // 1. Insert missing Weeks

            // 2. Sort according to the Training Week - This breaks if the input Training Weeks are not sorted Progressive Number-wise
            IEnumerable<WorkoutFullPlanDto> result = workouts.Values
                .OrderBy(x => trainingWeekIds.IndexOf(x.TrainingWeekId));

            return result;
        }


        /// <summary>
        /// Get all the Feedbacks - full detailed - of the Training Plans which are direct childs of the specified Training Plan.
        /// A training plan is considered direct child if:
        ///    1. it is the parent plan of the query
        ///    2. it is a direct variant of the parent plan
        ///    3. it is directly inherited from the parent plan
        ///    4. it is directly inherited of the 2. plans
        /// Direct childs of the selected Plan which have never been scheduled are still returned
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Root Training Plan</param>
        /// <param name="userId">The ID of the user who is querying - This is needed n order to skip a JOIN</param>
        /// <returns>The FullFeedbackDetailDto list</returns>
        public async Task<IEnumerable<FullFeedbackDetailDto>> GetFullFeedbacksDetails(uint trainingPlanId, uint userId)
        {
            string sql = @"SELECT TPRoot.Id as TrainingPlanId, TPRoot.Name as TrainingPlanName, U.Id as UserId, U.Username as UserName,
                    TS.Id as ScheduleId, Date(TS.StartDate, 'unixepoch') as StartDate, Date(TS.EndDate, 'unixepoch') as EndDate,
                    Ph.Id as TrainingPhaseId, Ph.Name as TrainingPhase,
                    TProf.Id as TrainingProficiencyId, TProf.Name as TrainingProficiency,
                    TSF.Id as FeedbackId, TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating, TSF.UserId as FeedbackOwnerId

                    FROM TrainingPlan TPRoot
                    LEFT JOIN TrainingPlanRelation TPR
                    ON TPRoot.Id = TPR.ChildPlanId
                    JOIN User U
                    ON TPRoot.OwnerId = U.Id

                    -- Proficiency
                    LEFT JOIN TrainingSchedule TS
                    ON TS.TrainingPlanId = TPRoot.Id
                    LEFT JOIN UserProficiency UHP
                    ON UHP.UserId = U.Id
                    AND UHP.OwnerId = @userId
                    AND UHP.StartDate <= TS.Startdate
                    AND COALESCE(UHP.EndDate, TS.Startdate + 1) >= TS.Startdate
                    LEFT JOIN TrainingProficiency TProf
                    ON TProf.Id = UHP.TrainingProficiencyId

                    -- TrainingPhase
                    LEFT JOIN UserPhase UP
                    ON UP.OwnerId = @userId
                    AND UP.UserId = U.Id                -- Denormalized
                    AND UP.StartDate <= TS.StartDate
                    AND COALESCE(UP.EndDate, TS.Startdate + 1) >= TS.Startdate
                    LEFT JOIN TrainingPhase Ph
                    ON UP.TrainingPhaseId = Ph.Id

                    -- Feedback
                    LEFT JOIN TrainingScheduleFeedback TSF
                    ON TSF.TrainingScheduleId = TS.Id

                    -- Variants of main plan
                    WHERE TPRoot.Id IN
                    (
                        GetDirectChildPlansOf(@trainingPlanId)
                    )
                    ORDER BY StartDate";

            Dictionary<uint?, FullFeedbackDetailDto> plans = new Dictionary<uint?, FullFeedbackDetailDto>();
            Dictionary<uint?, TrainingScheduleDto> schedules = new Dictionary<uint?, TrainingScheduleDto>();

            IEnumerable<FullFeedbackDetailDto> query = await _db.QueryAsync<FullFeedbackDetailDto, TrainingScheduleDto, FeedbackDto, FullFeedbackDetailDto>(sql,
                map: (plan, sched, fbk) =>
                {
                    FullFeedbackDetailDto trainingPlan;
                    TrainingScheduleDto schedule;

                    // Training Plans
                    if (!plans.TryGetValue(plan.TrainingPlanId, out trainingPlan))
                        plans.Add(plan.TrainingPlanId, trainingPlan = plan);

                    if (trainingPlan.TrainingSchedules == null)
                        trainingPlan.TrainingSchedules = new List<TrainingScheduleDto>();

                    if (sched != null)
                    {
                        if (trainingPlan.TrainingSchedules.Count(x => x.ScheduleId == sched.ScheduleId) == 0)
                            trainingPlan.TrainingSchedules.Add(sched);

                        // Training Schedules
                        if (!schedules.TryGetValue(sched.ScheduleId, out schedule))
                            schedules.Add(sched.ScheduleId, schedule = sched);

                        if (schedule.Feedbacks == null)
                            schedule.Feedbacks = new List<FeedbackDto>();

                        if (fbk != null)
                        {
                            // Schedule Feedbacks
                            if (schedule.Feedbacks.Count(x => x.FeedbackId == fbk.FeedbackId) == 0)
                                schedule.Feedbacks.Add(fbk);
                        }
                    }
                    return trainingPlan;

                },
                param: new { trainingPlanId, userId },
                splitOn: "ScheduleId, FeedbackId");

            return plans.Values;
        }


        /// <summary>
        /// Get the main details of the selected Training Plan.
        /// These include all the fields not fethced in the <see cref="TrainingPlanSummaryDto"/> excluding:
        ///
        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
        ///
        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
        ///
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        public async Task<IEnumerable<TrainingPlanDetailDto>> GetTrainingPlanDetails(uint trainingPlanId)
        {
            string sql = @"SELECT 
                    TP.Id, TP.TrainingPlanNoteId, TPN.Body as TrainingPlanNote,
                    MG.Id as FocusedMuscleId, MG.Abbreviation as FocusedMuscleAbbreviation, MG.Name as FocusedMuscle,
                    ParentPlan.Id as ParentPlanId, ParentPlan.OwnerId as ParentPlanOwnerId, TPR.ChildPlanTypeId as RelationTypeId
                    FROM TrainingPlan TP

                    -- TP missing data
                    LEFT JOIN TrainingPlanNote TPN
                    ON TP.TrainingPlanNoteId = TPN.Id
                    LEFT JOIN TrainingPlanMuscleFocus MF
                    ON TP.Id = MF.TrainingPlanId
                    LEFT JOIN MuscleGroup MG
                    ON MG.Id = MF.MuscleGroupId

                    --Parent plan
                    LEFT JOIN TrainingPlanRelation TPR
                    ON TP.Id = TPR.ChildPlanId
                    LEFT JOIN TrainingPlan ParentPlan
                    ON ParentPlan.Id = TPR.ParentPlanId

                    WHERE TP.Id = @trainingPlanId";

            Dictionary<uint?, TrainingPlanDetailDto> plans = new Dictionary<uint?, TrainingPlanDetailDto>();

            IEnumerable<TrainingPlanDetailDto> result = await _db.QueryAsync<TrainingPlanDetailDto, MuscleFocusDto, TrainingPlanDetailDto>(sql,
                map: (plan, focus) =>
                {
                    TrainingPlanDetailDto trainingPlan;

                    // Training Plans
                    if (!plans.TryGetValue(plan.TrainingPlanId, out trainingPlan))
                        plans.Add(plan.TrainingPlanId, trainingPlan = plan);

                    if (trainingPlan.MusclesFocuses == null)
                        trainingPlan.MusclesFocuses = new List<MuscleFocusDto>();

                    // Focused Muscles
                    if (focus != null)
                    {
                        if (trainingPlan.MusclesFocuses.Count(x => x.FocusedMuscleId == focus.FocusedMuscleId) == 0)
                            trainingPlan.MusclesFocuses.Add(focus);
                    }

                    return trainingPlan;

                },
                param: new { trainingPlanId },
                splitOn: "FocusedMuscleId");

            return plans.Values;
        }


        /// <summary>
        /// Get the all the details of the selected Training Plan, when no pre-fetch data is available, namely no  <see cref="TrainingPlanSummaryDto"/> in memory.
        /// No data about Workouts and Feedbaks/Variants are loaded, please refer to:
        ///
        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
        ///
        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
        ///
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        public async Task<TrainingPlanFullDetailDto> GetTrainingPlan(uint trainingPlanId)
        {
            string sql = @"SELECT 
                TP.Id as TrainingPlanId, TP.Name as TrainingPlanName, TP.IsBookmarked,
                TP.TrainingPlanNoteId, TPN.Body as TrainingPlanNote,
                ParentPlan.Id as ParentPlanId, ParentPlan.OwnerId as ParentPlanOwnerId, TPR.ChildPlanTypeId as RelationTypeId,
                --Number of training weeks
                (
                    SELECT Count(1)
                    FROM TrainingWeek
                    WHERE TrainingPlanId = TP.Id
                ) As TrainingWeeksCounter,
                TH.Id As HashtagId, TH.Body As Hashtag, 
                TProf.Id as TrainingProficiencyId, TProf.Name as TrainingProficiency, 
                Pha.Id As TrainingPhaseId, Pha.Name as TrainingPhase,
                MG.Id as FocusedMuscleId, MG.Abbreviation as FocusedMuscleAbbreviation, MG.Name as FocusedMuscle

                FROM TrainingPlan TP
                -- TP missing data
                LEFT JOIN TrainingPlanNote TPN
                ON TP.TrainingPlanNoteId = TPN.Id
                LEFT JOIN TrainingPlanMuscleFocus MF
                ON TP.Id = MF.TrainingPlanId
                LEFT JOIN MuscleGroup MG
                ON MG.Id = MF.MuscleGroupId

                --Parent plan
                LEFT JOIN TrainingPlanRelation TPR
                ON TP.Id = TPR.ChildPlanId
                LEFT JOIN TrainingPlan ParentPlan
                ON ParentPlan.Id = TPR.ParentPlanId

                -- Notes and hashtags
                LEFT JOIN TrainingPlanHashtag TPH
                ON TPH.TrainingPlanId = TP.Id
                LEFT JOIN TrainingHashtag TH
                ON TPH.HashtagId = TH.Id
                LEFT JOIN TrainingPlanProficiency TPP
                ON TPP.TrainingPlanId = TP.Id
                LEFT JOIN TrainingProficiency TProf
                ON TProf.Id = TPP.TrainingProficiencyId
                LEFT JOIN TrainingPlanPhase TPPh
                ON TPPh.TrainingPlanId = TP.Id
                LEFT JOIN TrainingPhase Pha
                ON TPPh.TrainingPhaseId = Pha.Id

                WHERE TP.Id = @trainingPlanId";


            //Dictionary<uint?, TrainingPlanFullDetailDto> plans = new Dictionary<uint?, TrainingPlanFullDetailDto>();
            TrainingPlanFullDetailDto trainingPlan = null;

            IEnumerable<TrainingPlanFullDetailDto> query = await _db.QueryAsync<TrainingPlanFullDetailDto, HashtagDto, TrainingProficiencyDto, TrainingPhaseDto, MuscleFocusDto, TrainingPlanFullDetailDto>(sql,
                map: (plan, hashtag, prof, phase, focus) =>
                {
                    if(trainingPlan == null)
                    {
                        trainingPlan = plan;
                        trainingPlan.Hashtags = new List<HashtagDto>();
                        trainingPlan.TargetProficiencies = new List<TrainingProficiencyDto>();
                        trainingPlan.TargetPhases = new List<TrainingPhaseDto>();
                        trainingPlan.MusclesFocuses = new List<MuscleFocusDto>();
                    }

                    //if (trainingPlan.Hashtags == null)
                    //    trainingPlan.Hashtags = new List<HashtagDto>();

                    //if (trainingPlan.TargetProficiencies == null)
                    //    trainingPlan.TargetProficiencies = new List<TrainingProficiencyDto>();

                    //if (trainingPlan.TargetPhases == null)
                    //    trainingPlan.TargetPhases = new List<TrainingPhaseDto>();

                    //if (trainingPlan.MusclesFocuses == null)
                    //    trainingPlan.MusclesFocuses = new List<MuscleFocusDto>();

                    // Hashtags
                    if (hashtag != null)
                    {
                        if (trainingPlan.Hashtags.Count(x => x.HashtagId == hashtag.HashtagId) == 0)
                            trainingPlan.Hashtags.Add(hashtag);
                    }
                    // Proficiency
                    if (prof != null)
                    {
                        if (trainingPlan.TargetProficiencies.Count(x => x.TrainingProficiencyId == prof.TrainingProficiencyId) == 0)
                            trainingPlan.TargetProficiencies.Add(prof);
                    }
                    // Phases
                    if (phase != null)
                    {
                        if (trainingPlan.TargetPhases.Count(x => x.TrainingPhaseId == phase.TrainingPhaseId) == 0)
                            trainingPlan.TargetPhases.Add(phase);
                    }
                    // Focused Muscles
                    if (focus != null)
                    {
                        if (trainingPlan.MusclesFocuses.Count(x => x.FocusedMuscleId == focus.FocusedMuscleId) == 0)
                            trainingPlan.MusclesFocuses.Add(focus);
                    }

                    return trainingPlan;

                },
                param: new { trainingPlanId },
                splitOn: "HashtagId, TrainingProficiencyId, TrainingPhaseId, FocusedMuscleId");

            return trainingPlan;
        }











        /// <summary>
        /// SQL recursive function for searching all the first-level variant plans an the inherited ones of a specific parent plan.
        /// A training plan is returned if:
        ///    1. it is the parent plan of the query
        ///    2. it is a direct variant of the parent plan
        ///    3. it is directly inherited from the parent plan
        ///    4. it is directly inherited of the 2. plans
        /// The query returns the ID of each record
        /// </summary>
        /// <returns>The SQL as a string</returns>
        private string GetDirectChildPlansOf(int parentTrainingPlan)

        => @"WITH RECURSIVE IsVariantOf(Id, ChildId, ChildTypeId) AS
                (
                    SELECT TP.Id, TPR.ChildPlanId, TPR.ChildPlanTypeId
                    FROM TrainingPlan TP
                    JOIN TrainingPlanRelation TPR
                    ON TP.Id = TPR.ParentPlanId
                                            
                    WHERE TPR.ParentPlanId = @parentTrainingPlan      -- Get First Childs only
                    OR ChildPlanTypeId = 2       -- Or inherited from first childs

                    UNION ALL
                    
                    SELECT TP.Id, IsVariantOf.ChildId, IsVariantOf.ChildTypeId
                    FROM TrainingPlan TP
                    JOIN TrainingPlanRelation TPR
                    ON TP.Id = TPR.ParentPlanId
                    JOIN IsVariantOf
                    ON IsVariantOf.Id = TPR.ChildPlanId
                    
                    WHERE TPR.ParentPlanId = @parentTrainingPlan
                    AND ChildPlanTypeId = 1    -- Get First Childs only
                    
                )
                SELECT ChildId
                FROM IsVariantOf
                WHERE IsVariantOf.Id = @parentTrainingPlan

                UNION ALL

                VALUES(@parentTrainingPlan)";

    }
}
