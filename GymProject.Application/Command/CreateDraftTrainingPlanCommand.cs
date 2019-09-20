﻿using MediatR;

namespace GymProject.Application.Command
{

    public class CreateDraftTrainingPlanCommand : IRequest<bool>
    {


        public uint OwnerId { get; private set; }



        public CreateDraftTrainingPlanCommand()
        {

        }


        public CreateDraftTrainingPlanCommand(uint ownerId) : this()
        {
            OwnerId = ownerId;
        }

    }
}
