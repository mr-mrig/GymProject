using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class OwnerEntity : Entity<IdTypeValue>
    {


        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfileImage { get; private set; }


        private OwnerEntity(string username, string imageProfileUrl) : base(null)
        {
            Username = UsernameValue.Register(username);
            ProfileImage = ProfilePictureValue.Link(UrlValue.CreateLink(imageProfileUrl));
        }


        public static OwnerEntity Register(string username, string imageprofile)
        {
            return new OwnerEntity(username, imageprofile);
        }

    }
}
