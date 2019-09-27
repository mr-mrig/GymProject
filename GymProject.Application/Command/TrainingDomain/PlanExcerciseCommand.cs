using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanExcerciseCommand : IRequest<bool>
    {






        public PlanExcerciseCommand(uint ownerId)
        {
            
        }



    }
}
