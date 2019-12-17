using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.TrainingDomain;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{

    public class TrainingDomainValidatorsTest
    {

        [Fact]
        public void WriteWorkUnitTemplateNoteCommand_ValidatorSuccess()
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
        public void WriteWorkUnitTemplateNoteCommand_ValidatorFail()
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
        public void PlanTrainingWeekCommand_ValidatorSuccess()
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
        public void PlanTrainingWeekCommand_ValidatorFail()
        {
            // Test
            uint id = 1;
            uint weekTypeId = 100;

            PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);

            var loggerValidator = new Mock<ILogger<PlanTrainingWeekCommandValidator>>();
            PlanTrainingWeekCommandValidator validator = new PlanTrainingWeekCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }


    }
}
