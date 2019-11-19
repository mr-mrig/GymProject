using GymProject.Application.Command;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.MediatorBehavior;
using GymProject.Application.Queries.TrainingDomain;
using GymProject.Application.Test.UnitTestEnvironment;
using GymProject.Application.Test.Utils;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS
{
    public class TrainingDomainQueriesTest
    {



        [Fact]
        public async Task GetTraininPlansSummariesTest_NoResults()
        {
            IEnumerable<TrainingPlanSummaryDto> results;
            uint id;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);

            // Dummy case:  No results
            id = uint.MaxValue;
            results = await queries.GetTraininPlansSummaries(id);
            // Check
            Assert.Empty(results);
        }



        [Fact]
        public async Task GetTraininPlansSummariesTest_StandardCase()
        {
            List<TrainingPlanSummaryDto> results;
            uint id;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);

            // Arrange
            id = 1;
            results = (await queries.GetTraininPlansSummaries(id)).ToList();

            // Check
            List<TrainingPlanSummaryDto> expectedResult = GetTraininPlansSummariesTest_StandardCaseExpected();
            Assert.Equal(expectedResult.Count(), results.Count());

            for (int ires = 0; ires < results.Count(); ires++)
            {
                string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
            }
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutDays_NoReults()
        {
            IEnumerable<WorkoutFullPlanDto> results;
            string workoutName;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);

            //List<uint> weekIds = context.TrainingPlans.Find(planId).TrainingWeeks as List<uint>;
            //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 1).TrainingWeeks.Select(x => x.Id.Value).ToList();
            List<uint> weekIds = new List<uint>() { 17, 11, 15, 1 };

            // Dummy case1:  Fake weeks -> No results
            results = await queries.GetTraininPlanPlannedWorkoutDays(new List<uint>() { uint.MaxValue }, "");
            Assert.Empty(results);

            // Dummy case2:  Fake name -> No results
            workoutName = "FAKE NAME";
            results = await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName);
            Assert.Empty(results);
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutDays_StandardCase()
        {
            List<WorkoutFullPlanDto> results;
            string workoutName;
            uint planId = 1;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);

            //List<uint> weekIds = context.TrainingPlans.Find((uint?)1).TrainingWeeks.Select(x => x.Id.Value).ToList();
            List<uint> weekIds = new List<uint>() { 17, 11, 15, 1 };


            // Arrange
            workoutName = "DAY A";
            results = (await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName)).ToList();


            // Check
            List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutDays_StandardCaseExpected();
            Assert.Equal(expectedResult.Count(), results.Count());

            for (int ires = 0; ires < results.Count(); ires++)
            {
                string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);

                //for(int iwunit = 0; iwunit < results[ires].WorkUnits.Count; iwunit++)
                //{
                //    string myJson = JsonConvert.SerializeObject(results[ires].WorkUnits.ElementAt(iwunit));
                //    string expectedJson = JsonConvert.SerializeObject(expectedResult[ires].WorkUnits.ElementAt(iwunit));
                //    Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
                //}
            }
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutDays_DraftPlanCase()
        {
            List<WorkoutFullPlanDto> results;
            string workoutName;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);


            // Arrange
            //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
            List<uint> weekIds = new List<uint>() { 10 };
            workoutName = "DAY A";
            results = (await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName)).ToList();

            // Check
            List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutDays_DraftPlanCaseExpected();
            Assert.Equal(expectedResult.Count(), results.Count());

            for (int ires = 0; ires < results.Count(); ires++)
            {
                string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
            }
        }
        


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutDays_DraftWorkUnitCase()
        {
            List<WorkoutFullPlanDto> results;
            string workoutName;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);


            // Arrange
            //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
            List<uint> weekIds = new List<uint>() { 10 };
            workoutName = "DAY B";
            results = (await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName)).ToList();

            // Check
            List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutDays_DraftWorkUnitCaseExpected();
            Assert.Equal(expectedResult.Count(), results.Count());

            for (int ires = 0; ires < results.Count(); ires++)
            {
                string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
            }
        }
        


        [Fact]
        public async Task GetFullFeedbacksDetails_NoResultsCase()
        {
            List<WorkoutFullPlanDto> results;
            string workoutName;

            throw new NotImplementedException();

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);


            // Arrange
            //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
            List<uint> weekIds = new List<uint>() { 10 };
            workoutName = "DAY B";
            results = (await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName)).ToList();

            // Check
            List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutDays_DraftWorkUnitCaseExpected();
            Assert.Equal(expectedResult.Count(), results.Count());

            for (int ires = 0; ires < results.Count(); ires++)
            {
                string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
            }
        }
        


        [Fact]
        public async Task GetFullFeedbacksDetails_StandardCase()
        {
            List<WorkoutFullPlanDto> results;
            string workoutName;

            throw new NotImplementedException();

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);


            // Arrange
            //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
            List<uint> weekIds = new List<uint>() { 10 };
            workoutName = "DAY B";
            results = (await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName)).ToList();

            // Check
            List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutDays_DraftWorkUnitCaseExpected();
            Assert.Equal(expectedResult.Count(), results.Count());

            for (int ires = 0; ires < results.Count(); ires++)
            {
                string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
            }
        }





        #region Expected Results Creators

        private List<TrainingPlanSummaryDto> GetTraininPlansSummariesTest_StandardCaseExpected()
        {
            List<TrainingPlanSummaryDto> expectedResults = new List<TrainingPlanSummaryDto>();

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                TrainingPlanId = 1,
                TrainingPlanName = "Plan1 User1",
                IsBookmarked = true,
                AvgWorkoutDays = 4.5f,
                AvgWorkingSets = 36,
                AvgIntensityPercentage = 68.8f,
                LastWorkoutTimestamp = null,
                TrainingWeeksCounter = 4,
                Hashtags = new List<HashtagDto>()
                {
                    new HashtagDto(){ HashtagId = 2, Hashtag = "MyHashtag4", },
                    new HashtagDto(){ HashtagId = 3, Hashtag = "MyHashtag2", },
                    new HashtagDto(){ HashtagId = 4, Hashtag = "MyHashtag1", },
                },
                TargetProficiencies = new List<TrainingProficiencyDto>()
                {
                    new TrainingProficiencyDto() { TrainingProficiencyId = 1, TrainingProficiency = "Pro", },
                    new TrainingProficiencyDto() { TrainingProficiencyId = 2, TrainingProficiency = "Advanced", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {
                    new TrainingPhaseDto() { TrainingPhaseId = 1, TrainingPhase = "My public", },
                    new TrainingPhaseDto() { TrainingPhaseId = 2, TrainingPhase = "My Private", },
                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                TrainingPlanId = 4,
                TrainingPlanName = "Plan4 User1",
                IsBookmarked = true,
                AvgWorkoutDays = 3,
                AvgWorkingSets = 27,
                AvgIntensityPercentage = null,
                LastWorkoutTimestamp = null,
                TrainingWeeksCounter = 3,
                Hashtags = new List<HashtagDto>()
                {
                    new HashtagDto(){ HashtagId = 4, Hashtag = "MyHashtag1", },
                },
                TargetProficiencies = new List<TrainingProficiencyDto>()
                {
                    new TrainingProficiencyDto() { TrainingProficiencyId = 3, TrainingProficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                TrainingPlanId = 5,
                TrainingPlanName = "Plan5 User1 Variant of Plan1 - never scheduled",
                IsBookmarked = true,
                AvgWorkoutDays = 3,
                AvgWorkingSets = 27,
                AvgIntensityPercentage = null,
                LastWorkoutTimestamp = null,
                TrainingWeeksCounter = 3,
                Hashtags = new List<HashtagDto>()
                {
                    new HashtagDto(){ HashtagId = 4, Hashtag = "MyHashtag1", },
                },
                TargetProficiencies = new List<TrainingProficiencyDto>()
                {
                    new TrainingProficiencyDto() { TrainingProficiencyId = 3, TrainingProficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                TrainingPlanId = 2,
                TrainingPlanName = "Plan2 User1 Variant of Plan1",
                IsBookmarked = false,
                AvgWorkoutDays = 2,
                AvgWorkingSets = null,
                AvgIntensityPercentage = null,
                LastWorkoutTimestamp = null,
                TrainingWeeksCounter = 1,
                Hashtags = new List<HashtagDto>()
                {
                    new HashtagDto(){ HashtagId = 4, Hashtag = "MyHashtag1", },
                },
                TargetProficiencies = new List<TrainingProficiencyDto>()
                {
                    new TrainingProficiencyDto() { TrainingProficiencyId = 3, TrainingProficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                TrainingPlanId = 3,
                TrainingPlanName = "Plan3 User1 Variant of Plan2",
                IsBookmarked = false,
                AvgWorkoutDays = 3,
                AvgWorkingSets = 27,
                AvgIntensityPercentage = null,
                LastWorkoutTimestamp = null,
                TrainingWeeksCounter = 3,
                Hashtags = new List<HashtagDto>()
                {
                    new HashtagDto(){ HashtagId = 4, Hashtag = "MyHashtag1", },
                },
                TargetProficiencies = new List<TrainingProficiencyDto>()
                {
                    new TrainingProficiencyDto() { TrainingProficiencyId = 3, TrainingProficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });


            return expectedResults;
        }


        private List<WorkoutFullPlanDto> GetTraininPlanPlannedWorkoutDays_StandardCaseExpected()
        {
            List<WorkoutFullPlanDto> expectedResults = new List<WorkoutFullPlanDto>();

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                TrainingWeekId = 6,
                WorkoutId = 1,
                WorkoutName = "DAY A",
                SpecificWeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WorkUnitId = 1,
                        NoteId = 1,
                        NoteBody = "note4",
                        WorkUnitProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 1,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 4, IntensityTechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 2,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 2,
                        WorkUnitProgressiveNumber = 1,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 3,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 4,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 5,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 2, IntensityTechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 6,
                                WsProgressiveNumber = 3,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 3,
                        WorkUnitProgressiveNumber = 2,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 7,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 8,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                },
            });

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                TrainingWeekId = 3,
                WorkoutId = 6,
                WorkoutName = "DAY A",
                SpecificWeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WorkUnitId = 16,
                        NoteId = 1,
                        NoteBody = "note4",
                        WorkUnitProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 41,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 4, IntensityTechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 42,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 17,
                        WorkUnitProgressiveNumber = 1,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 43,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 44,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 45,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 2, IntensityTechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 46,
                                WsProgressiveNumber = 3,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 18,
                        WorkUnitProgressiveNumber = 2,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 47,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 48,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                },
            });

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                TrainingWeekId = 8,
                WorkoutId = 10,
                WorkoutName = "DAY A",
                SpecificWeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WorkUnitId = 28,
                        NoteId = 1,
                        NoteBody = "note4",
                        WorkUnitProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 73,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 4, IntensityTechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 74,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 29,
                        WorkUnitProgressiveNumber = 1,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 75,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 76,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 77,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 2, IntensityTechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 78,
                                WsProgressiveNumber = 3,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 30,
                        WorkUnitProgressiveNumber = 2,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 79,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 80,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                },
            });

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                TrainingWeekId = 12,
                WorkoutId = 15,
                WorkoutName = "DAY A",
                SpecificWeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WorkUnitId = 43,
                        NoteId = 1,
                        NoteBody = "note4",
                        WorkUnitProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 113,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 4, IntensityTechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 114,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 44,
                        WorkUnitProgressiveNumber = 1,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 115,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 116,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 117,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 2, IntensityTechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 118,
                                WsProgressiveNumber = 3,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                    new WorkUnitDto()
                    {
                        WorkUnitId = 45,
                        WorkUnitProgressiveNumber = 2,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetDto>()
                        {
                            new WorkingSetDto()
                            {
                                WorkingSetId = 119,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { IntensityTechniqueId = 1, IntensityTechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetDto()
                            {
                                WorkingSetId = 120,
                                WsProgressiveNumber = 1,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                },
                            },
                        },
                    },
                },
            });

            return expectedResults;
        }


        private List<WorkoutFullPlanDto> GetTraininPlanPlannedWorkoutDays_DraftPlanCaseExpected()
        {
            List<WorkoutFullPlanDto> expectedResults = new List<WorkoutFullPlanDto>();

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                TrainingWeekId = 10,
                WorkoutId = 19,
                WorkoutName = "DAY A",
                SpecificWeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    //new WorkUnitDto(),
                },
            });

            return expectedResults;
        }


        private List<WorkoutFullPlanDto> GetTraininPlanPlannedWorkoutDays_DraftWorkUnitCaseExpected()
        {
            List<WorkoutFullPlanDto> expectedResults = new List<WorkoutFullPlanDto>();

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                TrainingWeekId = 10,
                WorkoutId = 19,
                WorkoutName = "DAY B",
                SpecificWeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto
                    {
                        WorkUnitId = 55,
                        WorkUnitProgressiveNumber = 0,
                        NoteId = null,
                        NoteBody = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 4,
                        ExcerciseName = "Excercise1",
                        WorkingSets = new List<WorkingSetDto>(),
                    },
                },
            });

            return expectedResults;
        }
        #endregion

    }
}
