using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class CreateTrainingPhaseCommand : IRequest<bool>
    {


        //public uint OwnerId { get; private set; }
        public uint EntryStatusId { get; private set; }
        public string PhaseName { get; private set; }




        public CreateTrainingPhaseCommand(uint entryStatusId, string phaseName)
        {
            EntryStatusId = entryStatusId;
            PhaseName = phaseName;
        }

    }
}
