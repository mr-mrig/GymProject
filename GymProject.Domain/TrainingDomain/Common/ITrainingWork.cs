namespace GymProject.Domain.TrainingDomain.Common
{
    public interface ITrainingWork
    {

        /// <summary>
        /// The progressive number of the training unit of work
        /// </summary>
        uint ProgressiveNumber { get; }
    }
}
