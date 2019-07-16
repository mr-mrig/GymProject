using GymProject.Domain.SharedKernel;
using GymProject.Domain.FitnessJournalDomain.Common;
using GymProject.Domain.FitnessJournalDomain.Exceptions;
using GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate;
using GymProject.Domain.Test.Util;
using System;
using System.Collections;
using System.Linq;
using Xunit;



namespace GymProject.Domain.Test.UnitTest
{
    public class FitnessAggregateTest
    {


        [Fact]
        public void CreateEmptyFitnessDayEntrySuccess()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);



            //Assert.NotNull(post);
            //Assert.Equal(caption, post.Caption);
            //Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            //Assert.Equal(UsernameValue.Register(username), post.PostAuthor.Username);
            //Assert.Equal(ProfilePictureValue.Link(UrlValue.CreateLink(profilePicUrl)), post.PostAuthor.ProfileImage);
            //Assert.Null(post.LastUpdate);
            //StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }


        [Fact]
        public void CheckWeightFormat()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);



            //Assert.NotNull(post);
            //Assert.Equal(caption, post.Caption);
            //Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            //Assert.Equal(UsernameValue.Register(username), post.PostAuthor.Username);
            //Assert.Equal(ProfilePictureValue.Link(UrlValue.CreateLink(profilePicUrl)), post.PostAuthor.ProfileImage);
            //Assert.Null(post.LastUpdate);
            //StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }


        [Fact]
        public void CheckKilogramsToPoundsConversion()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);



            //Assert.NotNull(post);
            //Assert.Equal(caption, post.Caption);
            //Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            //Assert.Equal(UsernameValue.Register(username), post.PostAuthor.Username);
            //Assert.Equal(ProfilePictureValue.Link(UrlValue.CreateLink(profilePicUrl)), post.PostAuthor.ProfileImage);
            //Assert.Null(post.LastUpdate);
            //StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }


        [Fact]
        public void CheckPoundsToKilogramsConversion()
        {
            string username = "user1";
            string profilePicUrl = "./Images/test.jpg";
            Author auth = Author.Register(username, profilePicUrl);



            //Assert.NotNull(post);
            //Assert.Equal(caption, post.Caption);
            //Assert.Equal(SharingPolicyEnum.Public, post.SharingPolicy);
            //Assert.Equal(UsernameValue.Register(username), post.PostAuthor.Username);
            //Assert.Equal(ProfilePictureValue.Link(UrlValue.CreateLink(profilePicUrl)), post.PostAuthor.ProfileImage);
            //Assert.Null(post.LastUpdate);
            //StaticUtils.CheckCurrentTimestamp(post.CreatedOn);
        }

    }
}
