//using System;
//using System.Data.SQLite;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Dapper;
//using System.Linq;
//using GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions;
//using GymProject.Application.Queries.Base;

//namespace GymProject.Application.Queries.TrainingDomain
//{
//    public class TrainingQueryWrapper : ITrainingQueryWrapper
//    {


//        private string _connectionString = string.Empty;

//        public TrainingQueryWrapper(string connectionString)
//        {
//            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString
//                : throw new ArgumentNullException(nameof(connectionString));
//        }



//        /// <summary>
//        /// Get the summaries of all the Training Plans belonging to the specified user
//        /// </summary>
//        /// <param name="userId">The ID of the user to be searched</param>
//        /// <returns>The Training Plan Summary DTO list</returns>
//        public async Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummaries(uint userId)
//        {
//            using (var connection = new SQLiteConnection(_connectionString))
//            {
//                //connection.Open();
//                connection.OpenGymAppConnection();

//                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
//                   @"SELECT TP.Id As Id, TP.Name as PlanName, TP.IsBookmarked, 
//	                TH.Id As HashtagId, TH.Body As Hashtag, TProf.Id as ProficiencyId, TProf.Name as Proficiency, 
//                    Pha.Id As PhaseId, Pha.Name as Phase,
//                	--Average Workout Days per plan
//                    (
//                        SELECT AVG(WoCount.Counter)
//                        FROM
//                        (
//                            SELECT TrainingPlanId, count(1) as Counter
//                            FROM TrainingWeek
//                            JOIN WorkoutTemplate
//                            ON TrainingWeek.Id = TrainingWeekId
//                            WHERE TrainingPlanId = TP.Id
//                            GROUP BY TrainingWeek.Id
//		                ) WoCount
//	                ) AS AvgWorkoutDays,
//                    --Average weekly working sets
//                    (
//                        SELECT round(Avg(WsCount.Counter), 1)
//                        FROM
//                        (
//                            SELECT TrainingWeek.Id, count(1) as Counter
//                            FROM TrainingWeek
//                            JOIN WorkoutTemplate
//                            ON TrainingWeek.Id = TrainingWeekId
//                            JOIN WorkUnitTemplate
//                            ON WorkoutTemplate.Id = WorkoutTemplateId
//                            JOIN WorkingSetTemplate
//                            ON WorkUnitTemplate.Id = WorkUnitTemplateId
//                            WHERE TrainingPlanId = TP.Id
//                            GROUP BY TrainingWeek.Id
//                        ) WsCount
//                    ) As AvgWorkingSets,
//                    --Average Intensity
//                    (
//                        SELECT Round(Avg(
//                        CASE
//                            WHEN Effort_EffortTypeId = 1 THEN Effort / 10.0-- Intensity[%]
//                            WHEN Effort_EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)-- RM
//                            WHEN Effort_EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)-- RPE
//                            ELSE null
//                        END), 1) as AvgIntensity
//                        FROM TrainingWeek
//                        JOIN WorkoutTemplate
//                        ON TrainingWeek.Id = TrainingWeekId
//                        JOIN WorkUnitTemplate
//                        ON WorkoutTemplate.Id = WorkoutTemplateId
//                        JOIN WorkingSetTemplate
//                        ON WorkUnitTemplate.Id = WorkUnitTemplateId
//                        WHERE TrainingPlanId = TP.Id
//                    ) As AvgIntensityPercentage,
//                    --Last workout date
//                    (
//                        SELECT Max(StartTime)
//                        FROM TrainingWeek
//                        JOIN WorkoutTemplate
//                        ON TrainingWeek.Id = TrainingWeekId
//                        JOIN WorkoutSession
//                        ON WorkoutTemplate.Id = WorkoutTemplateId
//                        WHERE TrainingPlanId = TP.Id
//                    ) As LastWorkoutTs,
//                    --Number of training weeks
//                    (
//                        SELECT Count(1)
//                        FROM TrainingWeek
//                        WHERE TrainingPlanId = TP.Id
//                    ) As TrainingWeeksNumber
//                    FROM TrainingPlan TP
//                    JOIN User U
//                    ON TP.OwnerId = U.Id
//                    -- Notes and hashtags
//                    LEFT JOIN TrainingPlanHashtag TPH
//                    ON TPH.TrainingPlanId = TP.Id
//                    LEFT JOIN TrainingHashtag TH
//                    ON TPH.HashtagId = TH.Id
//                    LEFT JOIN TrainingPlanProficiency TPP
//                    ON TPP.TrainingPlanId = TP.Id
//                    LEFT JOIN TrainingProficiency TProf
//                    ON TProf.Id = TPP.TrainingProficiencyId
//                    LEFT JOIN TrainingPlanPhase TPPh
//                    ON TPPh.TrainingPlanId = TP.Id
//                    LEFT JOIN TrainingPhase Pha
//                    ON TPPh.TrainingPhaseId = Pha.Id
//                    WHERE U.Id = @userId
//                    ORDER BY TP.IsBookmarked DESC"
//                    , new { userId }
//                );


//                //if (queryResult.AsList().Count == 0)
//                //    throw new KeyNotFoundException();

//                return MapTrainingPlanSummaryDto(queryResult);

//            }
//        }



//        private List<TrainingPlanSummaryDto> MapTrainingPlanSummaryDto(IEnumerable<dynamic> queryResult)
//        {
//            List<TrainingPlanSummaryDto> result = new List<TrainingPlanSummaryDto>();

//            for (int i = 0; i < queryResult.Count(); i++)
//            {
//                dynamic res = queryResult.ElementAt(i);
//                uint currentTrainingPlanId = (uint)res.Id;

//                // Fields shared by all the Training Plan records
//                TrainingPlanSummaryDto trainingPlanDto = new TrainingPlanSummaryDto()
//                {
//                    TrainingPlanId = currentTrainingPlanId,
//                    TrainingPlanName = (string)res.PlanName,
//                    IsBookmarked = (bool)(res.IsBookmarked != 0),
//                    AvgWorkoutDays = (float?)res.AvgWorkoutDays,
//                    AvgWorkingSets = (float?)res.AvgWorkingSets,
//                    AvgIntensityPercentage = (float?)res.AvgIntensityPercentage,
//                    LastWorkoutTimestamp = Conversions.GetDatetimeFromUnixTimestamp((long?)res.LastWorkoutTimestamp),
//                    Hashtags = new List<HashtagDto>(),
//                    TargetProficiencies = new List<ProficiencyDto>(),
//                    TargetPhases = new List<PhaseDto>(),
//                };

//                int j = i;      // Dont't skip the current row!

//                // Get all the records belonging to the TrainingPlan
//                while (j < queryResult.Count() && queryResult.ElementAt(j).Id == currentTrainingPlanId)
//                {
//                    dynamic nextRes = queryResult.ElementAt(j);

//                    if (nextRes.HashtagId != null)
//                    {
//                        if (!trainingPlanDto.Hashtags.Contains(new HashtagDto() { Id = (uint)nextRes.HashtagId }))
//                        {
//                            trainingPlanDto.Hashtags.Add(new HashtagDto()
//                            {
//                                Id = (uint)nextRes.HashtagId,
//                                Body = (string)nextRes.Hashtag,
//                            });
//                        }
//                    }

//                    if (nextRes.PhaseId != null)
//                    {
//                        if (!trainingPlanDto.TargetPhases.Contains(new PhaseDto() { Id = (uint)nextRes.PhaseId }))
//                        {
//                            trainingPlanDto.TargetPhases.Add(new PhaseDto()
//                            {
//                                Id = (uint)nextRes.PhaseId,
//                                Body = (string)nextRes.Phase,
//                            });
//                        }
//                    }

//                    if (nextRes.ProficiencyId != null)
//                    {
//                        if (!trainingPlanDto.TargetProficiencies.Contains(new ProficiencyDto() { Id = (uint)nextRes.ProficiencyId }))
//                        {
//                            trainingPlanDto.TargetProficiencies.Add(new ProficiencyDto()
//                            {
//                                Id = (uint)nextRes.ProficiencyId,
//                                Body = (string)nextRes.Proficiency,
//                            });
//                        }
//                    }

//                    j++;
//                }

//                i = j - 1;      // Don't skip any record

//                result.Add(trainingPlanDto);
//            }
//            return result;
//        }






//        /// <summary>
//        /// Get the main details of the selected Training Plan.
//        /// These include all the fields not fethced in the <see cref="TrainingPlanSummaryDto"/> excluding:
//        ///
//        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
//        ///
//        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
//        ///
//        /// </summary>
//        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
//        /// <returns>The Training Plan Details DTO</returns>
//        public async Task<TrainingPlanDetailDto> GetTrainingPlanDetails(uint TrainingPlanId)
//        {
//            using (var connection = new SQLiteConnection(_connectionString))
//            {
//                //connection.Open();
//                connection.OpenGymAppConnection();

//                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
//                   @"SELECT TP.Id, TP.TrainingPlanNoteId, TPN.Body as TrainingPlanNote,
//                    MG.Id as FocusedMuscleId, MG.Abbreviation as FocusedMuscleAbbreviation, MG.Name as FocusedMuscle,
//                    ParentPlan.Id as ParentPlanId, ParentPlan.OwnerId as ParentPlanOwnerId, TPR.ChildPlanTypeId as RelationTypeId
//                    FROM TrainingPlan TP

//                    -- TP missing data
//                    LEFT JOIN TrainingPlanNote TPN
//                    ON TP.TrainingPlanNoteId = TPN.Id
//                    LEFT JOIN TrainingPlanMuscleFocus MF
//                    ON TP.Id = MF.TrainingPlanId
//                    LEFT JOIN MuscleGroup MG
//                    ON MG.Id = MF.MuscleGroupId

//                    --Parent plan
//                    LEFT JOIN TrainingPlanRelation TPR
//                    ON TP.Id = TPR.ChildPlanId
//                    LEFT JOIN TrainingPlan ParentPlan
//                    ON ParentPlan.Id = TPR.ParentPlanId

//                    WHERE TP.Id = @TrainingPlanId"
//                    , new { TrainingPlanId }
//                );

//                // This really is an error!
//                if (queryResult.AsList().Count == 0)
//                    throw new KeyNotFoundException();

//                return MapTrainingPlanDetails(queryResult);

//            }
//        }


//        private TrainingPlanDetailDto MapTrainingPlanDetails(IEnumerable<dynamic> queryResult)
//        {
//            dynamic record = queryResult.First();
//            int i = 0;

//            TrainingPlanDetailDto result = new TrainingPlanDetailDto()
//            {
//                TrainingPlanNoteId = (uint?)record.TrainingPlanNoteId,
//                TrainingPlanNote = (string)record.TrainingPlanNote,
//                ParentPlanId = (uint?)record.ParentPlanId,
//                ParentPlanOwnerId = (uint?)record.ParentPlanOwnerId,
//                RelationTypeId = (uint?)record.RelationTypeId,
//                MusclesFocuses = new List<MuscleFocusDto>(),
//            };

//            // Check for Muscle Focus, if any
//            while (i < queryResult.Count())
//            {
//                record = queryResult.ElementAt(i);

//                if (record.FocusedMuscleId != null)
//                {
//                    // Protect from duplicate insertions, as the SQL returns cross-products
//                    if (!result.MusclesFocuses.Contains(new MuscleFocusDto() { Id = (uint)record.FocusedMuscleId }))
//                    {
//                        result.MusclesFocuses.Add(new MuscleFocusDto()
//                        {
//                            Id = (uint)record.FocusedMuscleId,
//                            MuscleAbbreviation = (string)record.FocusedMuscleAbbreviation,
//                            MuscleName = (string)record.FocusedMuscle,
//                        });
//                    }
//                }
//                i++;
//            }

//            return result;
//        }






//        /// <summary>
//        /// Get the all the details of the selected Training Plan, when no pre-fetch data is available, namely no  <see cref="TrainingPlanSummaryDto"/> in memory.
//        /// No data about Workouts and Feedbaks/Variants are loaded, please refer to:
//        ///
//        ///    - Schedules, Variants, Feedbacks - fetched in: <see cref="FullFeedbackDetailDto"/>
//        ///
//        ///    - Workout data - fetched in: <see cref="WorkoutFullPlanDto"/>
//        ///
//        /// </summary>
//        /// <param name="trainingPlanId">The ID of the Training Plan to be searched</param>
//        /// <returns>The Training Plan Details DTO</returns>
//        public async Task<TrainingPlanDetailDto> GetTrainingPlan(uint trainingPlanId)
//        {
//            using (var connection = new SQLiteConnection(_connectionString))
//            {
//                //connection.Open();
//                connection.OpenGymAppConnection();

//                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
//                   @"SELECT TP.Id, TP.Name, TP.TrainingPlanNoteId, TPN.Body as TrainingPlanNote,
//                        MG.Id as FocusedMuscleId, MG.Abbreviation as FocusedMuscleAbbreviation, MG.Name as FocusedMuscle,
//                        TH.Id As HashtagId, TH.Body As Hashtag, TProf.Id as ProficiencyId, TProf.Name as Proficiency, 
//                        Pha.Id As PhaseId, Pha.Name as Phase,
//                        ParentPlan.Id as ParentPlanId, ParentPlan.OwnerId as ParentPlanOwnerId, TPR.ChildPlanTypeId as RelationTypeId,
//                        --Number of training weeks
//                        (
//                            SELECT Count(1)
//                            FROM TrainingWeek
//                            WHERE TrainingPlanId = TP.Id
//                        ) As TrainingWeeksNumber

//                        FROM TrainingPlan TP

//                        -- TP missing data
//                        LEFT JOIN TrainingPlanNote TPN
//                        ON TP.TrainingPlanNoteId = TPN.Id
//                        LEFT JOIN TrainingPlanMuscleFocus MF
//                        ON TP.Id = MF.TrainingPlanId
//                        LEFT JOIN MuscleGroup MG
//                        ON MG.Id = MF.MuscleGroupId

//                        --Parent plan
//                        LEFT JOIN TrainingPlanRelation TPR
//                        ON TP.Id = TPR.ChildPlanId
//                        LEFT JOIN TrainingPlan ParentPlan
//                        ON ParentPlan.Id = TPR.ParentPlanId

//                        -- Notes and hashtags
//                        LEFT JOIN TrainingPlanHashtag TPH
//                        ON TPH.TrainingPlanId = TP.Id
//                        LEFT JOIN TrainingHashtag TH
//                        ON TPH.HashtagId = TH.Id
//                        LEFT JOIN TrainingPlanProficiency TPP
//                        ON TPP.TrainingPlanId = TP.Id
//                        LEFT JOIN TrainingProficiency TProf
//                        ON TProf.Id = TPP.TrainingProficiencyId
//                        LEFT JOIN TrainingPlanPhase TPPh
//                        ON TPPh.TrainingPlanId = TP.Id
//                        LEFT JOIN TrainingPhase Pha
//                        ON TPPh.TrainingPhaseId = Pha.Id

//                        WHERE TP.Id = @trainingPlanId"
//                    , new { trainingPlanId }
//                );

//                // This really is an error!
//                if (queryResult.AsList().Count == 0)
//                    throw new KeyNotFoundException();

//                return MapTrainingPlanDetails(queryResult, fullLoad: true); wdgvfwefb

//            }
//        }






//        /// <summary>
//        /// Get all the Feedbacks - full detailed - of the Training Plans which are direct childs of the specified Training Plan.
//        /// A training plan is considered direct child if:
//        ///    1. it is the parent plan of the query
//        ///    2. it is a direct variant of the parent plan
//        ///    3. it is directly inherited from the parent plan
//        ///    4. it is directly inherited of the 2. plans
//        /// Direct childs of the selected Plan which have never been scheduled are still returned
//        /// </summary>
//        /// <param name="trainingPlanId">The ID of the Root Training Plan</param>
//        /// <param name="userId">The ID of the user who is querying - This is needed n order to skip a JOIN</param>
//        /// <returns>The FullFeedbackDetailDto list</returns>
//        public async Task<IEnumerable<FullFeedbackDetailDto>> GetFullFeedbacksDetails(uint trainingPlanId, uint userId)
//        {
//            using (var connection = new SQLiteConnection(_connectionString))
//            {
//                //connection.Open();
//                connection.OpenGymAppConnection();

//                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
//                    $@"SELECT TPRoot.Id as PlanId, TPRoot.Name as TrainingPlanName, U.Username, U.Id as UserId,
//                    TS.Id as ScheduleId, Date(TS.StartDate, 'unixepoch') as ScheduleStartDate, Date(TS.EndDate, 'unixepoch') as ScheduleEndDate,
//                    Ph.Id as PhaseId, Ph.Name as Phase,
//                    TProf.Id as ProficiencyId, TProf.Name as Proficiency,
//                    TSF.Id as FeedbackId, TSF.Comment as FeedbackNote, TSF.Rating as FedbackRating, TSF.UserId as FeedbackOwnerId

//                    FROM TrainingPlan TPRoot
//                    LEFT JOIN TrainingPlanRelation TPR
//                    ON TPRoot.Id = TPR.ChildPlanId
//                    JOIN User U
//                    ON TPRoot.OwnerId = U.Id

//                    -- Proficiency
//                    LEFT JOIN TrainingSchedule TS
//                    ON TS.TrainingPlanId = TPRoot.Id
//                    LEFT JOIN UserProficiency UHP
//                    ON UHP.UserId = U.Id
//                    AND UHP.OwnerId = @userId
//                    AND UHP.StartDate <= TS.Startdate
//                    AND COALESCE(UHP.EndDate, TS.Startdate + 1) >= TS.Startdate
//                    LEFT JOIN TrainingProficiency TProf
//                    ON TProf.Id = UHP.TrainingProficiencyId

//                    -- TrainingPhase
//                    LEFT JOIN UserPhase UP
//                    ON UP.OwnerId = @userId
//                    AND UP.UserId = U.Id                -- Denormalized
//                    AND UP.StartDate <= TS.StartDate
//                    AND COALESCE(UP.EndDate, TS.Startdate + 1) >= TS.Startdate
//                    LEFT JOIN TrainingPhase Ph
//                    ON UP.TrainingPhaseId = Ph.Id

//                    -- Feedback
//                    LEFT JOIN TrainingScheduleFeedback TSF
//                    ON TSF.TrainingScheduleId = TS.Id

//                    -- Variants of main plan
//                    WHERE TPRoot.Id IN
//                    (
//                        GetDirectChildPlansOf(@trainingPlanId)
//                    )
//                    ORDER BY ScheduleStartDate"
//                    , new { trainingPlanId, userId }
//                );
//            }

//            // At least the root training plan should be fetched
//            if (queryResult.AsList().Count == 0)
//                throw new KeyNotFoundException();

//            return MapFeedbackDetails(queryResult);
//        }


//        public IEnumerable<FullFeedbackDetailDto> MapFeedbackDetails(IEnumerable<dynamic> queryResult)
//        {
//            List<FullFeedbackDetailDto> result = new List<FullFeedbackDetailDto>();

//            for (int i = 0; i < queryResult.Count(); i++)
//            {
//                dynamic currentPlan = queryResult.ElementAt(i);
//                uint currentPlanId = (uint)currentPlan.Id;

//                // Fields shared by all the Training Plan records
//                FullFeedbackDetailDto feebackDto = new FullFeedbackDetailDto()
//                {
//                    TrainingPlanId = (uint)currentPlan.PlanId,
//                    TrainingPlanName = (string)currentPlan.TrainingPlanName,
//                    TraineeId = (int)currentPlan.UserId,
//                    TraineeName = (string)currentPlan.Username,
//                    TrainingSchedules = new List<TrainingScheduleDto>(),
//                };

//                int currentIndex = i;          // Dont't skip the current row!

//                // Get all the records belonging to the same TraininPlan
//                while (currentIndex < queryResult.Count() && queryResult.ElementAt(currentIndex).Id == currentPlanId)
//                {
//                    dynamic currentRecord = queryResult.ElementAt(currentIndex);

//                    // Check if plan has ever been scheduled
//                    if (currentRecord.ScheduleId != null)
//                        TrainingSchedules.Add(MapScheduleWithFeedbacks(queryResult, ref currentIndex));     // CurrentIndex is now the index of the first record of the next Schedule (if any)

//                    // currentIndex++;
//                }
//                i = currentIndex - 1;
//            }
//        }


//        public TrainingScheduleDto MapScheduleWithFeedbacks(IEnumerable<dynamic> queryResult, ref int currentIndex)
//        {
//            dynamic currentRecord = queryResult.ElementAt(currentIndex);
//            int currentScheduleId = (uint)currentRecord.ScheduleId;

//            TrainingScheduleDto result = new TrainingScheduleDto()
//            {
//                Id = currentScheduleId,
//                StartDate = (DateTime)currentRecord.ScheduleStartDate,
//                EndDate = (DateTime?)currentRecord.ScheduleEndDate,
//                Feedbacks = new List<FeedbackDto>(),
//            };

//            if (currentRecord.PhaseId != null)
//                result.TrainingPhase = new PhaseDto()
//                {
//                    Id = (uint)currentRecord.PhaseId,
//                    Name = (string)currentRecord.Phase,
//                };

//            if (currentRecord.ProficiencyId != null)
//                result.Proficiency = new ProficiencyDto()
//                {
//                    Id = (uint)currentRecord.ProficiencyId,
//                    Name = (string)currentRecord.Proficiency,
//                };

//            // Get all the records belonging to the same Schedule
//            while (currentIndex < queryResult.Count() && queryResult.ElementAt(currentIndex).Id == currentScheduleId)
//            {
//                currentRecord = queryResult.ElementAt(currentIndex);

//                if (currentRecord.FeedbackId != null)
//                {
//                    result.Feedbacks.Add(new FeedbackDto()
//                    {
//                        Id = (uint)currentRecord.FeedbackId,
//                        Value = (float?)currentRecord.FeedbackRating,
//                        Value = (string)currentRecord.FeedbackNote,
//                        OwnerId = (string)currentRecord.FeedbackOwnerId,
//                    });
//                }
//                currentIndex++;
//            }
//        }






//        /// <summary>
//        /// Get the plan over the training weeks for the specified workout day.
//        /// In order to query for a specific training plan, the caller should already have all its training weeks and workouts names fetched, if not another query is needed.
//        /// Furthermore, notice that the weeks IDs should be ordered according to the progressive number, otherwise further post-processing is needed.
//        /// </summary>
//        /// <param name="trainingWeekIds">The IDs of the Training Weeks belonging to the Training Plan to be queried, ordered by their progressive number.</param>
//        /// <param name="workoutName">The name of the workout to be queried, which is unique with respect to the training week</param>
//        /// <returns>The WorkoutFullPlanDto list</returns>
//        public async Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutDays(IEnumerable<uint> trainingWeekIds, string workoutName)
//        {
//            using (var connection = new SQLiteConnection(_connectionString))
//            {
//                //connection.Open();
//                connection.OpenGymAppConnection();

//                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
//                   @"SELECT 
//                    WT.TrainingWeekId, WT.Id as WorkoutId,
//                    WT.Name as WorkoutName, WT.SpecificWeekday AS SpecificWeekdayId,
//                    WUTN.Id as NoteId, WUTN.Body as NoteBody,
//                    WUT.Id as WorkUnitId, WUT.ProgressiveNumber as WorkUnitProgressiveNumber, 
//                    IT1.Id as IntensityTechniqueId, IT1.Abbreviation as IntensityTechniqueAbbreviation, WST.Id as WorkingSetId,
//                    E.Id as ExcerciseId, E.Name as ExcerciseName,
//                    WST.Id as WorkingSetId, WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence AS LiftingTempo, WST.Effort,
//                    ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
//                    IT.Id as WsIntensityTechniqueId, IT.Abbreviation as IntensityTechnique

//                    FROM WorkoutTemplate WT
//                    LEFT JOIN WorkUnitTemplate WUT
//                    ON WT.Id = WUT.WorkoutTemplateId
//                    LEFT JOIN WorkUnitTemplateNote WUTN
//                    ON WUTN.Id = WUT.WorkUnitNoteId
//                    LEFT JOIN Excercise E
//                    ON E.Id = WUT.ExcerciseId
//                    LEFT JOIN WorkingSetTemplate WST
//                    ON WUT.Id = WST.WorkUnitTemplateId
//                    LEFT JOIN TrainingEffortType ET
//                    ON WST.Effort_EffortTypeId = ET.Id
//                    LEFT JOIN IntensityTechnique IT1
//                    ON IT1.Id = WUT.LinkingIntensityTechniqueId

//                    -- Fetch all the Techniques of the sets
//                    LEFT JOIN
//                    (
//                        SELECT WorkingSetId, WSTIT2.IntensityTechniqueId as Id, Abbreviation
//                        FROM WorkingSetTemplate WST
//                        JOIN WorkingSetIntensityTechnique WSTIT2
//                        ON WSTIT2.WorkingSetId = WST.Id
//                        LEFT JOIN IntensityTechnique IT2
//                        ON IT2.Id = WSTIT2.IntensityTechniqueId

//                    ) IT
//                    ON IT.WorkingSetId = WST.Id

//                    WHERE WT.TrainingWeekId IN @trainingWeekIds
//                    AND WT.Name = @workoutName

//                    ORDER BY WT.TrainingWeekId, WT.ProgressiveNumber, WUT.ProgressiveNumber, WST.ProgressiveNumber"
//                    , new { trainingWeekIds, workoutName }
//                );


//                //if (queryResult.AsList().Count == 0)
//                //    throw new KeyNotFoundException();

//                return MapTraininPlanPlannedWorkoutDays(queryResult, trainingWeekIds);

//            }
//        }

//        private IEnumerable<WorkoutFullPlanDto> MapTraininPlanPlannedWorkoutDays(IEnumerable<dynamic> queryResult, IEnumerable<uint> trainingWeekIds)
//        {
//            List<WorkoutFullPlanDto> result = new List<WorkoutFullPlanDto>();

//            for (int i = 0; i < queryResult.Count(); i++)
//            {
//                dynamic res = queryResult.ElementAt(i);
//                uint currentWeekId = (uint)res.Id;

//                // Fields shared by all the Training Plan records
//                WorkoutFullPlanDto workoutDto = new WorkoutFullPlanDto()
//                {
//                    TrainingWeekId = currentWeekId,
//                    Name = (string)res.PlanName,
//                    SpecificWeekdayId = (int?)res.SpecificWeekday,
//                    WorkUnits = new List<WorkUnitDto>(),
//                };

//                int j = i;          // Dont't skip the current row!

//                // Get all the records belonging to the same TrainingWeek
//                while (j < queryResult.Count() && queryResult.ElementAt(j).Id == currentWeekId)
//                {
//                    dynamic nextRes = queryResult.ElementAt(j);

//                    if (nextRes.WorkUnitId != null)
//                    {
//                        WorkUnitDto currentWorkUnit = new WorkUnitDto()
//                        {
//                            ProgressiveNumber = (uint)nextRes.WorkUnitProgressiveNumber,
//                            Excercise = new WorkUnitExcerciseDto()
//                            {
//                                Id = (uint)nextRes.ExcerciseId,
//                                Name = (string)nextRes.ExcerciseName,
//                            },
//                            WorkingSets = new List<WorkingSetDto>(),
//                        };

//                        if (nextRes.LinkingWorkUnitIntensityTechniqueId != null)
//                        {
//                            currentWorkUnit.LinkingIntensityTechnique = new IntensityTechniqueDto()
//                            {
//                                Id = (uint)nextRes.LinkingWorkUnitIntensityTechniqueId,
//                                Name = (string)nextRes.LinkingWorkUnitIntensityTechnique,
//                            };
//                        }

//                        if (nextRes.WorkUnitNoteId != null)
//                        {
//                            currentWorkUnit.Note = new WorkUnitNoteDto()
//                            {
//                                Id = (uint)nextRes.WorkUnitNoteId,
//                                Body = (string)nextRes.WorkUnitNote,
//                            };
//                        }

//                        // Go on with WSs...
//                        if (nextRes.WorkingSetId != null)
//                        {
//                            WorkingSets = MapWorkingSetTemplates(queryResult, ref j);   // After this, j is the last record of the current Work Unit
//                        }

//                        // Pay attention to duplicate WSs because of multiple IntTechniques




//                        // Add it
//                        workoutDto.Add(currentWorkUnit);
//                    }

//                    j++;
//                }

//                i = j - 1;      // Don't skip any record

//                result.Add(workoutDto);
//            }


//            // Check for missing workouts in training weeks

//            // Sort according to TW.ProgressiveNumber (now or before?)


//            return result;
//        }


//        private IEnumerable<WorkingSetDto> MapWorkingSetTemplates(IEnumerable<dynamic> queryResult, ref int currentIndex)
//        {
//            List<WorkingSetDto> result = new List<WorkingSetDto>();

//            dynamic currentRecord = queryResult.ElementAt(currentIndex);
//            uint currentWorkUnitId = (uint)currentRecord.WorkUnitId;

//            // Get all the records belonging to the same WorkUnit
//            while (currentIndex < queryResult.Count() && queryResult.ElementAt(currentIndex).Id == currentWorkUnitId)
//            {
//                WorkingSetDto workingSet = new WorkingSetDto()
//                {
//                    ProgressiveNumber = (uint)queryResult.WsProgressiveNumber,
//                    Repetitions = (int?)queryResult.TargetRepetitions,
//                    Rest = (int?)queryResult.Rest,
//                    LiftingTempo = (string)queryResult.LiftingTempo,
//                    // Effort = new EffortDto(),
//                    IntensityTechniques = new List<IntensityTechniqueDto>(),
//                };

//                if (queryResult.EffortTypeId != null)
//                {
//                    workingSet.Effort = new EffortDto()
//                    {
//                        Id = (uint)queryResult.EffortTypeId,
//                        Value = (int?)queryResult.Effort,
//                        Name = (string)queryResult.EffortName,
//                    };
//                }

//                // Fetch all the Intensity Techniques







//                currentIndex++;
//            }

//            currentIndex--;     // Don't skip any record

//            return result;
//        }







//        /// <summary>
//        /// SQL recursive function for searching all the first-level variant plans an the inherited ones of a specific parent plan.
//        /// A training plan is returned if:
//        ///    1. it is the parent plan of the query
//        ///    2. it is a direct variant of the parent plan
//        ///    3. it is directly inherited from the parent plan
//        ///    4. it is directly inherited of the 2. plans
//        /// The query returns the ID of each record
//        /// </summary>
//        /// <returns>The SQL as a string</returns>
//        private string GetDirectChildPlansOf(int parentTrainingPlan)

//        => @"WITH RECURSIVE IsVariantOf(Id, ChildId, ChildTypeId) AS
//                (
//                    SELECT TP.Id, TPR.ChildPlanId, TPR.ChildPlanTypeId
//                    FROM TrainingPlan TP
//                    JOIN TrainingPlanRelation TPR
//                    ON TP.Id = TPR.ParentPlanId
                                            
//                    WHERE TPR.ParentPlanId = @parentTrainingPlan      -- Get First Childs only
//                    OR ChildPlanTypeId = 2       -- Or inherited from first childs

//                    UNION ALL
                    
//                    SELECT TP.Id, IsVariantOf.ChildId, IsVariantOf.ChildTypeId
//                    FROM TrainingPlan TP
//                    JOIN TrainingPlanRelation TPR
//                    ON TP.Id = TPR.ParentPlanId
//                    JOIN IsVariantOf
//                    ON IsVariantOf.Id = TPR.ChildPlanId
                    
//                    WHERE TPR.ParentPlanId = @parentTrainingPlan
//                    AND ChildPlanTypeId = 1    -- Get First Childs only
                    
//                )
//                SELECT ChildId
//                FROM IsVariantOf
//                WHERE IsVariantOf.Id = @parentTrainingPlan

//                UNION ALL

//                VALUES(@parentTrainingPlan)";

//    }
//}