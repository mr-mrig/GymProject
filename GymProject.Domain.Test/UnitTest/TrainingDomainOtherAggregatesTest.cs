using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingDomainOtherAggregatesTest
    {



        public const int ntests = 500;




        [Fact]
        public void TrainingHashtagRootFullTest()
        {
            TrainingHashtagRoot trainingHashtag;

            for (int itest = 0; itest < ntests; itest++)
            {
                uint? id = (uint?)(RandomFieldGenerator.RandomInt(1, 99999));

                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.2f);
                bool setStatus = RandomFieldGenerator.RollEventWithProbability(0.5f);
                bool isFake = RandomFieldGenerator.RollEventWithProbability(0.3f);      // The only fake cases are the ones of the parent class and the Hashtag Value Object

                EntryStatusTypeEnum status = EntryStatusTypeEnum.From(
                    RandomFieldGenerator.RandomInt(0, 5));

                GenericHashtagValue hashtag = GenericHashtagValue.TagWith(
                    RandomFieldGenerator.RandomTextValue(GenericHashtagValue.DefaultMinimumLength, GenericHashtagValue.DefaultMaximumLength));


                if (isTransient)
                {
                    trainingHashtag = TrainingHashtagRoot.TagWith(hashtag);
                    Assert.Null(trainingHashtag.Id);
                    Assert.Equal(EntryStatusTypeEnum.Pending, trainingHashtag.EntryStatus);
                }
                else
                {
                    if (setStatus)
                    {
                        trainingHashtag = TrainingHashtagRoot.TagWith(id, hashtag, status);
                        Assert.Equal(status, trainingHashtag.EntryStatus);
                    }
                    else
                    {
                        trainingHashtag = TrainingHashtagRoot.TagWith(id, hashtag);
                        Assert.Equal(EntryStatusTypeEnum.Pending, trainingHashtag.EntryStatus);
                    }
                    Assert.Equal(id, trainingHashtag.Id);
                }

                status = EntryStatusTypeEnum.From(RandomFieldGenerator.RandomInt(0, 5));

                if (status == null)
                    Assert.Throws<InvalidOperationException>(() => trainingHashtag.ChangeEntryStatus(status));
                else
                {
                    trainingHashtag.ChangeEntryStatus(status);
                    Assert.Equal(status, trainingHashtag.EntryStatus);

                    trainingHashtag.ModerateEntryStatus(status);
                    Assert.Equal(status, trainingHashtag.EntryStatus);
                }
            }

        }

    }
}
