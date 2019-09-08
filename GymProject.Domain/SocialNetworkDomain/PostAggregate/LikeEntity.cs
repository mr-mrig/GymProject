using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class LikeEntity : ChangeTrackingEntity<uint?>, ICloneable
    {


        public AuthorEntity LikeAuthor { get; private set; }



        protected LikeEntity(uint? id, AuthorEntity author, DateTime? createdOn = null) : base(id)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Like with no author");

            Id = id;
            LikeAuthor = author;
            CreatedOn = createdOn ?? DateTime.Now;
            //LastUpdate = lastUpdate;    // Makes no sense for Likes
        }



        #region Factory Method

        public static LikeEntity Give(uint? id, AuthorEntity author)
        {
            return new LikeEntity(id, author);
        }


        public static LikeEntity Copy(uint? id, AuthorEntity author, DateTime createdOn)
        {
            return new LikeEntity(id, author, createdOn);
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => Copy(Id, LikeAuthor, CreatedOn);

        #endregion


    }
}
