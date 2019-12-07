﻿using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithMuscleFocusCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint MuscleId { get; private set; }




        public TagTrainingPlanWithMuscleFocusCommand(uint athleteId, uint trainingPlanId, uint muscleId)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            MuscleId = muscleId;
        }

    }
}
