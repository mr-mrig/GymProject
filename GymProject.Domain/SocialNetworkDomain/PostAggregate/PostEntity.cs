using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using GymProject.Domain.SocialNetworkDomain.Exceptions;
using System.Linq;
using GymProject.Domain.SocialNetworkDomain.Common;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Utils.Extensions;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class PostEntity : ChangeTrackingEntity<uint?>, IAggregateRoot, ICloneable
    {


        /// <summary>
        /// Post caption
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Post sharing policy
        /// </summary>
        public SharingPolicyEnum SharingPolicy { get; private set; }


        private ICollection<CommentEntity> _comments;

        /// <summary>
        /// List of comments to the Post
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<CommentEntity> Comments
        {
            get => _comments?.Clone().ToList().AsReadOnly() ?? new List<CommentEntity>().AsReadOnly();
            //get => _comments?.ToList().AsReadOnly();
        }

        private ICollection<LikeEntity> _likes;

        /// <summary>
        /// List of Likes to the Post
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<LikeEntity> Likes
        {
            get => _likes?.Clone().ToList().AsReadOnly() ?? new List<LikeEntity>().AsReadOnly();
            //get => _likes?.ToList().AsReadOnly();
        }


        /// <summary>
        /// Author of the Post
        /// </summary>
        public AuthorEntity PostAuthor { get; private set; }

        /// <summary>
        /// Attached Picture
        /// </summary>
        public PictureEntity AttachedPicture { get; private set; }




        #region Ctors

        private PostEntity(AuthorEntity author, string caption, SharingPolicyEnum isShared, PictureEntity attachedPicture = null, DateTime? createdOn = null, DateTime? lastUpdate = null) : base(null)
        {
            if (author == null)
                throw new ArgumentNullException("author", "Cannot create a Post with no author");

            PostAuthor = author;
            Caption = caption;
            SharingPolicy = isShared;
            AttachedPicture = attachedPicture;

            CreatedOn = createdOn ?? DateTime.Now;
            LastUpdate = lastUpdate;

            _comments = new List<CommentEntity>();
            _likes = new List<LikeEntity>();


            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            //var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
            //                                                          cardNumber, cardSecurityNumber,
            //                                                          cardHolderName, cardExpiration);

            //AddDomainEvent(orderStartedDomainEvent);
        }

        #endregion



        #region Factory Method

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="author">Post author</param>
        /// <param name="caption">Post caption</param>
        /// <param name="isShared">Sharing policy</param>
        /// <param name="attachedPicture">Picture to be attached - optional</param>
        /// <returns>A new Post instance</returns>
        public static PostEntity Write(AuthorEntity author, string caption, SharingPolicyEnum isShared, PictureEntity attachedPicture = null)
        {
            return new PostEntity(author, caption, isShared, attachedPicture);
        }


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="author">Post author</param>
        /// <param name="caption">Post caption</param>
        /// <param name="isShared">Sharing policy</param>
        /// <param name="attachedPicture">Picture to be attached - optional</param>
        /// <returns>A new Post instance</returns>
        public static PostEntity Copy(AuthorEntity author, string caption, SharingPolicyEnum isShared, DateTime createdOn, DateTime? lastUpdate, PictureEntity attachedPicture = null)
        {
            return new PostEntity(author, caption, isShared, attachedPicture, createdOn, lastUpdate);
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
            SharingPolicy = SharingPolicyEnum.Private;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Mark the Post as Public
        /// </summary>
        public void MakePublic()
        {
            SharingPolicy = SharingPolicyEnum.Public;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Mark the Post as Public
        /// </summary>
        public void ChangeSharingPolicy(SharingPolicyEnum newSharingPolicy)
        {
            SharingPolicy = newSharingPolicy;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Add a comment to the Post
        /// </summary>
        /// <param name="author">The comment author</param>
        /// <param name="commentText">The comment</param>
        public void AddComment(AuthorEntity author, string commentText)
        {
            _comments.Add(CommentEntity.Write(BuildCommentId(), author, commentText));

            LastUpdate = DateTime.Now;
        }

        ///// <summary>
        ///// Modify the comment belonging to the Post - Author.
        ///// </summary>
        ///// <param name="toBeModified">The comment to be modified</param>
        ///// <param name="newCommentText">The comment text</param>
        //public void ModifyComment(Comment toBeModified, string newCommentText)
        //{
        //    // Check for comment not found
        //    Comment srcComment = _comments.Where(x => x.Equals(toBeModified)).FirstOrDefault();

        //    srcComment = EditComment(srcComment, newCommentText);

        //    LastUpdate = DateTime.Now;
        //}

        /// <summary>
        /// Modify the comment belonging to the Post - Author
        /// </summary>
        /// <param name="commentId">The ID of the comment to be modified</param>
        /// <param name="newCommentText">The comment text</param>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        public void ModifyComment(uint? commentId, string newCommentText)
        {
            // Check for comment not found
            CommentEntity srcComment = FindCommentById(commentId);

            if (srcComment == default)
                throw new KeyNotFoundException($"No comment with Id {commentId.ToString()} in Post {Id.ToString()}");

            EditComment(srcComment, newCommentText);

            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Moderate the comment belonging to the Post - Moderator
        /// </summary>
        /// <param name="comment">The comment to be moderated</param>
        /// <param name="moderator">The moderator who performed the action</param>
        public void ModerateComment(CommentEntity toBeModerated, Moderator moderator)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Remove the comment belonging to the Post - Author
        /// </summary>
        /// <param name="id">The ID of the comment to be moderated</param>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        public void RemoveComment(uint? id)
        {
            // Check for Comment not found
            CommentEntity toBeRemoved = FindCommentById(id);

            DeleteComment(toBeRemoved);

            LastUpdate = DateTime.Now;
        }


        /// <summary>
        /// Find the Like with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The Like object/returns>
        public LikeEntity FindLikeById(uint? id)
        {
            if (id == null)
                throw new ArgumentNullException($"Cannot find a Like with NULL id");

            LikeEntity result = _likes.Where(x => x.Id == id).FirstOrDefault();

            if (result == default)
                throw new ArgumentException($"The Like with Id {id.ToString()} could not be found");

            return result;
        }


        /// <summary>
        /// Find the Like assigned to the selected user
        /// </summary>
        /// <param name="author">The author to be found</param>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The Like object/returns>
        public LikeEntity FindLikeByAuthor(AuthorEntity author)
        {
            if (author == null)
                throw new ArgumentNullException($"Cannot find a Like with NULL author");

            LikeEntity result = _likes.Where(x => x.LikeAuthor == author).FirstOrDefault();

            if (result == default)
                throw new ArgumentException($"The Like with AuthorId {author.ToString()} could not be found");

            return result;
        }


        /// <summary>
        /// Find the Comment with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The Comment object/returns>
        public CommentEntity FindCommentById(uint? id)
        {
            if (id == null)
                throw new ArgumentNullException($"Cannot find a Comment with NULL id");

            CommentEntity result = _comments.Where(x => x.Id == id).FirstOrDefault();

            if (result == default)
                throw new ArgumentException($"The Comment with Id {id.ToString()} could not be found");

            return result;
        }
        #endregion


        #region Business Methods - Likes

        /// <summary>
        /// Add a like to the Post
        /// </summary>
        /// <param name="author">The author of the like</param>
        public void AddLike(AuthorEntity author)
        {
            _likes.Add(LikeEntity.Give(BuildLikeId(), author));

            LastUpdate = DateTime.Now;
        }


        /// <summary>
        /// Unlike the Post
        /// </summary>
        /// <param name="author">The author of the like to be removed</param>
        public void Unlike(AuthorEntity author)
        {
            LikeEntity toBeRemoved = FindLikeByAuthor(author);

            // Check for Like not found
            if (!_likes.Remove(toBeRemoved))
                throw new SocialNetworkGenericException($"The selected Like - author={author.Username.Name} - could not be found in this Post - Id={Id.ToString()}");


            LastUpdate = DateTime.Now;
        }


        /// <summary>
        /// Unlike the Post
        /// </summary>
        /// <param name="likeId">The Id of the like to be removed</param>
        public void Unlike(uint? likeId)
        {
            LikeEntity srcLike = FindLikeById(likeId);

            // Check for Like not found
            if (!_likes.Remove(srcLike))
                throw new SocialNetworkGenericException($"The selected Like - Id={likeId.ToString()} - could not be found in this Post - Id={Id.ToString()}");


            LastUpdate = DateTime.Now;
        }

        #endregion



        #region Private Methods

        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The WS Id</returns>
        private uint? BuildLikeId()
        {
            if (_likes.Count == 0)
                return 1;

            else
                return _likes.Last().Id + 1;
        }


        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The WS Id</returns>
        private uint? BuildCommentId()
        {
            if (_comments.Count == 0)
                return 1;

            else
                return _comments.Last().Id + 1;
        }


        /// <summary>
        /// Modifies the Comment body
        /// </summary>
        /// <param name="srcComment"></param>
        /// <param name="commentText"></param>
        ///<exception cref="SocialNetworkGenericException">If comment not found among the Post ones</exception>
        /// <returns></returns>
        private void EditComment(CommentEntity srcComment, string commentText)
        {
            if (srcComment == default)
                throw new SocialNetworkGenericException($"The selected Comment - Id={srcComment.ToString()} - could not be found in this Post - Id={Id.ToString()}");

            else
                srcComment.ModifyComment(commentText);

            //return srcComment;
        }


        /// <summary>
        /// Removed the specified Comment
        /// </summary>
        /// <param name="srcComment"></param>
        ///<exception cref="SocialNetworkGenericException">If comment not found among the Post ones</exception>
        /// <returns></returns>
        private void DeleteComment(CommentEntity toBeRemoved)
        {
            if(!_comments.Remove(toBeRemoved))
                throw new SocialNetworkGenericException($"The selected Comment - Id={toBeRemoved.ToString()} - could not be found in this Post - Id={Id.ToString()}");
        }

        #endregion


        #region IClonable Implementation


        public object Clone()

            => Copy(PostAuthor, Caption, SharingPolicy, CreatedOn, LastUpdate, AttachedPicture);

        #endregion
    }
}
