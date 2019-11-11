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

                return MapTrainingPlanSummaryDto(queryResult);

            }
        }



        private List<TrainingPlanSummaryDto> MapTrainingPlanSummaryDto(IEnumerable<dynamic> queryResult)
        {
            List<TrainingPlanSummaryDto> result = new List<TrainingPlanSummaryDto>();

            for (int i = 0; i < queryResult.Count(); i++)
            {
                dynamic res = queryResult.ElementAt(i);
                uint currentTrainingPlanId = (uint)res.Id;

                // Fields shared by all the Training Plan records
                TrainingPlanSummaryDto trainingPlanDto = new TrainingPlanSummaryDto()
                {
                    TrainingPlanId = currentTrainingPlanId,
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

                int j = i;      // Dont't skip the current row!

                // Get all the records belonging to the TrainingPlan
                while (j < queryResult.Count() && queryResult.ElementAt(j).Id == currentTrainingPlanId)
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

                i = j - 1;      // Don't skip any record

                result.Add(trainingPlanDto);
            }
            return result;
        }






        /// <summary>
        /// Get the plan over the training weeks for the specified workout day.
        /// In order to query for a specific training plan, the caller should already have all its training weeks and workouts names fetched, if not another query is needed.
        /// Furthermore, notice that the weeks IDs should be ordered according to the progressive number, otherwise further post-processing is needed.
        /// </summary>
        /// <param name="trainingWeekIds">The IDs of the Training Weeks belonging to the Training Plan to be queried, ordered by their progressive number.</param>
        /// <param name="workoutName">The name of the workout to be queried, which is unique with respect to the training week</param>
        /// <returns>The WorkoutFullPlanDto list</returns>
        public async Task<IEnumerable<WorkoutFullPlanDto>> GetTraininPlanPlannedWorkoutDays(IList<uint> trainingWeekIds, string workoutName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                //connection.Open();
                connection.OpenGymAppConnection();

                IEnumerable<dynamic> queryResult = await connection.QueryAsync<dynamic>(
                   @"SELECT WT.TrainingWeekId, WT.Id as WorkoutId,
                    WT.Name as WorkoutName, WT.SpecificWeekday, WT.ProgressiveNumber as WorkoutProgressiveNumber,
                    WUTN.Id as WorkUnitNoteId, WUTN.Body as WorkUnitNote,
                    WUT.Id as WorkUnitId, WUT.ProgressiveNumber as WorkUnitProgressiveNumber, 
                    IT1.Id as LinkingWorkUnitIntensityTechniqueId, IT1.Abbreviation as LinkingWorkUnitIntensityTechnique, WST.Id as WorkingSetId,
                    E.Id as ExcerciseId, E.Name as ExcerciseName,
                    WST.ProgressiveNumber as WsProgressiveNumber, WST.TargetRepetitions, WST.Rest, WST.Cadence, WST.Effort,
                    ET.Id as EffortTypeId, ET.Abbreviation as EffortName,
                    IT.Id as WsIntensityTechniqueId, IT.Abbreviation as IntensityTechnique

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

                    ORDER BY WT.TrainingWeekId, WT.ProgressiveNumber, WUT.ProgressiveNumber, WST.ProgressiveNumber"
                    , new { trainingWeekIds, workoutName }
                );


                //if (queryResult.AsList().Count == 0)
                //    throw new KeyNotFoundException();

                return MapTraininPlanPlannedWorkoutDays(queryResult, trainingWeekIds);

            }
        }

        private IEnumerable<WorkoutFullPlanDto> MapTraininPlanPlannedWorkoutDays(IEnumerable<dynamic> queryResult, IList<uint> trainingWeekIds)
        {
            List<WorkoutFullPlanDto> result = new List<WorkoutFullPlanDto>();

            for (int i = 0; i < queryResult.Count(); i++)
            {
                dynamic res = queryResult.ElementAt(i);
                uint currentWeekId = (uint)res.TrainingWeekId;

                // Fields shared by all the Training Plan records
                WorkoutFullPlanDto workoutDto = new WorkoutFullPlanDto()
                {
                    Id = (uint)res.WorkoutId,
                    TrainingWeekId = currentWeekId,
                    ProgressiveNumber = (uint)res.WorkoutProgressiveNumber,
                    //Name = (string)res.WorkoutName,
                    SpecificWeekdayId = (int?)res.SpecificWeekday,
                    WorkUnits = new List<WorkUnitDto>(),
                };

                int internalIndex = i;          // Dont't skip the current row!

                // Get all the records belonging to the same TrainingWeek
                while (internalIndex < queryResult.Count() && queryResult.ElementAt(internalIndex).TrainingWeekId == currentWeekId)
                {
                    dynamic nextRes = queryResult.ElementAt(internalIndex);

                    if (nextRes.WorkUnitId != null)
                    {
                        WorkUnitDto currentWorkUnit = new WorkUnitDto()
                        {
                            Id = (uint)nextRes.WorkUnitId,
                            ProgressiveNumber = (uint)nextRes.WorkUnitProgressiveNumber,
                            Excercise = new WorkUnitExcerciseDto()
                            {
                                Id = (uint)nextRes.ExcerciseId,
                                Name = (string)nextRes.ExcerciseName,
                            },
                            WorkingSets = new List<WorkingSetDto>(),
                        };

                        if (nextRes.LinkingWorkUnitIntensityTechniqueId != null)
                        {
                            currentWorkUnit.LinkingIntensityTechnique = new IntensityTechniqueDto()
                            {
                                Id = (uint)nextRes.LinkingWorkUnitIntensityTechniqueId,
                                Name = (string)nextRes.LinkingWorkUnitIntensityTechnique,
                            };
                        }

                        if (nextRes.WorkUnitNoteId != null)
                        {
                            currentWorkUnit.Note = new WorkUnitNoteDto()
                            {
                                Id = (uint)nextRes.WorkUnitNoteId,
                                Body = (string)nextRes.WorkUnitNote,
                            };
                        }

                        // Go on with WSs...
                        if (nextRes.WorkingSetId != null)
                            currentWorkUnit.WorkingSets = 
                                MapWorkingSetTemplates(queryResult, ref internalIndex);   // After this, j is the last record of the current Work Unit

                        // Add it
                        workoutDto.WorkUnits.Add(currentWorkUnit);
                    }

                    internalIndex++;
                }

                i = internalIndex - 1;      // Don't skip any record

                result.Add(workoutDto);
            }


            // Check for missing workouts in training weeks
            List<uint> missingTrainingWeeksIds = trainingWeekIds.Except(result.Select(x => x.TrainingWeekId)).ToList();
            missingTrainingWeeksIds.ForEach(x =>
                result.Add(new WorkoutFullPlanDto()
                {
                    TrainingWeekId = x,
                }));

            // Sort according to TW.ProgressiveNumber - this breaks if trainingWeekIds is not sorted ProgressiveNumber-wise
            result = result.OrderBy(x => trainingWeekIds.IndexOf(x.TrainingWeekId)).ToList();

            return result;
        }


        private ICollection<WorkingSetDto> MapWorkingSetTemplates(IEnumerable<dynamic> queryResult, ref int currentIndex)
        {
            List<WorkingSetDto> result = new List<WorkingSetDto>();

            dynamic currentRecord = queryResult.ElementAt(currentIndex);
            uint currentWorkUnitId = (uint)currentRecord.WorkUnitId;

            // Get all the records belonging to the same WorkUnit
            while (currentIndex < queryResult.Count() && queryResult.ElementAt(currentIndex).WorkUnitId == currentWorkUnitId)
            {
                currentRecord = queryResult.ElementAt(currentIndex);
                uint currentWorkingSetId = (uint)currentRecord.WorkingSetId;

                WorkingSetDto workingSet = new WorkingSetDto()
                {
                    Id = currentWorkingSetId,
                    ProgressiveNumber = (uint)currentRecord.WsProgressiveNumber,
                    Repetitions = (int?)currentRecord.TargetRepetitions,
                    Rest = (int?)currentRecord.Rest,
                    LiftingTempo = (string)currentRecord.Cadence,
                    IntensityTechniques = new List<IntensityTechniqueDto>(),
                };

                if (currentRecord.EffortTypeId != null)
                {
                    workingSet.Effort = new EffortDto()
                    {
                        Id = (uint)currentRecord.EffortTypeId,
                        Value = (int?)currentRecord.Effort,
                        Name = (string)currentRecord.EffortName,
                    };
                }

                // Get all the records belonging to the same Working Set, looking for its Intensity Techniques
                while (currentIndex < queryResult.Count() && queryResult.ElementAt(currentIndex).WorkingSetId == currentWorkingSetId)
                {
                    dynamic nextRecord = queryResult.ElementAt(currentIndex);

                    if(nextRecord.WsIntensityTechniqueId != null)
                    {
                        workingSet.IntensityTechniques.Add(
                            new IntensityTechniqueDto()
                            {
                                Id = (uint)nextRecord.WsIntensityTechniqueId,
                                Name = (string)nextRecord.IntensityTechnique,
                            });
                    }
                    currentIndex++;
                }
                result.Add(workingSet);
            }
            currentIndex--;     // Don't skip any record

            return result;
        }

    }
}