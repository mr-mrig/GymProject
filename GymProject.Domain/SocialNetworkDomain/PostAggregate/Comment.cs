using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Comment : ChangeTrackingEntity<IdTypeValue>, ICloneable
    {


        public string Body { get; private set; }

        public Author CommentAuthor { get; private set; }



        #region Ctors

        protected Comment(IdTypeValue id, Author author, string body, DateTime? createdOn = null, DateTime? lastUpdate = null)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Comment with no author");

            Id = id;
            CommentAuthor = author;
            Body = body;
            CreatedOn = createdOn ?? DateTime.Now;
            LastUpdate = lastUpdate;
        }

        #endregion



        #region Factory Method

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="author">The author of the comment</param>
        /// <param name="comment">The body of the comment</param>
        /// <param name="id">The Id of the Comment</param>
        /// <returns>The Comment instance</returns>
        public static Comment Write(IdTypeValue id, Author author, string comment)
        {
            return new Comment(id, author, comment);
        }


        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="author">The author of the comment</param>
        /// <param name="comment">The body of the comment</param>
        /// <param name="id">The Id of the Comment</param>
        /// <param name="createdOn">The date which the comment has been created on</param>
        /// <param name="lastUpdate">The date which the comment has been lastly updated on</param>
        /// <returns>The Comment instance</returns>
        public static Comment Copy(IdTypeValue id, Author author, string comment, DateTime createdOn, DateTime lastUpdate)
        {
            return new Comment(id, author, comment, (DateTime?)createdOn, (DateTime?)lastUpdate);
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Change the comment caption
        /// </summary>
        /// <param name="newComment">The new caption</param>
        internal void ModifyComment(string newComment)
        {
            Body = newComment;
            LastUpdate = DateTime.Now;
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => Copy(Id, CommentAuthor, Body, CreatedOn, LastUpdate.Value);

        #endregion
    }
}
