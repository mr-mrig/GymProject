using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Picture : Entity<IdTypeValue>
    {


        public UrlValue PictureUrl { get; private set; }



        #region Ctors


        private Picture(UrlValue url)
        {
            PictureUrl = url;
        }


        private Picture(string url)
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
        public static Picture Attach(UrlValue url)
        {
            return new Picture(url);
        }

        /// <summary>
        /// Attach the Picutre to the Post
        /// </summary>
        /// <param name="url">The picture url</param>
        /// <returns>The Picture to be attached</returns>
        public static Picture Attach(string url)
        {
            return new Picture(url);
        }
        #endregion
    }
}
