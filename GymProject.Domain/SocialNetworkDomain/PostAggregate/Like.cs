using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Like : ChangeTrackingEntity<IdType>
    {


        public Author LikeAuthor { get; private set; }



        protected Like(Author author)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Like with no author");


            LikeAuthor = author;
            CreatedOn = DateTime.Now;
        }



        #region Factory Method

        public static Like Give(Author author)
        {
            return new Like(author);
        }

        #endregion



    }
}
