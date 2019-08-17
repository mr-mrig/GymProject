using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Like : ChangeTrackingEntity<IdType>, ICloneable
    {


        public Author LikeAuthor { get; private set; }



        protected Like(IdType id, Author author, DateTime? createdOn = null)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Like with no author");

            Id = id;
            LikeAuthor = author;
            CreatedOn = createdOn ?? DateTime.Now;
            //LastUpdate = lastUpdate;    // Makes no sense for Likes
        }



        #region Factory Method

        public static Like Give(IdType id, Author author)
        {
            return new Like(id, author);
        }


        public static Like Copy(IdType id, Author author, DateTime createdOn)
        {
            return new Like(id, author, createdOn);
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => Copy(Id, LikeAuthor, CreatedOn);

        #endregion


    }
}
