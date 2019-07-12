using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using GymProject.Domain.SocialNetworkDomain.Exceptions;
using System.Linq;


namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Post : ChangeTrackingEntity, IAggregateRoot
    {


        public string Caption { get; private set; }

        public SharingPolicy IsShared { get; private set; }

        private List<Comment> _comments;

        private List<Like> _likes;


        #region External Bounded Context reference

        public Author PostAuthor { get; private set; }
        #endregion




        private Post(Author author, string caption, SharingPolicy isShared)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Post with no author");

            PostAuthor = author;
            Caption = caption;
            IsShared = isShared;
            CreatedOn = DateTime.Now;

            _comments = new List<Comment>();

            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            //var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
            //                                                          cardNumber, cardSecurityNumber,
            //                                                          cardHolderName, cardExpiration);

            //AddDomainEvent(orderStartedDomainEvent);
        }



        #region Factory Method

        public static Post Write(Author author, string caption, SharingPolicy isShared)
        {
            return new Post(author, caption, isShared);
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Change the Post caption
        /// </summary>
        public void ModifyCaption(string newCaption)
        {
            Caption = newCaption;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Mark the Post as Private
        /// </summary>
        public void MakePrivate()
        {
            IsShared = SharingPolicy.Private;
        }

        /// <summary>
        /// Mark the Post as Public
        /// </summary>
        public void MakePublic()
        {
            IsShared = SharingPolicy.Public;
        }

        /// <summary>
        /// Add a comment to the Post
        /// </summary>
        /// <param name="author">The comment author</param>
        /// <param name="commentText">The comment</param>
        public void AddComment(Author author, string commentText)
        {
            _comments.Add(Comment.Write(author, commentText));

            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Modify the Post comment
        /// </summary>
        /// <param name="author">The comment author</param>
        /// <param name="newCommentText">The comment</param>
        public void ModifyComment(Comment comment, string newCommentText)
        {
            if(_comments.Contains(comment))
            {
                _comments[0] = null;

                //_comments.
            }
            else
                throw new SocialNetworkGenericException($"The selected Comment - Id={comment.Id.ToString()} - could not be found in this Post - Id={Id.ToString()}");


            LastUpdate = DateTime.Now;
        }

        ///// <summary>
        ///// Modify the Post comment
        ///// </summary>
        ///// <param name="author">The comment author</param>
        ///// <param name="newCommentText">The comment</param>
        //public void ModerateComment(Comment comment, Moderator moderator)
        //{
        //    _comments.Add(Comment.Write(author, commentText));

        //    LastUpdate = DateTime.Now;
        //}

        ///// <summary>
        ///// Add a like to the Post
        ///// </summary>
        ///// <param name="author">The author of the like</param>
        //public void AddLike(Author author)
        //{
        //    _likes.Add(Comment.Write(author, commentText));

        //    LastUpdate = DateTime.Now;
        //}
        #endregion



        #region Private Methods

        #endregion

    }
}
