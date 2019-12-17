using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Test.FakeCommands;
using GymProject.Application.Test.Utils;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{
    public class TrainingDomainTransactionalTest
    {




        [Fact]
        public async Task TagTrainingPlanAsNewHashtagCommand_TransactionStep1Fail()
        {
            GymContext context;
            ILogger<FakeTagTrainingPlanAsNewHashtagCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeTagTrainingPlanAsNewHashtagCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athleteRepo = new SQLAthleteRepository(context);
            var hashtagRepo = new SQLTrainingHashtagRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            string hashtagBody = "MyHashtag";
            var planHashtagsBefore = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var hashtagsBefore = context.TrainingHashtags.ToList();

            TagTrainingPlanAsNewHashtagCommand command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, hashtagBody);
            FakeTagTrainingPlanAsNewHashtagCommandHandler handler = new FakeTagTrainingPlanAsNewHashtagCommandHandler(athleteRepo, hashtagRepo, logger, true, false);

            Assert.False(await handler.Handle(command, default));

            // Check
            var planHashtagsAfter = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var hashtagsAfter = context.TrainingHashtags.ToList();

            Assert.True(hashtagsBefore.SequenceEqual(hashtagsAfter));
            Assert.True(planHashtagsBefore.SequenceEqual(planHashtagsAfter));
        }


        [Fact]
        public async Task TagTrainingPlanAsNewHashtagCommand_TransactionStep2Fail()
        {
            GymContext context;
            ILogger<FakeTagTrainingPlanAsNewHashtagCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeTagTrainingPlanAsNewHashtagCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athleteRepo = new SQLAthleteRepository(context);
            var hashtagRepo = new SQLTrainingHashtagRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            string hashtagBody = "MyHashtag";
            //var planHashtagsBefore = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var planHashtagsBefore = context.Athletes.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var hashtagsBefore = context.TrainingHashtags.ToList();

            TagTrainingPlanAsNewHashtagCommand command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, hashtagBody);
            FakeTagTrainingPlanAsNewHashtagCommandHandler handler = new FakeTagTrainingPlanAsNewHashtagCommandHandler(athleteRepo, hashtagRepo, logger, false, true);

            Assert.False(await handler.Handle(command, default));

            // Check
            //var planHashtagsAfter = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var planHashtagsAfter = context.Athletes.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var hashtagsAfter = context.TrainingHashtags.ToList();

            Assert.True(hashtagsBefore.SequenceEqual(hashtagsAfter));
            Assert.True(planHashtagsBefore.SequenceEqual(planHashtagsAfter));
        }



        [Fact]
        public async Task DraftTrainingPlanCommand_TransactionFail()
        {
            GymContext context;
            ILogger<FakeCreateDraftTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeCreateDraftTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IAthleteRepository athleteRepo = new SQLAthleteRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            List<TrainingPlanRoot> plansBefore = context.TrainingPlans.ToList();
            List<UserTrainingPlanEntity> userPlansBefore = context.Athletes.ToList().SelectMany(x => x.TrainingPlans).ToList();

            // Test
            uint userId = 1;

            CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(userId);
            FakeCreateDraftTrainingPlanCommandHandler handler = new FakeCreateDraftTrainingPlanCommandHandler(planRepository, athleteRepo, logger);

            Assert.False(await handler.Handle(command, default));

            // Check the two operations have failed as a whole
            List<TrainingPlanRoot> plansAfter = context.TrainingPlans.ToList();
            List<UserTrainingPlanEntity> userPlansAfter = context.Athletes.ToList().SelectMany(x => x.TrainingPlans).ToList();

            Assert.True(plansBefore.SequenceEqual(plansAfter));
            Assert.True(userPlansBefore.SequenceEqual(userPlansAfter));
        }


        [Fact]
        public async Task DeleteTrainingPlanCommand_TransactionFail()
        {
            GymContext context;
            ILogger<FakeDeleteTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeDeleteTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            TrainingPlanRoot src = planRepository.Find(id).Clone() as TrainingPlanRoot;

            DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
            FakeDeleteTrainingPlanCommandHandler handler = new FakeDeleteTrainingPlanCommandHandler(
                workoutRepository, planRepository, logger);

            Assert.False(await handler.Handle(command, default));

            // Check
            foreach (uint workoutId in src.WorkoutIds.Select(x => x.Value))
                Assert.Contains(workoutId, context.WorkoutTemplates.Select(x => x.Id.Value));

            Assert.NotNull(planRepository.Find(id));
            Assert.Contains(id, context.TrainingPlans.Select(x => x.Id.Value));
        }


        [Fact]
        public async Task WriteTrainingPlanNoteCommand_TransactionStep1Fail()
        {
            GymContext context;
            ILogger<FakeWriteTrainingPlanNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeWriteTrainingPlanNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            // Test
            uint userId = 1;
            uint planId = 1;
            string note = "my short note";

            var athletes = new SQLAthleteRepository(context);
            var notes = new SQLTrainingPlanNoteRepository(context);
            var notesBefore = context.TrainingPlanNotes.ToList();
            var userNoteBefore = athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId;

            WriteTrainingPlanNoteCommand command = new WriteTrainingPlanNoteCommand(userId, planId, note);
            FakeWriteTrainingPlanNoteCommandHandler handler = new FakeWriteTrainingPlanNoteCommandHandler(athletes, notes, logger, true, false);

            Assert.False(await handler.Handle(command, default));

            // Check step1 falied
            var notesAfter = context.TrainingPlanNotes.ToList();
            Assert.Equal(notesBefore.Count, notesAfter.Count);

            // Check step2 failed
            var userNoteAfter = athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId;
            Assert.Equal(userNoteBefore, userNoteAfter);
        }


        [Fact]
        public async Task WriteTrainingPlanNoteCommand_TransactionStep2Fail()
        {
            GymContext context;
            ILogger<FakeWriteTrainingPlanNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeWriteTrainingPlanNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            // Test
            uint userId = 1;
            uint planId = 1;
            string note = "my short note";

            var athletes = new SQLAthleteRepository(context);
            var notes = new SQLTrainingPlanNoteRepository(context);
            var notesBefore = context.TrainingPlanNotes.ToList();
            var userNoteBefore = athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId;

            WriteTrainingPlanNoteCommand command = new WriteTrainingPlanNoteCommand(userId, planId, note);
            FakeWriteTrainingPlanNoteCommandHandler handler = new FakeWriteTrainingPlanNoteCommandHandler(athletes, notes, logger, false, true);

            Assert.False(await handler.Handle(command, default));

            // Check step1 falied
            var notesAfter = context.TrainingPlanNotes.ToList();
            Assert.Equal(notesBefore.Count, notesAfter.Count);

            // Check step2 failed
            var userNoteAfter = athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId;
            Assert.Equal(userNoteBefore, userNoteAfter);
        }


        [Fact]
        public async Task WriteWorkUnitTemplateNoteCommand_TransactionStep1Fail()
        {
            GymContext context;
            ILogger<FakeWriteWorkUnitTemplateNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeWriteWorkUnitTemplateNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var workouts = new SQLWorkoutTemplateRepository(context);
            var notes = new SQLWorkUnitTemplateNoteRepository(context);

            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            string note = "my short work unit note!";

            var notesBefore = context.WorkUnitTemplateNotes.ToList();
            var wunitNoteBefore = workouts.Find(workoutId).CloneWorkUnit(workUnitPnum).WorkUnitNoteId;

            WriteWorkUnitTemplateNoteCommand command = new WriteWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, note);
            FakeWriteWorkUnitTemplateNoteCommandHandler handler = new FakeWriteWorkUnitTemplateNoteCommandHandler(workouts, notes, logger, true, false);

            Assert.False(await handler.Handle(command, default));

            // Check note added
            var notesAfter = context.WorkUnitTemplateNotes.ToList();
            Assert.Equal(notesBefore.Count, notesAfter.Count);

            // Check note attached to plan
            var wunitNoteAfter = workouts.Find(workoutId).CloneWorkUnit(workUnitPnum).WorkUnitNoteId;
            Assert.Equal(wunitNoteBefore, wunitNoteAfter);
        }


        [Fact]
        public async Task WriteWorkUnitTemplateNoteCommand_TransactionStep2Fail()
        {
            GymContext context;
            ILogger<FakeWriteWorkUnitTemplateNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<FakeWriteWorkUnitTemplateNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var workouts = new SQLWorkoutTemplateRepository(context);
            var notes = new SQLWorkUnitTemplateNoteRepository(context);

            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            string note = "my short work unit note!";

            var notesBefore = context.WorkUnitTemplateNotes.ToList();
            var wunitNoteBefore = workouts.Find(workoutId).CloneWorkUnit(workUnitPnum).WorkUnitNoteId;

            WriteWorkUnitTemplateNoteCommand command = new WriteWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, note);
            FakeWriteWorkUnitTemplateNoteCommandHandler handler = new FakeWriteWorkUnitTemplateNoteCommandHandler(workouts, notes, logger, false, true);

            Assert.False(await handler.Handle(command, default));

            // Check note added
            var notesAfter = context.WorkUnitTemplateNotes.ToList();
            Assert.Equal(notesBefore.Count, notesAfter.Count);

            // Check note attached to plan
            var wunitNoteAfter = workouts.Find(workoutId).CloneWorkUnit(workUnitPnum).WorkUnitNoteId;
            Assert.Equal(wunitNoteBefore, wunitNoteAfter);
        }



        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_LastOne_TransactionFail()
        {
            GymContext context;
            ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ExcludeTrainingPlanFromUserLibraryCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);
            var plans = new SQLTrainingPlanRepository(context);

            // Test
            uint athleteId = 1;
            uint planId = 1;        // This plan is owned by this user only

            ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
            ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

            Assert.False(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(athleteId);
            Assert.Contains(planId, modified.TrainingPlans.Select(x => x.TrainingPlanId));
            Assert.Contains(planId, context.TrainingPlans.Select(x => x.Id));
        }


    }
}
