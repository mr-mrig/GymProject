using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.TrainingDomain;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{

    public class TrainingDomainValidatorsTest
    {

        [Fact]
        public void WriteWorkUnitTemplateNoteCommandValidator_Success()
        {
            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            string note = "my short work unit note!";

            WriteWorkUnitTemplateNoteCommand command = new WriteWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, note);

            var loggerValidator = new Mock<ILogger<WriteWorkUnitTemplateNoteCommandValidator>>();
            var validator = new WriteWorkUnitTemplateNoteCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }

        
        [Fact]
        public void WriteWorkUnitTemplateNoteCommandValidator_Fail()
        {
            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            string note = ".".PadRight(PersonalNoteValue.DefaultMaximumLength);

            WriteWorkUnitTemplateNoteCommand command = new WriteWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, note);

            var loggerValidator = new Mock<ILogger<WriteWorkUnitTemplateNoteCommandValidator>>();
            var validator = new WriteWorkUnitTemplateNoteCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }


        [Fact]
        public void PlanTrainingWeekCommandValidator_Success()
        {
            // Test
            uint id = 1;
            uint weekTypeId = 1;

            PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);

            var loggerValidator = new Mock<ILogger<PlanTrainingWeekCommandValidator>>();
            PlanTrainingWeekCommandValidator validator = new PlanTrainingWeekCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }
        

        [Fact]
        public void PlanTrainingWeekCommandValidator_Fail()
        {
            // Test
            uint id = 1;
            uint weekTypeId = 100;

            PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);

            var loggerValidator = new Mock<ILogger<PlanTrainingWeekCommandValidator>>();
            PlanTrainingWeekCommandValidator validator = new PlanTrainingWeekCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }
        

        [Fact]
        public void ProvideTrainingScheduleFeedbackCommandValidator_Success()
        {
            // Test
            ProvideTrainingScheduleFeedbackCommand command = new ProvideTrainingScheduleFeedbackCommand(1, 1, 5, null);

            var loggerValidator = new Mock<ILogger<ProvideTrainingScheduleFeedbackCommandValidator>>();
            ProvideTrainingScheduleFeedbackCommandValidator validator = new ProvideTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }        


        [Fact]
        public void ProvideTrainingScheduleFeedbackCommandValidator_NullRating_Success()
        {
            // Test
            ProvideTrainingScheduleFeedbackCommand command = new ProvideTrainingScheduleFeedbackCommand(1, 1, null, null);

            var loggerValidator = new Mock<ILogger<ProvideTrainingScheduleFeedbackCommandValidator>>();
            ProvideTrainingScheduleFeedbackCommandValidator validator = new ProvideTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }
        

        [Fact]
        public void ProvideTrainingScheduleFeedbackCommandValidator_InvalidRating_Fail()
        {
            // Test
            ProvideTrainingScheduleFeedbackCommand command = new ProvideTrainingScheduleFeedbackCommand(1, 1, 5.1f, "my comment");

            var loggerValidator = new Mock<ILogger<ProvideTrainingScheduleFeedbackCommandValidator>>();
            ProvideTrainingScheduleFeedbackCommandValidator validator = new ProvideTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }    

        [Fact]
        public void ProvideTrainingScheduleFeedbackCommandValidator_InvalidComment_Fail()
        {
            // Test
            ProvideTrainingScheduleFeedbackCommand command = new ProvideTrainingScheduleFeedbackCommand(1, 1, 1, "a".PadRight(PersonalNoteValue.DefaultMaximumLength + 1));

            var loggerValidator = new Mock<ILogger<ProvideTrainingScheduleFeedbackCommandValidator>>();
            ProvideTrainingScheduleFeedbackCommandValidator validator = new ProvideTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }
        

        [Fact]
        public void ChangeTrainingScheduleFeedbackCommandValidator_Success()
        {
            // Test
            ChangeTrainingScheduleFeedbackCommand command = new ChangeTrainingScheduleFeedbackCommand(1, 1, 5, null);

            var loggerValidator = new Mock<ILogger<ChangeTrainingScheduleFeedbackCommandValidator>>();
            ChangeTrainingScheduleFeedbackCommandValidator validator = new ChangeTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }        


        [Fact]
        public void ChangeTrainingScheduleFeedbackCommandValidator_NullRating_Success()
        {
            // Test
            ChangeTrainingScheduleFeedbackCommand command = new ChangeTrainingScheduleFeedbackCommand(1, 1, null, null);

            var loggerValidator = new Mock<ILogger<ChangeTrainingScheduleFeedbackCommandValidator>>();
            ChangeTrainingScheduleFeedbackCommandValidator validator = new ChangeTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }
        

        [Fact]
        public void ChangeTrainingScheduleFeedbackCommandValidator_InvalidRating_Fail()
        {
            // Test
            ChangeTrainingScheduleFeedbackCommand command = new ChangeTrainingScheduleFeedbackCommand(1, 1, 5.1f, "my comment");

            var loggerValidator = new Mock<ILogger<ChangeTrainingScheduleFeedbackCommandValidator>>();
            ChangeTrainingScheduleFeedbackCommandValidator validator = new ChangeTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }    

        [Fact]
        public void ChangeTrainingScheduleFeedbackCommandValidator_InvalidComment_Fail()
        {
            // Test
            ChangeTrainingScheduleFeedbackCommand command = new ChangeTrainingScheduleFeedbackCommand(1, 1, 1, "a".PadRight(PersonalNoteValue.DefaultMaximumLength + 1));

            var loggerValidator = new Mock<ILogger<ChangeTrainingScheduleFeedbackCommandValidator>>();
            ChangeTrainingScheduleFeedbackCommandValidator validator = new ChangeTrainingScheduleFeedbackCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }

        [Fact]
        public void ScheduleTrainingPlanCommandValidator_TodayDate_Success()
        {
            // Test
            CompleteTrainingScheduleCommand command = new CompleteTrainingScheduleCommand(1, DateTime.UtcNow.Date);

            var loggerValidator = new Mock<ILogger<CompleteTrainingScheduleCommandValidator>>();
            CompleteTrainingScheduleCommandValidator validator = new CompleteTrainingScheduleCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }

        [Fact]
        public void ScheduleTrainingPlanCommandValidator_PastDate_Success()
        {
            // Test
            CompleteTrainingScheduleCommand command = new CompleteTrainingScheduleCommand(1, DateTime.UtcNow.AddDays(-1).Date);

            var loggerValidator = new Mock<ILogger<CompleteTrainingScheduleCommandValidator>>();
            CompleteTrainingScheduleCommandValidator validator = new CompleteTrainingScheduleCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }

        [Fact]
        public void ScheduleTrainingPlanCommandValidator_TomorrowDate_fail()
        {
            // Test
            CompleteTrainingScheduleCommand command = new CompleteTrainingScheduleCommand(1, DateTime.UtcNow.AddDays(1).Date);

            var loggerValidator = new Mock<ILogger<CompleteTrainingScheduleCommandValidator>>();
            CompleteTrainingScheduleCommandValidator validator = new CompleteTrainingScheduleCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }

        [Fact]
        public void RecheduleTrainingPlanCommandValidator_TodayDate_Success()
        {
            // Test
            RescheduleTrainingPlanCommand command = new RescheduleTrainingPlanCommand(1, DateTime.UtcNow.AddDays(-1).Date);

            var loggerValidator = new Mock<ILogger<RescheduleTrainingPlanCommandValidator>>();
            RescheduleTrainingPlanCommandValidator validator = new RescheduleTrainingPlanCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);
        }

        [Fact]
        public void RecheduleTrainingPlanCommandValidator_PastDate_Fail()
        {
            // Test
            RescheduleTrainingPlanCommand command = new RescheduleTrainingPlanCommand(1, DateTime.UtcNow.AddDays(-1).Date);

            var loggerValidator = new Mock<ILogger<RescheduleTrainingPlanCommandValidator>>();
            RescheduleTrainingPlanCommandValidator validator = new RescheduleTrainingPlanCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }

    }
}
