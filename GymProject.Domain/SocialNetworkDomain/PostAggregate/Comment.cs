﻿using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Comment : ChangeTrackingEntity<IdType>
    {


        public string Body { get; private set; }

        public Author CommentAuthor { get; private set; }



        protected Comment(Author author, string body)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Comment with no author");


            CommentAuthor = author;
            Body = body;
            CreatedOn = DateTime.Now;
        }



        #region Factory Method

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="author">The author of the comment</param>
        /// <param name="comment">The body of the comment</param>
        /// <returns>The nre Comment instance</returns>
        public static Comment Write(Author author, string comment)
        {
            return new Comment(author, comment);
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

    }
}
