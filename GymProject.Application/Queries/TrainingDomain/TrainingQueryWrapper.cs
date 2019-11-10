using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using GymProject.Infrastructure.Persistence.EFContext.SQLiteExtensions;
using GymProject.Application.Queries.Base;

namespace GymProject.Application.Queries.TrainingDomain
{
    public class TrainingQueryWrapper : ITrainingQueryWrapper
    {


        private string _connectionString = string.Empty;

        public TrainingQueryWrapper(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString 
                : throw new ArgumentNullException(nameof(connectionString));
        }



        /// <summary>
        /// Get the summaries of all the Training Plans belonging to the specified user
        /// </summary>
        /// <param name="trainingPlanId">The ID of the user to be searched</param>
        /// <returns>The Training Plan Summary DTO list</returns>
        public async Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummaries(uint userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                //connection.Open();
                connection.OpenGymAppConnection();

                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
                   @"SELECT TP.Id As Id, TP.Name as PlanName, TP.IsBookmarked, 
	                TH.Id As HashtagId, TH.Body As Hashtag, TProf.Id as ProficiencyId, TProf.Name as Proficiency, 
                    Pha.Id As PhaseId, Pha.Name as Phase,
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
                    ) As LastWorkoutTs
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
                    ORDER BY TP.IsBookmarked DESC"
                    , new { userId }
                );


                //if (queryResult.AsList().Count == 0)
                //    throw new KeyNotFoundException();

                return mapTrainingPlanSummaryDto(queryResult);

            }
        }



        private List<TrainingPlanSummaryDto> mapTrainingPlanSummaryDto(IEnumerable<dynamic> queryResult)
        {
            List<TrainingPlanSummaryDto> result = new List<TrainingPlanSummaryDto>();

            for (int i = 0; i < queryResult.Count(); i++)
            {
                dynamic res = queryResult.ElementAt(i);
                uint nextResrentPlanId = (uint)res.Id;

                // Fields shared by all the Training Plan records
                TrainingPlanSummaryDto trainingPlanDto = new TrainingPlanSummaryDto()
                {
                    TrainingPlanId = nextResrentPlanId,
                    TrainingPlanName = (string)res.PlanName,
                    IsBookmarked = (bool)(res.IsBookmarked != 0),
                    AvgWorkoutDays = (float?)res.AvgWorkoutDays,
                    AvgWorkingSets = (float?)res.AvgWorkingSets,
                    AvgIntensityPercentage = (float?)res.AvgIntensityPercentage,
                    LastWorkoutTimestamp = Conversions.GetDatetimeFromUnixTimestamp((long?)res.LastWorkoutTimestamp),
                    Hashtags = new List<HashtagDto>(),
                    TargetProficiencies = new List<ProficiencyDto>(),
                    TargetPhases = new List<PhaseDto>(),
                };

                // Get all the records belonging to the TrainingPlan
                int j = i;

                while (j < queryResult.Count() && queryResult.ElementAt(j).Id == nextResrentPlanId)
                {
                    dynamic nextRes = queryResult.ElementAt(j);

                    if (nextRes.HashtagId != null)
                    {
                        if (!trainingPlanDto.Hashtags.Contains(new HashtagDto() { Id = (uint)nextRes.HashtagId }))
                        {
                            trainingPlanDto.Hashtags.Add(new HashtagDto()
                            {
                                Id = (uint)nextRes.HashtagId,
                                Body = (string)nextRes.Hashtag,
                            });
                        }
                    }

                    if (nextRes.PhaseId != null)
                    {
                        if (!trainingPlanDto.TargetPhases.Contains(new PhaseDto() { Id = (uint)nextRes.PhaseId }))
                        {
                            trainingPlanDto.TargetPhases.Add(new PhaseDto()
                            {
                                Id = (uint)nextRes.PhaseId,
                                Body = (string)nextRes.Phase,
                            });
                        }
                    }

                    if (nextRes.ProficiencyId != null)
                    {
                        if (!trainingPlanDto.TargetProficiencies.Contains(new ProficiencyDto() { Id = (uint)nextRes.ProficiencyId }))
                        {
                            trainingPlanDto.TargetProficiencies.Add(new ProficiencyDto()
                            {
                                Id = (uint)nextRes.ProficiencyId,
                                Body = (string)nextRes.Proficiency,
                            });
                        }
                    }

                    j++;
                }

                i = j - 1;

                result.Add(trainingPlanDto);
            }
            return result;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainingPlanId">The ID of the user to be searched</param>
        /// <returns>The Training Plan Summary DTO list</returns>
        public async Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlanPlannedWorkoutDays(uint trainingPlanId, string workoutName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                //connection.Open();
                connection.OpenGymAppConnection();

                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
                   @"SELECT WT.Id as WorkoutId, WT.ProgressiveNumber as WorkoutProgressiveNumber, 
                    WT.TrainingWeekId, W.ProgressiveNumber as WeekProgressiveNumber, WT.Name as WorkoutName, WT.SpecificWeekday,
                    WUTN.Id as WorkUnitNoteId, WUTN.Body as WorkUnitNote,
                    WUT.Id as WorkUnitTemplateId, WUT.LinkingIntensityTechniqueId as LinkingIntensityTechniqueId, IT1.Abbreviation as LinkingIntensityTechnique,
                    E.Id as ExcerciseId, E.Name as ExcerciseName,
                    WST.Id as WorkingSetId, WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence, WST.Effort,
                    ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
                    IT.Id as WsIntensityTechniqueId, IT.Abbreviation as IntensityTechnique

                    FROM TrainingWeek W
                    JOIN WorkoutTemplate WT
                    ON W.Id = WT.TrainingWeekId
                    JOIN WorkUnitTemplate WUT
                    ON WT.Id = WUT.WorkoutTemplateId
                    LEFT JOIN WorkUnitTemplateNote WUTN
                    ON WUTN.Id = WUT.WorkUnitNoteId
                    JOIN Excercise E
                    ON E.Id = WUT.ExcerciseId
                    JOIN WorkingSetTemplate WST
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

                    WHERE W.TrainingPlanId = @trainingPlanId
                    AND WT.Name = @workoutName

                    ORDER BY W.ProgressiveNumber, WT.ProgressiveNumber, WUT.ProgressiveNumber, WST.ProgressiveNumber"
                    , new { trainingPlanId, workoutName }
                );


                //if (queryResult.AsList().Count == 0)
                //    throw new KeyNotFoundException();

                //return mapTrainingPlanSummaryDto(queryResult);

            }
        }
    }
}
