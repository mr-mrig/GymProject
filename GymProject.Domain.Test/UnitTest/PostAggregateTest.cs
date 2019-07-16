using GymProject.Domain.SharedKernel;
using GymProject.Domain.SocialNetworkDomain.Common;
using GymProject.Domain.SocialNetworkDomain.Exceptions;
using GymProject.Domain.SocialNetworkDomain.PostAggregate;
using GymProject.Domain.Test.Util;
using System;
using System.Collections;
using System.Linq;
using Xunit;


namespace GymProject.Domain.Test.UnitTest
{
    public class PostAggregateTest
    {


        public PostAggregateTest()
        {

        }


        [Fact]
        public void CreatePublicPostSuccess()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Public);

            Assert.NotNull(post);
            Assert.Equal(caption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            Assert.Equal(UsernameValue.Register(username), post.PostAuthor.Username);
            Assert.Equal(ProfilePictureValue.Link(UrlValue.CreateLink(profilePicUrl)), post.PostAuthor.ProfileImage);
            Assert.Null(post.LastUpdate);
            StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }


        [Fact]
        public void CreatePrivatePostSuccess()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Private);

            Assert.NotNull(post);
            Assert.Equal(caption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Private, post.SharingPolicy);
            Assert.Equal(UsernameValue.Register(username), post.PostAuthor.Username);
            Assert.Equal(ProfilePictureValue.Link(UrlValue.CreateLink(profilePicUrl)), post.PostAuthor.ProfileImage);
            Assert.Null(post.LastUpdate);
            StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }


        [Fact]
        public void CreatePostWithImage()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";

            string attachedPicUrl = "./Images/attachemnt.jpg";
            Picture pic = Picture.Attach(attachedPicUrl);

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Public, pic);

            Assert.NotNull(post);
            Assert.Equal(caption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            Assert.Equal(auth, post.PostAuthor);
            Assert.Equal(pic, post.AttachedPicture);
            Assert.Null(post.LastUpdate);
            StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }


        [Fact]
        public void CreatePublicPostFail()
        {
            Author auth = null;
            string caption = "My comment.";

            Assert.Throws<ArgumentNullException>(() => Post.Write(auth, caption, SharingPolicyEnum.Public));
        }


        [Fact]
        public void PrivateToPublic()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";

            string attachedPicUrl = "./Images/attachemnt.jpg";
            Picture pic = Picture.Attach(attachedPicUrl);

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Public, pic);

            post.MakePrivate();

            Assert.NotNull(post);
            Assert.Equal(caption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Private, post.SharingPolicy);
            Assert.Equal(auth, post.PostAuthor);
            Assert.Equal(pic, post.AttachedPicture);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void PublicToPrivate()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";

            string attachedPicUrl = "./Images/attachemnt.jpg";
            Picture pic = Picture.Attach(attachedPicUrl);

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Private, pic);

            post.MakePublic();

            Assert.NotNull(post);
            Assert.Equal(caption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            Assert.Equal(auth, post.PostAuthor);
            Assert.Equal(pic, post.AttachedPicture);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void PublicToPublic()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";

            string attachedPicUrl = "./Images/attachemnt.jpg";
            Picture pic = Picture.Attach(attachedPicUrl);

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Public, pic);

            post.MakePublic();

            Assert.NotNull(post);
            Assert.Equal(caption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            Assert.Equal(auth, post.PostAuthor);
            Assert.Equal(pic, post.AttachedPicture);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void ChangeCaption()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);

            string caption = "My comment.";
            string newCaption = "Changed caption.";

            string attachedPicUrl = "./Images/attachemnt.jpg";
            Picture pic = Picture.Attach(attachedPicUrl);

            Post post = Post.Write(auth, caption, SharingPolicyEnum.Public, pic);
            post.ModifyCaption(newCaption);


            Assert.NotNull(post);
            Assert.Equal(newCaption, post.Caption);
            Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            Assert.Equal(auth, post.PostAuthor);
            Assert.Equal(pic, post.AttachedPicture);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void AddComments()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author commentAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";
            string comment2 = "Another comment.";

            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddComment(commentAuth, comment2);

            Assert.Equal(comment, post.Comments.FirstOrDefault().Body);
            Assert.Equal(comment2, post.Comments.Where((x, i) => i == 1).Select(x => x.Body).FirstOrDefault());
            Assert.Equal(2, post.Comments.Count);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void ChangeComment()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author commentAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";
            string comment2 = "Another comment.";
            string newComment = "Changed comment.";

            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddComment(commentAuth, comment2);

            post.ModifyComment(post.Comments.FirstOrDefault(), newComment);

            Assert.Equal(newComment, post.Comments.FirstOrDefault().Body);
            Assert.Equal(comment2, post.Comments.Where((x, i) => i == 1).Select(x => x.Body).FirstOrDefault());
            Assert.Equal(2, post.Comments.Count);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
            StaticUtils.CheckCurrentTimestamp(post.Comments.FirstOrDefault().LastUpdate.Value);
            Assert.Null(post.Comments.ToList()[1].LastUpdate);
        }


        [Fact]
        public void RemoveComment()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author commentAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";
            string comment2 = "Another comment.";

            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddComment(commentAuth, comment2);

            post.RemoveComment(post.Comments.FirstOrDefault());

            Assert.Equal(comment2, post.Comments.FirstOrDefault().Body);
            Assert.Equal(1, post.Comments.Count);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void CheckInvariants()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author commentAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";
            string comment2 = "Another comment.";

            // 2 comments, 2 likes
            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddComment(commentAuth, comment2);
            post.AddLike(commentAuth);
            post.AddLike(postAuth);

            // Trying to remove a comment from outside the aggregate
            post.Comments.ToList().Remove(post.Comments.FirstOrDefault());

            Assert.Equal(comment, post.Comments.FirstOrDefault().Body);
            Assert.Equal(comment2, post.Comments.Where((x, i) => i == 1).Select(x => x.Body).FirstOrDefault());
            Assert.Equal(2, post.Comments.Count);

            // Trying to remove a like from outside the aggregate
            post.Likes.ToList().Remove(post.Likes.FirstOrDefault());

            Assert.Equal(commentAuth, post.Likes.Select(x => x.LikeAuthor).FirstOrDefault());
            Assert.Equal(postAuth, post.Likes.Where((x, i) => i == 1).Select(x => x.LikeAuthor).FirstOrDefault());
            Assert.Equal(2, post.Comments.Count);
        }


        [Fact]
        public void AddLikes()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author likeAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";

            // 1 comment, 2 likes
            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddLike(likeAuth);
            post.AddLike(postAuth);

            Assert.Equal(likeAuth, post.Likes.Select(x => x.LikeAuthor).FirstOrDefault());
            Assert.Equal(postAuth, post.Likes.Where((x, i) => i == 1).Select(x => x.LikeAuthor).FirstOrDefault());
            Assert.Equal(2, post.Likes.Count);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void RemoveLikeByAuthor()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author likeAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";

            // 1 comment, 2 likes
            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddLike(likeAuth);
            post.AddLike(postAuth);

            post.Unlike(postAuth);

            Assert.Equal(likeAuth, post.Likes.Select(x => x.LikeAuthor).FirstOrDefault());
            Assert.Equal(1, post.Likes.Count);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }


        [Fact]
        public void RemoveLike()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author postAuth = Author.Register(username, profilePicUrl);

            string username2 = "user2";
            string profilePicUrl2 = "./Images/test2.jpg";
            Author likeAuth = Author.Register(username2, profilePicUrl2);

            string caption = "My caption.";
            string comment = "My comment.";

            // 1 comment, 2 likes
            Post post = Post.Write(postAuth, caption, SharingPolicyEnum.Public);
            post.AddComment(postAuth, comment);
            post.AddLike(likeAuth);
            post.AddLike(postAuth);

            post.Unlike(post.Likes.ToList().FirstOrDefault());

            Assert.Equal(postAuth, post.Likes.Select(x => x.LikeAuthor).FirstOrDefault());
            Assert.Equal(1, post.Likes.Count);
            StaticUtils.CheckCurrentTimestamp(post.LastUpdate.Value);
        }




        #region Support Functions


        #endregion

    }
}
