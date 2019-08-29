using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class IntensityTechniqueAggregateTest
    {



        public const int ntests = 1000;





        [Fact]
        public static void IntensityTechnqiueFail()
        {
            for (int itest = 0; itest < ntests; itest++)
            {
                string name = RandomFieldGenerator.RandomTextValue(1, 20);
                string abbreviation = RandomFieldGenerator.RandomTextValue(2, 5);
                bool isLinkingTechnique = RandomFieldGenerator.RollEventWithProbability(0.5f);
                IdTypeValue ownerId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 999999));
                PersonalNoteValue description = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

                float constructorProbability = RandomFieldGenerator.RandomFloat(0, 1);

                switch(constructorProbability)
                {
                    case var _ when constructorProbability < 0.33f:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreatePublicIntensityTechnique(null, null, name, abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreatePublicIntensityTechnique(null, ownerId, "     ", abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreatePublicIntensityTechnique(null, ownerId, null, abbreviation, description, isLinkingTechnique));
                        break;

                    case var _ when constructorProbability < 0.33f:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreateNativeIntensityTechnique(null, null, name, abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreateNativeIntensityTechnique(null, ownerId, "     ", abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreateNativeIntensityTechnique(null, ownerId, null, abbreviation, description, isLinkingTechnique));
                        break;

                    default:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreatePrivateIntensityTechnique(null, null, name, abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreatePrivateIntensityTechnique(null, ownerId, "     ", abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechnique.CreatePrivateIntensityTechnique(null, ownerId, null, abbreviation, description, isLinkingTechnique));
                        break;
                }

                IntensityTechnique tech = IntensityTechnique.CreateNativeIntensityTechnique();
            }
        }



        [Fact]
        public static void IntensityTechniqueFullTest()
        {

        }






        #region Support Methods

        #endregion

    }
}