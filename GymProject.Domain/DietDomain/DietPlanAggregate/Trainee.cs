using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class Trainee : Entity<IdTypeValue>
    {


        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfilePicture { get; private set; }


        private Trainee(string username, string profilePictureUrl)
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
        public static Trainee Register(string username, string profilePictureUrl) => new Trainee(username, profilePictureUrl);
        #endregion

    }
}
