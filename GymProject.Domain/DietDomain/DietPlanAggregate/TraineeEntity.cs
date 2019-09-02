using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class TraineeEntity : Entity<IdTypeValue>
    {


        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfilePicture { get; private set; }


        private TraineeEntity(string username, string profilePictureUrl) : base(null)
        {
            Username = UsernameValue.Register(username);
            ProfilePicture = ProfilePictureValue.Link(UrlValue.CreateLink(profilePictureUrl));
        }



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="profilePictureUrl">The profile picture URL</param>
        /// <returns>A new Trainee instance</returns>
        public static TraineeEntity Register(string username, string profilePictureUrl) => new TraineeEntity(username, profilePictureUrl);
        #endregion

    }
}
