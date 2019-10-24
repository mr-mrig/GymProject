using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AddWorkingSetIntensityTechniqueCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkingSetProgressiveNumber { get; private set; }
        public uint IntensityTechniqueId { get; private set; }




        public AddWorkingSetIntensityTechniqueCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber, uint intensityTechniqueId)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkingSetProgressiveNumber = workingSetProgressiveNumber;
            IntensityTechniqueId = intensityTechniqueId;
        }



    }
}
