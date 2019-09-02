using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class PictureEntity : Entity<IdTypeValue>
    {


        public UrlValue PictureUrl { get; private set; }



        #region Ctors


        private PictureEntity(UrlValue url) : base(null)
        {
            PictureUrl = url;
        }


        private PictureEntity(string url) : base(null)
        {
            PictureUrl = UrlValue.CreateLink(url);
        }
        #endregion


        #region Factories

        /// <summary>
        /// Attach the Picutre to the Post
        /// </summary>
        /// <param name="url">The picture url</param>
        /// <returns>The Picture to be attached</returns>
        public static PictureEntity Attach(UrlValue url)
        {
            return new PictureEntity(url);
        }

        /// <summary>
        /// Attach the Picutre to the Post
        /// </summary>
        /// <param name="url">The picture url</param>
        /// <returns>The Picture to be attached</returns>
        public static PictureEntity Attach(string url)
        {
            return new PictureEntity(url);
        }
        #endregion
    }
}
