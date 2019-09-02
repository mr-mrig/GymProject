using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class LikeEntity : ChangeTrackingEntity<IdTypeValue>, ICloneable
    {


        public AuthorEntity LikeAuthor { get; private set; }



        protected LikeEntity(IdTypeValue id, AuthorEntity author, DateTime? createdOn = null) : base(id)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Like with no author");

            Id = id;
            LikeAuthor = author;
            CreatedOn = createdOn ?? DateTime.Now;
            //LastUpdate = lastUpdate;    // Makes no sense for Likes
        }



        #region Factory Method

        public static LikeEntity Give(IdTypeValue id, AuthorEntity author)
        {
            return new LikeEntity(id, author);
        }


        public static LikeEntity Copy(IdTypeValue id, AuthorEntity author, DateTime createdOn)
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
