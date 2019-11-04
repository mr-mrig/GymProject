using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

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



        public async Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummaries(int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
                   @"SELECT TP.Id As Id, TP.Name as PlanName, TP.IsBookmarked, 
	                TH.Id Aa HAshtagId, TH.Body As Hashtag, TProf.Id as ProficiencyId, TProf.Name as Proficiency, 
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
                            WHEN EffortTypeId = 1 THEN Effort / 10.0-- Intensity[%]
                            WHEN EffortTypeId = 2 THEN Round(RmToIntensityPerc(Effort), 1)-- RM
                            WHEN EffortTypeId = 3 THEN Round(RmToIntensityPerc(TargetRepetitions + (10 - Effort)), 1)-- RPE
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
                    ) As AvgIntensityPerc,
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
                uint nextResrentPlanId = res.Id;

                // Fields shared by all the Training Plan records
                TrainingPlanSummaryDto trainingPlanDto = new TrainingPlanSummaryDto()
                {
                    TrainingPlanId = nextResrentPlanId,
                    TrainingPlanName = res.PlanName,
                    IsBookmarked = res.IsBookmarked,
                    AvgWorkoutDays = res.AvgWorkoutDays,
                    AvgWorkingSets = res.AvgWorkingSets,
                    AvgIntensityPercentage = res.AvgIntensityPercentage,
                    LastWorkoutTimestamp = res.LastWorkoutTimestamp,
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
                                Id = res.HashtagId,
                                Body = res.Hashtag,
                            });
                        }
                    }

                    if (nextRes.PhaseId != null)
                    {
                        if (!trainingPlanDto.TargetPhases.Contains(new PhaseDto() { Id = (uint)nextRes.PhaseId }))
                        {
                            trainingPlanDto.TargetPhases.Add(new PhaseDto()
                            {
                                Id = res.PhaseId,
                                Body = res.Phase,
                            });
                        }
                    }

                    if (nextRes.ProficiencyId != null)
                    {
                        if (!trainingPlanDto.TargetProficiencies.Contains(new ProficiencyDto() { Id = (uint)nextRes.PhaseId }))
                        {
                            trainingPlanDto.TargetProficiencies.Add(new ProficiencyDto()
                            {
                                Id = res.ProficiencyId,
                                Body = res.Proficiency,
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
    }
}
