using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class Owner : Entity<IdTypeValue>
    {


        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfilePicture { get; private set; }


        private Owner(string username, string profilePictureUrl) : base(null)
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
        /// <returns>A new Owner instance</returns>
        public static Owner Register(string username, string profilePictureUrl) => new Owner(username, profilePictureUrl);
        #endregion

    }
}
