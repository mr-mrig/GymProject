using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SocialNetworkDomain.Common;

namespace GymProject.Domain.SocialNetworkDomain.ImageAggregate
{
    public class PictureEntity : Entity<uint?>, IAggregateRoot
    {

        /// <summary>
        /// Url to the image
        /// </summary>
        public UrlValue ImageUrl { get; private set; }

        /// <summary>
        /// Type of the picture
        /// </summary>
        public PictureTypeEnum PictureType { get; private set; }

        /// <summary>
        /// FK to the Post
        /// </summary>
        public long PostId { get; private set; }



        #region Ctors

        private PictureEntity(UrlValue imageUrl, PictureTypeEnum pictureType, long postId) : base(null)
        {
            ImageUrl = imageUrl;
            PictureType = pictureType;
            PostId = postId;
        }

        private PictureEntity(string imageUrl, PictureTypeEnum pictureType, long postId) : base(null)
        {
            ImageUrl = UrlValue.CreateLink(imageUrl);
            PictureType = pictureType;
            PostId = postId;
        }
        #endregion


        #region Factories

        /// <summary>
        /// Create a new Picture object
        /// </summary>
        /// <param name="url">Image url</param>
        /// <param name="picType">Picture type</param>
        /// <returns></returns>
        public static PictureEntity Attach(UrlValue url, PictureTypeEnum picType, long postId)
        {
            return new PictureEntity(url, picType, postId);
        }

        /// <summary>
        /// Create a new Picture object
        /// </summary>
        /// <param name="url">Image url</param>
        /// <param name="picType">Picture type</param>
        /// <returns></returns>
        public static PictureEntity Attach(string url, PictureTypeEnum picType, long postId)
        {
            return new PictureEntity(url, picType, postId);
        }

        /// <summary>
        /// Create a new Progress Picture object
        /// </summary>
        /// <param name="url">Image url</param>
        /// <returns></returns>
        public static PictureEntity AttachProgreesPicture(string url, long postId)
        {
            return new PictureEntity(url, PictureTypeEnum.ProgressPic, postId);
        }


        /// <summary>
        /// Create a new Progress Picture object
        /// </summary>
        /// <param name="url">Image url</param>
        /// <returns></returns>
        public static PictureEntity AttachProgreesPicture(UrlValue url, long postId)
        {
            return new PictureEntity(url, PictureTypeEnum.ProgressPic, postId);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Change the picture type
        /// </summary>
        /// <param name="newType">The new picture type</param>
        public void ChangePictureType(PictureTypeEnum newType)
        {
            PictureType = newType;
        }

        /// <summary>
        /// Change the picture type to Progress Picture
        /// </summary>
        public void MakeProgressPicture()
        {
            PictureType = PictureTypeEnum.ProgressPic;
        }
        #endregion

    }
}
