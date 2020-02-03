using GymProject.Application.Queries.TrainingDomain;
using GymProject.Application.Test.UnitTestEnvironment;
using GymProject.Application.Test.Utils;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{
    public class TrainingDomainQueriesTest
    {




        [Fact]
        public async Task GetTraininPlansSummariesTest_NoResults()
        {
            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);

                    // Dummy case:  No results
                    uint id = uint.MaxValue;
                    var results = await queries.GetTraininPlansSummariesAsync(id);
                    // Check
                    Assert.Empty(results);
                }
            }
        }



        [Fact]
        public async Task GetTraininPlansSummariesTest_StandardCase()
        {
            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);

                    // Arrange
                    uint id = 1;
                    var results = (await queries.GetTraininPlansSummariesAsync(id)).ToList();

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
            }
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutAsync_NoResults()
        {
            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);

                    //List<uint> weekIds = context.TrainingPlans.Find(planId).TrainingWeeks as List<uint>;
                    //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 1).TrainingWeeks.Select(x => x.Id.Value).ToList();
                    List<uint> weekIds = new List<uint>() { 17, 11, 15, 1 };

                    // Dummy case1:  Fake weeks -> No results
                    var results = await queries.GetTraininPlanPlannedWorkoutAsync(new List<uint>() { uint.MaxValue }, "");
                    Assert.Empty(results);

                    // Dummy case2:  Fake name -> No results
                    string workoutName = "FAKE NAME";
                    results = await queries.GetTraininPlanPlannedWorkoutAsync(weekIds, workoutName);
                    Assert.Empty(results);
                }
            }
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutAsync_StandardCase()
        {
            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);

                    //List<uint> weekIds = context.TrainingPlans.Find((uint?)1).TrainingWeeks.Select(x => x.Id.Value).ToList();
                    List<uint> weekIds = new List<uint>() { 17, 11, 15, 1 };


                    // Arrange
                    string workoutName = "DAY A";
                    var results = (await queries.GetTraininPlanPlannedWorkoutAsync(weekIds, workoutName)).ToList();


                    // Check
                    List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutAsync_StandardCaseExpected();
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
            }
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutAsync_DraftPlanCase()
        {
            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);


                    // Arrange
                    //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
                    List<uint> weekIds = new List<uint>() { 10 };
                    string workoutName = "DAY A";
                    var results = (await queries.GetTraininPlanPlannedWorkoutAsync(weekIds, workoutName)).ToList();

                    // Check
                    List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutAsync_DraftPlanCaseExpected();
                    Assert.Equal(expectedResult.Count(), results.Count());

                    for (int ires = 0; ires < results.Count(); ires++)
                    {
                        string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                        string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                        Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
                    }
                }
            }
        }
        


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutAsync_DraftWorkUnitCase()
        {
            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);


                    //RIGM: to be removed
                    var repo = new Infrastructure.Persistence.SqlRepository.TrainingDomain.SQLTrainingPlanRepository(context);
                    repo.Find(1);

                    // Arrange
                    //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
                    List<uint> weekIds = new List<uint>() { 10 };
                    string workoutName = "DAY B";
                    var results = (await queries.GetTraininPlanPlannedWorkoutAsync(weekIds, workoutName)).ToList();

                    // Check
                    List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutAsync_DraftWorkUnitCaseExpected();
                    Assert.Equal(expectedResult.Count(), results.Count());

                    for (int ires = 0; ires < results.Count(); ires++)
                    {
                        string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                        string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                        Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
                    }
                }
            }
        }
        


        [Fact]
        public async Task GetFullFeedbacksDetails_NoResultsCase()
        {
            throw new NotImplementedException();
            List<WorkoutFullPlanDto> results;
            string workoutName;

            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);


                    // Arrange
                    //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
                    List<uint> weekIds = new List<uint>() { 10 };
                    workoutName = "DAY B";
                    results = (await queries.GetTraininPlanPlannedWorkoutAsync(weekIds, workoutName)).ToList();

                    // Check
                    List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutAsync_DraftWorkUnitCaseExpected();
                    Assert.Equal(expectedResult.Count(), results.Count());

                    for (int ires = 0; ires < results.Count(); ires++)
                    {
                        string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                        string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                        Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
                    }
                }
            }
        }
        


        [Fact]
        public async Task GetFullFeedbacksDetails_StandardCase()
        {
            throw new NotImplementedException();
            List<WorkoutFullPlanDto> results;
            string workoutName;

            using (SQLDbContextFactory factory = new SQLDbContextFactory())
            {
                using (GymContext context = await factory.CreateContextAsync())
                {
                    TrainingQueryWrapper queries = new TrainingQueryWrapper(context.ConnectionString);

                    // Arrange
                    //List<uint> weekIds = context.TrainingPlans.Single(x => x.Id == 2).TrainingWeeks.Select(x => x.Id.Value).ToList();
                    List<uint> weekIds = new List<uint>() { 10 };
                    workoutName = "DAY B";
                    results = (await queries.GetTraininPlanPlannedWorkoutAsync(weekIds, workoutName)).ToList();

                    // Check
                    List<WorkoutFullPlanDto> expectedResult = GetTraininPlanPlannedWorkoutAsync_DraftWorkUnitCaseExpected();
                    Assert.Equal(expectedResult.Count(), results.Count());

                    for (int ires = 0; ires < results.Count(); ires++)
                    {
                        string myJson = ApplicationTestService.JsonUnitTestSafeSerializer(results[ires]);
                        string expectedJson = ApplicationTestService.JsonUnitTestSafeSerializer(expectedResult[ires]);
                        Assert.Equal(expectedJson, myJson, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
                    }
                }
            }
        }





        #region Expected Results Builders

        private List<TrainingPlanSummaryDto> GetTraininPlansSummariesTest_StandardCaseExpected()
        {
            List<TrainingPlanSummaryDto> expectedResults = new List<TrainingPlanSummaryDto>();

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                PlanId = 1,
                PlanName = "Plan1 User1",
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
                    new TrainingProficiencyDto() { ProficiencyId = 1, Proficiency = "Pro", },
                    new TrainingProficiencyDto() { ProficiencyId = 2, Proficiency = "Advanced", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {
                    new TrainingPhaseDto() { PhaseId = 1, Phase = "My public", },
                    new TrainingPhaseDto() { PhaseId = 2, Phase = "My Private", },
                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                PlanId = 4,
                PlanName = "Plan4 User1",
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
                    new TrainingProficiencyDto() { ProficiencyId = 3, Proficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                PlanId = 5,
                PlanName = "Plan5 User1 Variant of Plan1 - never scheduled",
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
                    new TrainingProficiencyDto() { ProficiencyId = 3, Proficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                PlanId = 2,
                PlanName = "Plan2 User1 Variant of Plan1",
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
                    new TrainingProficiencyDto() { ProficiencyId = 3, Proficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });

            expectedResults.Add(new TrainingPlanSummaryDto()
            {
                PlanId = 3,
                PlanName = "Plan3 User1 Variant of Plan2",
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
                    new TrainingProficiencyDto() { ProficiencyId = 3, Proficiency = "Intermediate", },
                },
                TargetPhases = new List<TrainingPhaseDto>()
                {

                }
            });


            return expectedResults;
        }


        private List<WorkoutFullPlanDto> GetTraininPlanPlannedWorkoutAsync_StandardCaseExpected()
        {
            List<WorkoutFullPlanDto> expectedResults = new List<WorkoutFullPlanDto>();

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                WeekId = 6,
                WorkoutId = 1,
                WorkoutName = "DAY A",
                WeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WuId = 1,
                        NoteId = 1,
                        Note = "note4",
                        WuProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 1,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { TechniqueId = 4, TechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 2,
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
                        WuId = 2,
                        WuProgressiveNumber = 1,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 3,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 4,
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
                            new WorkingSetTemplateDto()
                            {
                                WsId = 5,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 2, TechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 6,
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
                        WuId = 3,
                        WuProgressiveNumber = 2,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 7,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 8,
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
                WeekId = 3,
                WorkoutId = 6,
                WorkoutName = "DAY A",
                WeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WuId = 16,
                        NoteId = 1,
                        Note = "note4",
                        WuProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 41,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { TechniqueId = 4, TechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 42,
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
                        WuId = 17,
                        WuProgressiveNumber = 1,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 43,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 44,
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
                            new WorkingSetTemplateDto()
                            {
                                WsId = 45,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 2, TechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 46,
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
                        WuId = 18,
                        WuProgressiveNumber = 2,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 47,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 48,
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
                WeekId = 8,
                WorkoutId = 10,
                WorkoutName = "DAY A",
                WeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WuId = 28,
                        NoteId = 1,
                        Note = "note4",
                        WuProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 73,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { TechniqueId = 4, TechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 74,
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
                        WuId = 29,
                        WuProgressiveNumber = 1,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 75,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 76,
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
                            new WorkingSetTemplateDto()
                            {
                                WsId = 77,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 2, TechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 78,
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
                        WuId = 30,
                        WuProgressiveNumber = 2,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 79,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 80,
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
                WeekId = 12,
                WorkoutId = 15,
                WorkoutName = "DAY A",
                WeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto()
                    {
                        WuId = 43,
                        NoteId = 1,
                        Note = "note4",
                        WuProgressiveNumber = 0,
                        WuIntensityTechniqueId = 5,
                        WuIntensityTechniqueAbbreviation = "IT5",
                        ExcerciseId = 1,
                        ExcerciseName = "Excercise4",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 113,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 120,
                                LiftingTempo = null,
                                Effort = 12,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                    new IntensityTechniqueDto() { TechniqueId = 4, TechniqueAbbreviation = "IT2" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 114,
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
                        WuId = 44,
                        WuProgressiveNumber = 1,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 2,
                        ExcerciseName = "Excercise3",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 115,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 116,
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
                            new WorkingSetTemplateDto()
                            {
                                WsId = 117,
                                WsProgressiveNumber = 2,
                                TargetRepetitions = 10,
                                Rest = null,
                                LiftingTempo = "3030",
                                Effort = null,
                                EffortTypeId = null,
                                EffortName = null,
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 2, TechniqueAbbreviation = "IT3" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 118,
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
                        WuId = 45,
                        WuProgressiveNumber = 2,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 3,
                        ExcerciseName = "Excercise2",
                        WorkingSets = new List<WorkingSetTemplateDto>()
                        {
                            new WorkingSetTemplateDto()
                            {
                                WsId = 119,
                                WsProgressiveNumber = 0,
                                TargetRepetitions = 10,
                                Rest = 90,
                                LiftingTempo = null,
                                Effort = 15,
                                EffortTypeId = 2,
                                EffortName = "RM",
                                IntensityTechniques = new List<IntensityTechniqueDto>()
                                {
                                    new IntensityTechniqueDto() { TechniqueId = 1, TechniqueAbbreviation = "IT4" },
                                },
                            },
                            new WorkingSetTemplateDto()
                            {
                                WsId = 120,
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


        private List<WorkoutFullPlanDto> GetTraininPlanPlannedWorkoutAsync_DraftPlanCaseExpected()
        {
            List<WorkoutFullPlanDto> expectedResults = new List<WorkoutFullPlanDto>();

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                WeekId = 10,
                WorkoutId = 19,
                WorkoutName = "DAY A",
                WeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    //new WorkUnitDto(),
                },
            });

            return expectedResults;
        }


        private List<WorkoutFullPlanDto> GetTraininPlanPlannedWorkoutAsync_DraftWorkUnitCaseExpected()
        {
            List<WorkoutFullPlanDto> expectedResults = new List<WorkoutFullPlanDto>();

            expectedResults.Add(new WorkoutFullPlanDto()
            {
                WeekId = 10,
                WorkoutId = 19,
                WorkoutName = "DAY B",
                WeekdayId = 0,
                WorkUnits = new List<WorkUnitDto>()
                {
                    new WorkUnitDto
                    {
                        WuId = 55,
                        WuProgressiveNumber = 0,
                        NoteId = null,
                        Note = null,
                        WuIntensityTechniqueId = null,
                        WuIntensityTechniqueAbbreviation = null,
                        ExcerciseId = 4,
                        ExcerciseName = "Excercise1",
                        WorkingSets = new List<WorkingSetTemplateDto>(),
                    },
                },
            });

            return expectedResults;
        }
        #endregion

    }
}
