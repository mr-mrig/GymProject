using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Test.Util;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace GymProject.Domain.Test.UnitTest
{
    public class CommonClassesTest
    {



        public const int ntests = 500;



        [Fact]
        public void TrainingHashtagRootFullTest()
        {
            float trimCaseChance = 0.25f;

            string validBody = RandomFieldGenerator.RandomTextValue(GenericHashtagValue.DefaultMinimumLength, GenericHashtagValue.DefaultMaximumLength);

            bool isFake = RandomFieldGenerator.RollEventWithProbability(0.4f);


            if (isFake)
            {
                // Hashtag too long
                Assert.Throws<ValueObjectInvariantViolationException>(()
                    => GenericHashtagValue.TagWith(RandomFieldGenerator.RandomTextValue(GenericHashtagValue.DefaultMaximumLength + 1, GenericHashtagValue.DefaultMaximumLength + 10)));

                // Hashtag too short
                if (GenericHashtagValue.DefaultMinimumLength > 0)
                {
                    Assert.Throws<ValueObjectInvariantViolationException>(()
                        => GenericHashtagValue.TagWith(RandomFieldGenerator.RandomTextValue(0, GenericHashtagValue.DefaultMinimumLength - 1)));
                }

                // Hashtag with spaces
                string fakeBody = validBody.Insert(
                    RandomFieldGenerator.RandomInt(1, validBody.Length - 1), " ");

                fakeBody = fakeBody.Length > GenericHashtagValue.DefaultMaximumLength ? fakeBody.Substring(0, fakeBody.Length) : fakeBody;

                Assert.Throws<ValueObjectInvariantViolationException>(() => GenericHashtagValue.TagWith(fakeBody));

                // Hashtag with delimiter
                fakeBody = validBody.Insert(
                    RandomFieldGenerator.RandomInt(1, validBody.Length - 1), GenericHashtagValue.HashtagDelimiter);

                fakeBody = fakeBody.Length > GenericHashtagValue.DefaultMaximumLength ? fakeBody.Substring(0, fakeBody.Length) : fakeBody;

                Assert.Throws<ValueObjectInvariantViolationException>(() => GenericHashtagValue.TagWith(fakeBody));
            }
            else
            {
                // Test trims
                if(RandomFieldGenerator.RollEventWithProbability(trimCaseChance))
                {
                    if (validBody.Length < GenericHashtagValue.DefaultMaximumLength)
                        validBody = validBody + " ";

                    validBody = " " + validBody;
                }

                GenericHashtagValue hashtag = GenericHashtagValue.TagWith(validBody);
                Assert.Equal(validBody.Trim(), hashtag.Body);
                Assert.Equal(validBody.Trim().Length, hashtag.Length());
                Assert.Equal(validBody.Trim().Length + 1, hashtag.FullLength());
                Assert.Equal(GenericHashtagValue.HashtagDelimiter + validBody.Trim(), hashtag.ToFullHashtag());


            }
        }


    }
}
