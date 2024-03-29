﻿using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using GymProject.Domain.SocialNetworkDomain.Exceptions;
using System.Linq;
using GymProject.Domain.SocialNetworkDomain.Common;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Post : ChangeTrackingEntity, IAggregateRoot
    {


        /// <summary>
        /// Post caption
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Post sharing policy
        /// </summary>
        public SharingPolicyEnum IsShared { get; private set; }

        /// <summary>
        /// List of comments to the Post
        /// </summary>
        private ICollection<Comment> _comments;

        /// <summary>
        /// List of Likes to the Post
        /// </summary>
        private ICollection<Like> _likes;



        #region External Bounded Context references

        /// <summary>
        /// Author of the Post
        /// </summary>
        public Author PostAuthor { get; private set; }

        /// <summary>
        /// Attached Picture
        /// </summary>
        public Picture AttachedPicture { get; private set; }
        #endregion




        private Post(Author author, string caption, SharingPolicyEnum isShared)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Post with no author");

            PostAuthor = author;
            Caption = caption;
            IsShared = isShared;
            CreatedOn = DateTime.Now;

            _comments = new List<Comment>();
            _likes = new List<Like>();

            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            //var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
            //                                                          cardNumber, cardSecurityNumber,
            //                                                          cardHolderName, cardExpiration);

            //AddDomainEvent(orderStartedDomainEvent);
        }



        #region Factory Method

        public static Post Write(Author author, string caption, SharingPolicyEnum isShared)
        {
            return new Post(author, caption, isShared);
        }

        #endregion


        #region Business Methods - Comments

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
            IsShared = SharingPolicyEnum.Private;
        }

        /// <summary>
        /// Mark the Post as Public
        /// </summary>
        public void MakePublic()
        {
            IsShared = SharingPolicyEnum.Public;
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
        /// Modify the comment belonging to the Post - Author.
        /// </summary>
        /// <param name="toBeModified">The comment to be modified</param>
        /// <param name="newCommentText">The comment text</param>
        public void ModifyComment(Comment toBeModified, string newCommentText)
        {
            // Check for comment not found
            Comment srcComment = _comments.Where(x => x.Equals(toBeModified)).FirstOrDefault();

            srcComment = EditComment(srcComment, newCommentText);

            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Modify the comment belonging to the Post - Author
        /// </summary>
        /// <param name="toBeModified">The comment to be modified</param>
        /// <param name="newCommentText">The comment text</param>
        public void ModifyComment(long commentId, string newCommentText)
        {
            // Check for comment not found
            Comment srcComment = _comments.Where(x => x.Id == commentId).FirstOrDefault();

            srcComment = EditComment(srcComment, newCommentText);

            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Moderate the comment belonging to the Post - Moderator
        /// </summary>
        /// <param name="comment">The comment to be moderated</param>
        /// <param name="moderator">The moderator who performed the action</param>
        public void ModerateComment(Comment toBeModerated, Moderator moderator)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Remove the comment belonging to the Post - Author
        /// </summary>
        /// <param name="comment">The comment to be moderated</param>
        public void RemoveComment(Comment toBeRemoved)
        {
            // Check for Comment not found
            Comment srcComment = _comments.Where(x => x.Equals(toBeRemoved)).FirstOrDefault();

            DeleteComment(toBeRemoved);

            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Get the comments
        /// </summary>
        /// <returns>The read only collection of the comments</returns>
        public IReadOnlyCollection<Comment>GetComments()
        {
            return _comments.ToList().AsReadOnly();
        }
        #endregion


        #region Business Methods - Likes

        /// <summary>
        /// Add a like to the Post
        /// </summary>
        /// <param name="author">The author of the like</param>
        public void AddLike(Author author)
        {
            _likes.Add(Like.Give(author));

            LastUpdate = DateTime.Now;
        }


        /// <summary>
        /// Unlike the Post
        /// </summary>
        /// <param name="author">The author of the like to be removed</param>
        public void Unlike(Author author)
        {
            Like toBeRemoved = _likes.Where(x => x.LikeAuthor.Equals(author)).FirstOrDefault();

            // Check for Like not found
            if (!_likes.Remove(toBeRemoved))
                throw new SocialNetworkGenericException($"The selected Like - author={author.Username.Name} - could not be found in this Post - Id={Id.ToString()}");


            LastUpdate = DateTime.Now;
        }


        /// <summary>
        /// Unlike the Post
        /// </summary>
        /// <param name="likeId">The Id of the like to be removed</param>
        public void Unlike(Like toBeRemoved)
        {
            Like srcLike = _likes.Where(x => x.Equals(toBeRemoved) ).FirstOrDefault();

            // Check for Like not found
            if (!_likes.Remove(srcLike))
                throw new SocialNetworkGenericException($"The selected Like - Id={toBeRemoved.Id.ToString()} - could not be found in this Post - Id={Id.ToString()}");


            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Get the likes
        /// </summary>
        /// <returns>The read only collection of the likes</returns>
        public IReadOnlyCollection<Like> GetLikes()
        {
            return _likes.ToList().AsReadOnly();
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Modifies the Comment body
        /// </summary>
        /// <param name="srcComment"></param>
        /// <param name="commentText"></param>
        ///<exception cref="SocialNetworkGenericException">If comment not found among the Post ones</exception>
        /// <returns></returns>
        private Comment EditComment(Comment srcComment, string commentText)
        {
            if (srcComment == default(Comment))
                throw new SocialNetworkGenericException($"The selected Comment - Id={srcComment.Id.ToString()} - could not be found in this Post - Id={Id.ToString()}");

            else
                srcComment.ModifyComment(commentText);

            return srcComment;
        }


        /// <summary>
        /// Removed the specified Comment
        /// </summary>
        /// <param name="srcComment"></param>
        ///<exception cref="SocialNetworkGenericException">If comment not found among the Post ones</exception>
        /// <returns></returns>
        private void DeleteComment(Comment toBeRemoved)
        {
            if(!_comments.Remove(toBeRemoved))
                throw new SocialNetworkGenericException($"The selected Comment - Id={toBeRemoved.Id.ToString()} - could not be found in this Post - Id={Id.ToString()}");
        }
        #endregion

    }
}
