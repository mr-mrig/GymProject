using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions;

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
        public async Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummariesAsync(uint userId)
        {
            string sql = @"SELECT 
                TP.Id As PlanId, UTP.Id as PlanUserLibraryId, TP.OwnerId as OwnerId, 
                UTP.Name as PlanName, UTP.IsBookmarked,
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
                TProf.Id as ProficiencyId, TProf.Name as Proficiency, 
                Pha.Id as PhaseId, Pha.Name as Phase

                FROM UserTrainingPlan UTP
                JOIN TrainingPlan TP
                ON UTP.TrainingPlanId = TP.Id
                                    
                -- Notes and hashtags
                LEFT JOIN TrainingPlanHashtag TPH
                ON TPH.TrainingPlanId = UTP.Id
                LEFT JOIN TrainingHashtag TH
                ON TPH.HashtagId = TH.Id
                LEFT JOIN TrainingPlanProficiency TPProf
                ON TPProf.TrainingPlanId = UTP.Id
                LEFT JOIN TrainingProficiency TProf
                ON TProf.Id = TPProf.TrainingProficiencyId
                LEFT JOIN TrainingPlanPhase TPPh
                ON TPPh.TrainingPlanId = UTP.Id
                LEFT JOIN TrainingPhase Pha
                ON TPPh.TrainingPhaseId = Pha.Id
                
                WHERE UTP.UserId = @userId
                
                ORDER BY UTP.IsBookmarked DESC";

            Dictionary<uint?, TrainingPlanSummaryDto> plans = new Dictionary<uint?, TrainingPlanSummaryDto>();

            IEnumerable<TrainingPlanSummaryDto> res = await _db.QueryAsync<TrainingPlanSummaryDto, HashtagDto, TrainingProficiencyDto, TrainingPhaseDto, TrainingPlanSummaryDto>(sql,
                map: (plan, hash, prof, phase) =>
                {
                    TrainingPlanSummaryDto trainingPlan;

                    // Training Plans
                    if (!plans.TryGetValue(plan.PlanId, out trainingPlan))
                        plans.Add(plan.PlanId, trainingPlan = plan);

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
                        if (trainingPlan.TargetProficiencies.Count(x => x.ProficiencyId == prof.ProficiencyId) == 0)
                            trainingPlan.TargetProficiencies.Add(prof);
                    }

                    // Phases
                    if (phase != null)
                    {
                        if (trainingPlan.TargetPhases.Count(x => x.PhaseId == phase.PhaseId) == 0)
                            trainingPlan.TargetPhases.Add(phase);
                    }

                    return trainingPlan;

                },
                param: new { userId },
                splitOn: "HashtagId, ProficiencyId, PhaseId");

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
        public async Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutAsync(IList<uint> trainingWeekIds, string workoutName)
        {
            string sql = @"SELECT
                WT.TrainingWeekId as WeekId, WT.Id as WorkoutId,
                WT.Name as WorkoutName, WT.SpecificWeekday as WeekdayId,
                WUT.Id as WuId, WUT.ProgressiveNumber as WuProgressiveNumber, 
                IT1.Id as WuIntensityTechniqueId, IT1.Abbreviation as WuIntensityTechniqueAbbreviation,
                WUTN.Id as NoteId, WUTN.Body as Note,
                E.Id as ExcerciseId, E.Name as ExcerciseName, MG.Id as PrimaryMuscleId,
                WST.Id as WsId, WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence AS LiftingTempo, WST.Effort,
                ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
                IT.Id as TechniqueId, IT.Abbreviation as TechniqueAbbreviation

                FROM WorkoutTemplate WT
                LEFT JOIN WorkUnitTemplate WUT
                ON WT.Id = WUT.WorkoutTemplateId
                LEFT JOIN WorkUnitTemplateNote WUTN
                ON WUTN.Id = WUT.WorkUnitNoteId
                LEFT JOIN Excercise E
                ON E.Id = WUT.ExcerciseId
                LEFT JOIN MuscleGroup MG
                ON E.PrimaryMuscleId = MG.Id
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


            Dictionary<uint, WorkoutFullPlanDto> workouts = new Dictionary<uint, WorkoutFullPlanDto>();
            Dictionary<uint, WorkUnitDto> workUnits = new Dictionary<uint, WorkUnitDto>();

            IEnumerable<WorkoutFullPlanDto> res = await _db.QueryAsync<WorkoutFullPlanDto, WorkUnitDto, WorkingSetTemplateDto, IntensityTechniqueDto, WorkoutFullPlanDto>(sql,
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
                        if (workout.WorkUnits.Count(x => x.WuId == wu.WuId) == 0)
                            workout.WorkUnits.Add(wu);

                        // Work Units
                        if (!workUnits.TryGetValue(wu.WuId, out workUnit))
                            workUnits.Add(wu.WuId, workUnit = wu);

                        if (workUnit.WorkingSets == null)
                            workUnit.WorkingSets = new List<WorkingSetTemplateDto>();

                        if (ws != null)
                        {
                            // WorkingSets
                            if (workUnit.WorkingSets.Count(x => x.WsId == ws.WsId) == 0)
                                workUnit.WorkingSets.Add(ws);

                            // WS Intensity Techniques - Allow duplicates: the same IT can be linked to different WSs
                            WorkingSetTemplateDto currentWs = workUnit.WorkingSets.Single(x => x.WsId == ws.WsId);

                            if (currentWs?.IntensityTechniques == null)
                                currentWs.IntensityTechniques = new List<IntensityTechniqueDto>();

                            if (it != null)
                                currentWs.IntensityTechniques.Add(it);
                        }
                    }
                    return workout;
                },
                param: new { trainingWeekIds, workoutName },
                splitOn: "WuId, WsId, TechniqueId");

            // HERE OR CLIENT-SIDE?
            // 1. Insert missing Weeks

            // 2. Sort according to the Training Week - This breaks if the input Training Weeks are not sorted Progressive Number-wise
            IEnumerable<WorkoutFullPlanDto> result = workouts.Values
                .OrderBy(x => trainingWeekIds.IndexOf(x.WeekId));

            return result;
        }



        /// <summary>
        /// Get the plan over the training weeks for the specified workout day, assuming the Workout IDs are already known, which should be the case.
        /// In order to query for a specific training plan, the caller should already have all its Workout IDs fetched, IE: <cref="GetWorkoutsWeeklyScheduleAsync"> should hav been called previously.
        /// Furthermore, notice that the Workouts IDs should be ordered according to the Week progressive number, otherwise further post-processing is needed.
        /// </summary>
        /// <param name="workoutsIds">The IDs of the Workouts to be queried</param>
        /// <returns>The WorkoutFullPlanDto list</returns>
        public async Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutAsync(IEnumerable<uint> workoutsIds)
        {
                string sql = @"SELECT
                    WT.TrainingWeekId as WeekId, WT.Id as WorkoutId,
                    WT.Name as WorkoutName, WT.SpecificWeekday as WeekdayId,
                    WUT.Id as WuId, WUT.ProgressiveNumber as WuProgressiveNumber, 
                    IT1.Id as WuIntensityTechniqueId, IT1.Abbreviation as WuIntensityTechniqueAbbreviation,
                    WUTN.Id as NoteId, WUTN.Body as Note,
                    E.Id as ExcerciseId, E.Name as ExcerciseName, MG.Id as PrimaryMuscleId,
                    WST.Id as WsId, WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence AS LiftingTempo, WST.Effort,
                    ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
                    IT.Id as TechniqueId, IT.Abbreviation as TechniqueAbbreviation

                    FROM WorkoutTemplate WT
                    LEFT JOIN WorkUnitTemplate WUT
                    ON WT.Id = WUT.WorkoutTemplateId
                    LEFT JOIN WorkUnitTemplateNote WUTN
                    ON WUTN.Id = WUT.WorkUnitNoteId
                    LEFT JOIN Excercise E
                    ON E.Id = WUT.ExcerciseId
                    LEFT JOIN MuscleGroup MG
                    ON E.PrimaryMuscleId = MG.Id
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

                    WHERE WT.Id IN @workoutsIds

                    ORDER BY WT.TrainingWeekId, WT.ProgressiveNumber, WUT.ProgressiveNumber, WST.ProgressiveNumber";


            Dictionary<uint, WorkoutFullPlanDto> workouts = new Dictionary<uint, WorkoutFullPlanDto>();
            Dictionary<uint, WorkUnitDto> workUnits = new Dictionary<uint, WorkUnitDto>();

            IEnumerable<WorkoutFullPlanDto> res = await _db.QueryAsync<WorkoutFullPlanDto, WorkUnitDto, WorkingSetTemplateDto, IntensityTechniqueDto, WorkoutFullPlanDto>(sql,
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
                        if (workout.WorkUnits.Count(x => x.WuId == wu.WuId) == 0)
                            workout.WorkUnits.Add(wu);

                        // Work Units
                        if (!workUnits.TryGetValue(wu.WuId, out workUnit))
                            workUnits.Add(wu.WuId, workUnit = wu);

                        if (workUnit.WorkingSets == null)
                            workUnit.WorkingSets = new List<WorkingSetTemplateDto>();

                        if (ws != null)
                        {
                            // WorkingSets
                            if (workUnit.WorkingSets.Count(x => x.WsId == ws.WsId) == 0)
                                workUnit.WorkingSets.Add(ws);

                            // WS Intensity Techniques - Allow duplicates: the same IT can be linked to different WSs
                            WorkingSetTemplateDto currentWs = workUnit.WorkingSets.Single(x => x.WsId == ws.WsId);

                            if (currentWs?.IntensityTechniques == null)
                                currentWs.IntensityTechniques = new List<IntensityTechniqueDto>();

                            if (it != null)
                                currentWs.IntensityTechniques.Add(it);
                        }
                    }

                    return workout;

                },
                param: new { workoutsIds },
                splitOn: "WuId, WsId, TechniqueId");

            // HERE OR CLIENT-SIDE?
            // 1. Insert missing Weeks

            //// 2. Sort according to the Training Week - This breaks if the input Training Weeks are not sorted Progressive Number-wise
            //IEnumerable<WorkoutFullPlanDto> result = workouts.Values
            //    .OrderBy(x => ids.IndexOf(x.TrainingWeekId));

            //return result;

            return workouts.Values;
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
        /// <param name="userId">The ID of the user who is querying - This is needed in order to skip a JOIN</param>
        /// <returns>The FullFeedbackDetailDto list</returns>
        public async Task<IEnumerable<FullFeedbackDetailDto>> GetFullFeedbacksDetailsAsync(uint trainingPlanId, uint userId)
        {
            string sql = $@"SELECT 
                UTP.TrainingPlanId as PlanId, UTP.Id as PlanUserLibraryId, UTP.Name as PlanName, U.Id as UserId, U.Username as UserName,
                TS.Id as ScheduleId, TS.StartDate as StartDate, TS.EndDate as EndDate,
                Ph.Id as PhaseId, Ph.Name as Phase,
                TProf.Id as ProficiencyId, TProf.Name as Proficiency,
                TSF.Id as FeedbackId, TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating, TSF.UserId as FeedbackOwnerId

                FROM UserTrainingPlan UTP

                -- Proficiency
                LEFT JOIN TrainingSchedule TS
                ON TS.TrainingPlanId = UTP.TrainingPlanId
                AND TS.AthleteId = UTP.UserId
                LEFT JOIN UserTrainingProficiency UP
                ON UP.UserId = UTP.UserId
                --AND UP.OwnerId = @userId
                AND UP.StartDate <= TS.Startdate
                AND COALESCE(UP.EndDate, TS.Startdate + 1) >= TS.Startdate
                LEFT JOIN TrainingProficiency TProf
                ON TProf.Id = UP.TrainingProficiencyId

                -- TrainingPhase
                LEFT JOIN UserTrainingPhase UPh
                ON UPh.UserId = UTP.UserId              -- Denormalized
                AND UPh.StartDate <= TS.StartDate
                AND COALESCE(UPh.EndDate, TS.Startdate + 1) >= TS.Startdate
                -- AND UPh.OwnerId = @userId
                LEFT JOIN TrainingPhase Ph
                ON UPh.TrainingPhaseId = Ph.Id

                -- Feedback
                LEFT JOIN TrainingScheduleFeedback TSF
                ON TSF.TrainingScheduleId = TS.Id

                -- User
                LEFT JOIN User U
                ON UTP.UserId = U.Id

                WHERE UTP.TrainingPlanId IN (  { GetDirectChildPlansOf("trainingPlanId") } )";

            Dictionary<uint?, FullFeedbackDetailDto> plans = new Dictionary<uint?, FullFeedbackDetailDto>();
            Dictionary<uint?, TrainingScheduleDto> schedules = new Dictionary<uint?, TrainingScheduleDto>();

            IEnumerable<FullFeedbackDetailDto> res = await _db.QueryAsync<FullFeedbackDetailDto, TrainingScheduleDto, FeedbackDto, FullFeedbackDetailDto>(sql,
                map: (plan, sched, fbk) =>
                {
                    FullFeedbackDetailDto trainingPlan;
                    TrainingScheduleDto schedule;

                    // Training Plans
                    if (!plans.TryGetValue(plan.PlanUserLibraryId, out trainingPlan))
                        plans.Add(plan.PlanUserLibraryId, trainingPlan = plan);

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
        public async Task<IEnumerable<TrainingPlanDetailDto>> GetTrainingPlanDetailsAsync(uint trainingPlanId)
        {
            string sql = @"SELECT
                UTP.Id as PlanUserLibraryId, UTP.ParentPlanId as ParentId, UTP.TrainingPlanNoteId as NoteId, TPN.Body as Note,
                CASE
                    WHEN UTP.ParentPlanId is not null THEN 1    -- Variant
                    WHEN TP.OwnerId<> UTP.UserId THEN 2         -- Inherited
                END as RelationTypeId,
                TPUParent.Name as ParentName, TP.OwnerId as PlanOwnerId,
                MG.Id as PrimaryMuscleId, MG.Abbreviation as MuscleAbbreviation

                FROM UserTrainingPlan UTP
                JOIN TrainingPlan TP
                ON TP.Id = UTP.TrainingPlanId
                LEFT JOIN UserTrainingPlan TPUParent
                ON UTP.ParentPlanId = TPUParent.Id

                -- TP missing data
                LEFT JOIN TrainingPlanNote TPN
                ON UTP.TrainingPlanNoteId = TPN.Id
                LEFT JOIN TrainingPlanMuscleFocus MF
                ON UTP.Id = MF.TrainingPlanId
                LEFT JOIN MuscleGroup MG
                ON MG.Id = MF.MuscleGroupId

                WHERE UTP.Id = @trainingPlanId";

            Dictionary<uint?, TrainingPlanDetailDto> plans = new Dictionary<uint?, TrainingPlanDetailDto>();

            IEnumerable<TrainingPlanDetailDto> res = await _db.QueryAsync<TrainingPlanDetailDto, MuscleFocusDto, TrainingPlanDetailDto>(sql,
                map: (plan, focus) =>
                {
                    TrainingPlanDetailDto trainingPlan;

                    // Training Plans
                    if (!plans.TryGetValue(plan.PlanUserLibraryId, out trainingPlan))
                        plans.Add(plan.PlanUserLibraryId, trainingPlan = plan);

                    if (trainingPlan.MusclesFocuses == null)
                        trainingPlan.MusclesFocuses = new List<MuscleFocusDto>();

                    // Focused Muscles
                    if (focus != null)
                    {
                        if (trainingPlan.MusclesFocuses.Count(x => x.MuscleId == focus.MuscleId) == 0)
                            trainingPlan.MusclesFocuses.Add(focus);
                    }

                    return trainingPlan;

                },
                param: new { trainingPlanId },
                splitOn: "PrimaryMuscleId");

            return plans.Values;
        }


        /// <summary>
        /// Get all the details of the selected Training Plan, when no pre-fetch data is available, namely no  <see cref="TrainingPlanSummaryDto"/> in memory.
        /// No data about Workouts and Feedbaks/Variants are loaded, please refer to:
        ///
        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
        ///
        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
        ///
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        public async Task<TrainingPlanFullDetailDto> GetTrainingPlanFullDetailsAsync(uint trainingPlanId)
        {
            string sql = @"SELECT 
            UTP.Id as PlanUserLibraryId, UTP.ParentPlanId as ParentId, UTP.TrainingPlanNoteId, TPN.Body as TrainingPlanNote, 
            TP.OwnerId, UTP.IsBookmarked,
            CASE 
                WHEN UTP.ParentPlanId is not null THEN 1    -- Variant
                WHEN TP.OwnerId <> UTP.UserId THEN 2        -- Inherited
            END as PlanTypeId,
            UTP.Name as PlanName, TPUParent.Name as ParentName,
            --Number of training weeks
            (
                SELECT Count(1)
                FROM TrainingWeek
                WHERE TrainingPlanId = TP.Id
            ) As TrainingWeeksCounter,
            TH.Id As HashtagId, TH.Body As Hashtag, 
            TProf.Id as ProficiencyId, TProf.Name as Proficiency, 
            Pha.Id as PhaseId, Pha.Name as Phase,
            MG.Id as PrimaryMuscleId, MG.Abbreviation as MuscleAbbreviation

            FROM UserTrainingPlan UTP
            JOIN TrainingPlan TP
            ON TP.Id = UTP.TrainingPlanId
            LEFT JOIN UserTrainingPlan TPUParent
            ON UTP.ParentPlanId = TPUParent.Id

            -- TP missing data
            LEFT JOIN TrainingPlanNote TPN
            ON UTP.TrainingPlanNoteId = TPN.Id
            LEFT JOIN TrainingPlanMuscleFocus MF
            ON UTP.Id = MF.TrainingPlanId
            LEFT JOIN MuscleGroup MG
            ON MG.Id = MF.MuscleGroupId

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

            WHERE UTP.Id = @trainingPlanId";


            Dictionary<uint?, TrainingPlanFullDetailDto> plans = new Dictionary<uint?, TrainingPlanFullDetailDto>();

            IEnumerable<TrainingPlanFullDetailDto> res = await _db.QueryAsync<TrainingPlanFullDetailDto, HashtagDto, TrainingProficiencyDto, TrainingPhaseDto, MuscleFocusDto, TrainingPlanFullDetailDto>(sql,
                map: (plan, hashtag, prof, phase, focus) =>
                {
                    TrainingPlanFullDetailDto trainingPlan;

                    // Training Plans
                    if (!plans.TryGetValue(plan.TrainingPlanId, out trainingPlan))
                        plans.Add(plan.TrainingPlanId, trainingPlan = plan);

                    if (trainingPlan.Hashtags == null)
                        trainingPlan.Hashtags = new List<HashtagDto>();

                    if (trainingPlan.TargetProficiencies == null)
                        trainingPlan.TargetProficiencies = new List<TrainingProficiencyDto>();

                    if (trainingPlan.TargetPhases == null)
                        trainingPlan.TargetPhases = new List<TrainingPhaseDto>();

                    if (trainingPlan.MusclesFocuses == null)
                        trainingPlan.MusclesFocuses = new List<MuscleFocusDto>();

                    // Hashtags
                    if (hashtag != null)
                    {
                        if (trainingPlan.Hashtags.Count(x => x.HashtagId == hashtag.HashtagId) == 0)
                            trainingPlan.Hashtags.Add(hashtag);
                    }
                    // Proficiency
                    if (prof != null)
                    {
                        if (trainingPlan.TargetProficiencies.Count(x => x.ProficiencyId == prof.ProficiencyId) == 0)
                            trainingPlan.TargetProficiencies.Add(prof);
                    }
                    // Phases
                    if (phase != null)
                    {
                        if (trainingPlan.TargetPhases.Count(x => x.PhaseId == phase.PhaseId) == 0)
                            trainingPlan.TargetPhases.Add(phase);
                    }
                    // Focused Muscles
                    if (focus != null)
                    {
                        if (trainingPlan.MusclesFocuses.Count(x => x.MuscleId == focus.MuscleId) == 0)
                            trainingPlan.MusclesFocuses.Add(focus);
                    }

                    return trainingPlan;
                },
                param: new { trainingPlanId },
                splitOn: "HashtagId, ProficiencyId, PhaseId, PrimaryMuscleId");

            return res.FirstOrDefault();
        }


        /// <summary>
        /// Get the Workouts schedule over the planned Training Weeks.
        /// This query is redundant if all the <cref="GetTraininPlanPlannedWorkoutAsync"> or the single <cref="GetTraininPlanAllPlannedWorkoutsAsync"> query has been executed for each Workout Day
        ///
        /// </summary>
        /// <param name="trainingWeekIds">The IDs of the planned Training Weeks</param>
        /// <returns>The Training Plan Details DTO</returns>
        public async Task<TrainingPlanWorkoutsScheduleDto> GetWorkoutsWeeklyScheduleAsync(IEnumerable<uint> trainingWeekIds)
        {
            string sql = @"SELECT 
                TW.Id as WeekId,  TW.ProgressiveNumber as WeekProgressiveNumber, TW.TrainingWeekTypeId,
                WOT.Id as WorkoutId, WOT.ProgressiveNumber as WorkoutProgressiveNumber, WOT.Name as WorkoutName, WOT.SpecificWeekday

                FROM TrainingWeek TW
                LEFT JOIN WorkoutTemplate WOT
                ON TW.Id = WOT.TrainingWeekId

                WHERE TW.Id IN @trainingWeekIds
                
                ORDER BY TW.ProgressiveNumber, WOT.ProgressiveNumber";


            Dictionary<uint?, TrainingPlanWorkoutsScheduleDto> fullSchedule = new Dictionary<uint?, TrainingPlanWorkoutsScheduleDto>();

            IEnumerable<TrainingPlanWorkoutsScheduleDto> res = await _db.QueryAsync<TrainingPlanWorkoutsScheduleDto, WorkoutScheduleInfoDto, TrainingPlanWorkoutsScheduleDto>(sql,
                map: (sched, wo) =>
                {
                    TrainingPlanWorkoutsScheduleDto weekSchedule;

                    // Training Plans
                    if (!fullSchedule.TryGetValue(sched.WeekId, out weekSchedule))
                        fullSchedule.Add(sched.WeekId, weekSchedule = sched);

                    if (weekSchedule.Workouts == null)
                        weekSchedule.Workouts = new List<WorkoutScheduleInfoDto>();

                    // Workout schedule
                    if (wo != null)
                    {
                        if (weekSchedule.Workouts.Count(x => x.WorkoutId == wo.WorkoutId) == 0)
                            weekSchedule.Workouts.Add(wo);
                    }

                    return weekSchedule;

                },
                splitOn: "WorkoutId");

            return res.FirstOrDefault();
        }


        /// <summary>
        /// Get the plan over the training weeks for all the Workouts belonging to the specified Training Weeks. No info about the Training Weeks is fetched.
        /// Please notice that in order to use this query to search for the Training Plan Workouts, the Training Weeks should have been fetched before - however if additional week info is required a more specific query is suggested.
        /// <param name="trainingWeeksIds">The IDs of the Training Weeks to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        public async Task<IEnumerable<WorkoutFullPlanDto>> GetTrainingPlanAllPlannedWorkoutsAsync(IEnumerable<uint> trainingWeeksIds)
        {
            string sql = @"SELECT 
                WT.TrainingWeekId as WeekId, WT.Id as WorkoutId,
                WT.Name as WorkoutName, WT.SpecificWeekday AS SpecificWeekdayId,
                WUT.Id as WuId, WUT.ProgressiveNumber as WuProgressiveNumber, 
                IT1.Id as WuIntensityTechniqueId, IT1.Abbreviation as WuIntensityTechniqueAbbreviation,
                WUTN.Id as NoteId, WUTN.Body as Note,
                E.Id as ExcerciseId, E.Name as ExcerciseName, MG.Id as PrimaryMuscleId,
                WST.Id as WsId, WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence AS LiftingTempo, WST.Effort,
                ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
                IT.Id as TechniqueId, IT.Abbreviation as TechniqueAbbreviation

                FROM WorkoutTemplate WT
                LEFT JOIN WorkUnitTemplate WUT
                ON WT.Id = WUT.WorkoutTemplateId
                LEFT JOIN WorkUnitTemplateNote WUTN
                ON WUTN.Id = WUT.WorkUnitNoteId
                LEFT JOIN Excercise E
                ON E.Id = WUT.ExcerciseId
                LEFT JOIN MuscleGroup MG
                ON E.PrimaryMuscleId = MG.Id
                LEFT JOIN WorkingSetTemplate WST
                ON WUT.Id = WST.WorkUnitTemplateId
                LEFT JOIN TrainingEffortType ET
                ON WST.Effort_EffortTypeId = ET.Id
                LEFT JOIN IntensityTechnique IT1
                ON IT1.Id = WUT.LinkingIntensityTechniqueId
                LEFT JOIN IntensityTechnique IT2
                ON WUT.LinkingIntensityTechniqueId = IT2.Id

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

                WHERE WT.TrainingWeekId IN @trainingWeeksId

                ORDER BY WT.TrainingWeekId, WT.ProgressiveNumber, WUT.ProgressiveNumber, WST.ProgressiveNumber";

            Dictionary<uint?, WorkoutFullPlanDto> workouts = new Dictionary<uint?, WorkoutFullPlanDto>();
            Dictionary<uint?, WorkUnitDto> workUnits = new Dictionary<uint?, WorkUnitDto>();

            IEnumerable<WorkoutFullPlanDto> res = await _db.QueryAsync<WorkoutFullPlanDto, WorkUnitDto, WorkingSetTemplateDto, IntensityTechniqueDto, WorkoutFullPlanDto>(sql,
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
                        if (workout.WorkUnits.Count(x => x.WuId == wu.WuId) == 0)
                            workout.WorkUnits.Add(wu);

                        // Work Units
                        if (!workUnits.TryGetValue(wu.WuId, out workUnit))
                            workUnits.Add(wu.WuId, workUnit = wu);

                        if (workUnit.WorkingSets == null)
                            workUnit.WorkingSets = new List<WorkingSetTemplateDto>();

                        if (ws != null)
                        {
                            // WorkingSets
                            if (workUnit.WorkingSets.Count(x => x.WsId == ws.WsId) == 0)
                                workUnit.WorkingSets.Add(ws);

                            // WS Intensity Techniques - Allow duplicates: the same IT can be linked to different WSs
                            WorkingSetTemplateDto currentWs = workUnit.WorkingSets.Single(x => x.WsId == ws.WsId);

                            if (currentWs?.IntensityTechniques == null)
                                currentWs.IntensityTechniques = new List<IntensityTechniqueDto>();

                            if (it != null)
                                currentWs.IntensityTechniques.Add(it);
                        }
                    }

                    return workout;

                },
                param: new { trainingWeeksIds },
                splitOn: "WuId, WsId, TechniqueId");

            return workouts.Values;
        }



        /// <summary>
        /// Get the Workouts scheduled for the specified Training Week fetching all the WOs data, not only the main info.
        /// This query might be useful when loading the schedule for the current Training Week. Three cases:
        ///   - SessionStarted, SessionEnded are null -> Workout still to be done
        ///   - SessionStarted, SessionEnded not null -> Workout completed
        ///   - SessionStarted not null, SessionEnded is null -> Workout not completed
        /// <param name="trainingWeekId">The IDs of the Training Week to be searched</param>
        /// <returns>The Training Plan Details DTO</returns>
        public async Task<TrainingWeekWorkoutsDetailsDto> GetTrainingWeekPlannedWorkoutsDetailsAsync(uint trainingWeekId)
        {
            string sql = $@"SELECT
                    WOT.TrainingWeekId, WOT.Id as WorkoutId, WOT.ProgressiveNumber as WorkoutProgressiveNumber, WOT.Name as WorkoutName, WOT.SpecificWeekday as WeekdayId,
                    WOS.Id as SessionId, {GetSqlSafeDate("WOS.StartTime")} as SessionStarted, {GetSqlSafeDate("WOS.EndTime")} as SessionEnded,
                    WUT.Id as WuId, WUT.ProgressiveNumber as WuProgressiveNumber, WUT.LinkingIntensityTechniqueId as WuIntensityTechniqueId, IT2.Abbreviation as WuIntensityTechniqueAbbreviation,
                    WUT.WorkUnitNoteId as NoteId, WTN.Body as Note, 
                    E.Id as ExcerciseId, E.Name as ExcerciseName, E.PrimaryMuscleId AS PrimaryMuscleId,
                    WS.Id as WsId, WS.ProgressiveNumber as WsProgressiveNumber, WS.TargetRepetitions as Repetitions, WS.Rest, 
                    WS.Cadence as LiftingTempo, WS.Effort as Effort, 
                    WS.Effort_EffortTypeId as EffortTypeId, ET.Abbreviation as EffortAbbreviation,
                    IT.Id as TechniqueId, IT.Abbreviation as TechniqueAbbreviation

                    FROM WorkoutTemplate WOT
                    LEFT JOIN WorkoutSession WOS
                    ON WOS.WorkoutTemplateId = WOT.Id
                    LEFT JOIN WorkUnitTemplate WUT
                    ON WOT.Id = WUT.WorkoutTemplateId
                    LEFT JOIN WorkUnitTemplateNote WTN
                    ON WUT.WorkUnitNoteId = WTN.Id
                    LEFT JOIN Excercise E
                    ON E.Id = WUT.ExcerciseId
                    LEFT JOIN WorkingSetTemplate WS
                    ON WUT.Id = WS.WorkUnitTemplateId
                    LEFT JOIN TrainingEffortType ET
                    ON WS.Effort_EffortTypeId = ET.Id
                    LEFT JOIN WorkingSetIntensityTechnique WSIT
                    ON WSIT.WorkingSetId = WS.Id
                    LEFT JOIN IntensityTechnique IT
                    ON WSIT.IntensityTechniqueId = IT.Id
                    LEFT JOIN IntensityTechnique IT2
                    ON WUT.LinkingIntensityTechniqueId = IT2.Id

                    WHERE WOT.TrainingWeekId = @trainingWeekId

                    ORDER BY WOT.ProgressiveNumber, WUT.ProgressiveNumber, WS.ProgressiveNumber";

            Dictionary<uint?, TrainingWeekWorkoutsDetailsDto> workouts = new Dictionary<uint?, TrainingWeekWorkoutsDetailsDto>();
            Dictionary<uint?, WorkUnitDto> workUnits = new Dictionary<uint?, WorkUnitDto>();

            IEnumerable<TrainingWeekWorkoutsDetailsDto> res = await _db.QueryAsync<TrainingWeekWorkoutsDetailsDto, WorkUnitDto, WorkingSetTemplateDto, IntensityTechniqueDto, TrainingWeekWorkoutsDetailsDto>(sql,
                map: (wo, wu, ws, it) =>
                {
                    TrainingWeekWorkoutsDetailsDto workout;
                    WorkUnitDto workUnit;

                    // Workouts
                    if (!workouts.TryGetValue(wo.WorkoutId, out workout))
                        workouts.Add(wo.WorkoutId, workout = wo);

                    if (workout.WorkUnits == null)
                        workout.WorkUnits = new List<WorkUnitDto>();

                    if (wu != null)
                    {
                        if (workout.WorkUnits.Count(x => x.WuId == wu.WuId) == 0)
                            workout.WorkUnits.Add(wu);

                        // Work Units
                        if (!workUnits.TryGetValue(wu.WuId, out workUnit))
                            workUnits.Add(wu.WuId, workUnit = wu);

                        if (workUnit.WorkingSets == null)
                            workUnit.WorkingSets = new List<WorkingSetTemplateDto>();

                        if (ws != null)
                        {
                            // WorkingSets
                            if (workUnit.WorkingSets.Count(x => x.WsId == ws.WsId) == 0)
                                workUnit.WorkingSets.Add(ws);

                            // WS Intensity Techniques - Allow duplicates: the same IT can be linked to different WSs
                            WorkingSetTemplateDto currentWs = workUnit.WorkingSets.Single(x => x.WsId == ws.WsId);

                            if (currentWs?.IntensityTechniques == null)
                                currentWs.IntensityTechniques = new List<IntensityTechniqueDto>();

                            if (it != null)
                                currentWs.IntensityTechniques.Add(it);
                        }
                    }

                    return workout;

                },
                param: new { trainingWeekId },
                splitOn: "WuId, WsId, TechniqueId");

            return res.FirstOrDefault();
        }





        /// <summary>
        /// Get the details for the specified Workout Session - IE: the performed excercises
        /// <param name="workoutSessionId">The ID of the Workout Session to be searched</param>
        /// <returns>The WorkUnitSessionDetailsDto</returns>
        public async Task<WorkUnitSessionDetailsDto> GetWorkoutSessionDetailsAsync(uint workoutSessionId)
        {
            string sql = $@"SELECT 
            WU.Id as WorkUnitId, WU.PRogressiveNumber as WorkUnitProgressiveNumber, WU.Rating, 
            WU.ExcerciseId, E.Name as Excercise,
            WS.Id as WsId, WS.ProgressiveNumber as WsProgressiveNumber, WS.Repetitions, Ws.WeightKg as Weight

            FROM WorkoutSession WOS
            LEFT JOIN WorkUnit WU
            ON WU.WorkoutSessionId = WOS.Id
            LEFT JOIN Excercise E
            ON WU.ExcerciseId = E.Id
            LEFT JOIN WorkingSet WS
            ON WS.WorkUnitId = WU.Id

            WHERE WOS.Id = @workoutSessionId";

            Dictionary<uint?, WorkUnitSessionDetailsDto> workUnits = new Dictionary<uint?, WorkUnitSessionDetailsDto>();

            IEnumerable<WorkUnitSessionDetailsDto> res = await _db.QueryAsync<WorkUnitSessionDetailsDto, WorkingSetDto, WorkUnitSessionDetailsDto>(sql,
                map: (wu, ws) =>
                {
                    WorkUnitSessionDetailsDto wunit;

                    // Workouts
                    if (!workUnits.TryGetValue(wu.WorkUnitId, out wunit))
                        workUnits.Add(wu.WorkUnitId, wunit = wu);

                    if (wunit.WorkingSets == null)
                        wunit.WorkingSets = new List<WorkingSetDto>();

                    if (ws != null)
                        wunit.WorkingSets.Add(ws);

                    return wunit;
                },
                param: new { workoutSessionId },
                splitOn: "WsId");

            return res.FirstOrDefault();
        }




        /// <summary>
        /// Get the Working Sets of the specified Excercise performed by the specified user, including additional WorkUnit info.
        /// IMPORTANT: This query does not work for on-the-fly workouts. Two solutions are available:
        ///
        /// 1. Go through the Post table looking for on-the-fly WOs
        /// 2. Create a dummy TrainingPlan for collecting all on-the-fly WOs
        /// 3. Add them to the Schedule
        /// <param name="excerciseId">The ID of the Excercise to be searched</param>
        /// <param name="ownerId">The ID of the user to be searched</param>
        /// <param name="offset">The offset of the results to be fetched -> unlimited if value < 0 or left default</param>
        /// <param name="limit">The limit of the results to be fetched -> unlimited if value <= 0 or left default</param>
        /// <returns>The ExcerciseHistoryDto</returns>
        //public async Task<ExcerciseHistoryDto> GetExcerciseHistoryAsync(uint ownerId, uint excerciseId, int offset = -1, int limit = -1)
        //{
        //    string limitSqlStr = "";

        //    if (offset >= 0 && limit > 0)
        //    {
        //        limitSqlStr = GetSqlSafeLimit(offset, limit);
        //    }

        //    string sql = $@"SELECT
        //        WU.Id as WorkUnitId, WU.Rating, 
        //        Date(WOS.StartTime, 'unixepoch') as LogDate,
        //        WS.Id as WsId, WS.ProgressiveNumber as WsProgressiveNumber, WS.Repetitions as Repetitions, WS.WeightKg as Weight,
        //        WS.NoteId as NoteId, WSN.Body as Note,
        //        WST.Effort as Effort, WST.Effort_EffortTypeId as EffortTypeId, ET.Abbreviation as EffortAbbreviation,
        //        IT.Id as IntensityTechniqueId, IT.Abbreviation as IntensityTechniqueAbbreviation

        //        FROM TrainingPlan TP
        //        JOIN TrainingWeek TW
        //        ON TW.TrainingPlanId = TP.Id
        //        JOIN WorkoutTemplate WOT
        //        ON WOT.TrainingWeekId = TW.Id
        //        JOIN WorkoutSession WOS
        //        ON WOS.WorkoutTemplateId = WOT.Id
        //        JOIN WorkUnit WU
        //        ON WU.WorkoutSessionId = WOS.Id
        //        JOIN WorkingSet WS
        //        ON WS.WorkUnitId = WU.Id
        //        LEFT JOIN WorkingSetNote WSN
        //        ON WS.NoteId = WSN.Id

        //        -- Get template data - Using LEFT JOIN to support on-the-fly excercises
        //        LEFT JOIN WorkUnitTemplate WUT
        //        ON WUT.WorkoutTemplateId = WOT.Id
        //        AND WUT.ProgressiveNumber = WU.ProgressiveNumber
        //        LEFT JOIN WorkingSetTemplate WST
        //        ON WST.WorkUnitTemplateId = WUT.Id 
        //        AND WST.ProgressiveNumber = WS.ProgressiveNumber
        //        LEFT JOIN TrainingEffortType ET
        //        ON WST.Effort_EffortTypeId = ET.Id
        //        LEFT JOIN WorkingSetIntensityTechnique WSIT
        //        ON WSIT.WorkingSetId = WST.Id
        //        LEFT JOIN IntensityTechnique IT
        //        ON WSIT.IntensityTechniqueId = IT.Id

        //        WHERE TP.OwnerId = @ownerId
        //        AND WU.ExcerciseId = @excerciseId

        //        ORDER BY StartTime DESC, WU.ProgressiveNumber DESC
                
        //        { limitSqlStr }";

        //    Dictionary<uint?, WorkUnitHistoryEntryDto> workUnits = new Dictionary<uint?, WorkUnitHistoryEntryDto>();
        //    Dictionary<uint?, WorkingSetHistoryEntryDto> workingSets = new Dictionary<uint?, WorkingSetHistoryEntryDto>();

        //    IEnumerable<WorkUnitHistoryEntryDto> res = await _db.QueryAsync<WorkUnitHistoryEntryDto, WorkingSetHistoryEntryDto, IntensityTechniqueDto, WorkUnitHistoryEntryDto>(sql,
        //        map: (wu, ws, it) =>
        //        {
        //            WorkUnitHistoryEntryDto wunit;
        //            WorkingSetHistoryEntryDto wset;

        //            // Work Units
        //            if (!workUnits.TryGetValue(wu.WorkUnitId, out wunit))
        //                workUnits.Add(wu.WorkUnitId, wunit = wu);

        //            if (wunit.WorkingSets == null)
        //                wunit.WorkingSets = new List<WorkingSetHistoryEntryDto>();

        //            if (ws != null)
        //            {
        //                if (wunit.WorkingSets.Count(x => x.WsId == ws.WsId) == 0)
        //                    wunit.WorkingSets.Add(ws);

        //                // Working Sets
        //                if (!workingSets.TryGetValue(ws.WsId, out wset))
        //                    workingSets.Add(ws.WsId, wset = ws);

        //                if (wset.IntensityTechniques == null)
        //                    wset.IntensityTechniques = new List<IntensityTechniqueDto>();

        //                if (it != null)
        //                    wset.IntensityTechniques.Add(it);
        //            }


        //            return wunit;
        //        },
        //        param: new { ownerId, excerciseId, offset, limit }
        //        splitOn: "WsId, IntensityTechniqueId");

        //    return workUnits.Values;
        //}


        ///// <summary>
        ///// Get the estimated 1RM of the specified Excercise performed by the specified user, including additional WorkUnit info.
        ///// IMPORTANT: This query does not work for on-the-fly workouts. Two solutions are available:
        /////
        ///// 1. Go through the Post table looking for on-the-fly WOs
        ///// 2. Create a dummy TrainingPlan for collecting all on-the-fly WOs
        ///// <param name="excerciseId">The ID of the Excercise to be searched</param>
        ///// <param name="ownerId">The ID of the user to be searched</param>
        ///// <param name="offset">The offset of the results to be fetched -> unlimited if value < 0 or left default</param>
        ///// <param name="limit">The limit of the results to be fetched -> unlimited if value <= 0 or left default</param>
        ///// <returns>The ExcerciseHistoryDto</returns>
        //public async Task<ExcerciseHistoryDto> GetExcerciseRecordsHistory(uint ownerId, uint excerciseId, int offset = -1, int limit = -1)
        //{

        //}


        //public async Task<ExcerciseHistoryDto> GetTrainingPlanScheduledWorkouts(uint scheduleId)
        //{

        //}


        //public async Task<ExcerciseHistoryDto> GetTrainingPlanSchedules(uint trainingPlanId)
        //{

        //}


        //public async Task<ExcerciseHistoryDto> GetWorkoutsHistory(DateTime from, DateTime to)
        //{

        //}









        public string GetSqlSafeDate(string dateFieldName)

            => $@"Date({dateFieldName}, 'unixepoch')";




        public string GetSqlSafeLimit(int offset, int limit)

            => $@"LIMIT {offset.ToString()} {limit.ToString()})";



        /// <summary>
        /// Fetches the first-level variant plans of a specific parent plan, plus the training plan itself.
        /// A training plan is returned if:
        ///    1. it is the parent plan of the query
        ///    2. it is a direct variant of the parent plan
        ///    3. it is directly inherited from the parent plan
        ///    4. it is directly inherited of the 2. plans
        /// The query returns the ID of each record
        /// </summary>
        /// <param name="parentPlanIdSqlParameter">The name of the SQL parameter for the Parent Plan Id</param>
        /// <returns>The SQL as a string</returns>
        private string GetDirectChildPlansOf(string parentPlanIdSqlParameter)
        {
            if (parentPlanIdSqlParameter.IndexOf("@") != 0)
                parentPlanIdSqlParameter = '@' +  parentPlanIdSqlParameter;

            return 
                $@" SELECT 
                TrainingPlanId
                FROM UserTrainingPlan UTP
                WHERE ParentPlanId = @{ parentPlanIdSqlParameter }
                OR TrainingPlanId = @{ parentPlanIdSqlParameter } ";
        }
    }

}
