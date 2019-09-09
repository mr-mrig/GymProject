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
        public static void IntensityTechniqueFail()
        {
            for (int itest = 0; itest < ntests; itest++)
            {
                string name = RandomFieldGenerator.RandomTextValue(1, 20);
                string abbreviation = RandomFieldGenerator.RandomTextValue(2, 5);
                bool isLinkingTechnique = RandomFieldGenerator.RollEventWithProbability(0.5f);
                uint? ownerId = (uint?)(RandomFieldGenerator.RandomInt(1, 999999));
                PersonalNoteValue description = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

                float constructorProbability = RandomFieldGenerator.RandomFloat(0, 1);

                switch(constructorProbability)
                {
                    case var _ when constructorProbability < 0.33f:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, null, name, abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, ownerId, "     ", abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreatePublicIntensityTechnique(null, ownerId, null, abbreviation, description, isLinkingTechnique));
                        break;

                    case var _ when constructorProbability < 0.33f:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreateNativeIntensityTechnique(null, null, name, abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreateNativeIntensityTechnique(null, ownerId, "     ", abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreateNativeIntensityTechnique(null, ownerId, null, abbreviation, description, isLinkingTechnique));
                        break;

                    default:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreatePrivateIntensityTechnique(null, null, name, abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreatePrivateIntensityTechnique(null, ownerId, "     ", abbreviation, description, isLinkingTechnique));
                        Assert.Throws<TrainingDomainInvariantViolationException>(() => IntensityTechniqueRoot.CreatePrivateIntensityTechnique(null, ownerId, null, abbreviation, description, isLinkingTechnique));
                        break;
                }

                uint? id = 1;

                IntensityTechniqueRoot tech = IntensityTechniqueRoot.CreateNativeIntensityTechnique(id, ownerId, name, abbreviation, description, isLinkingTechnique);
                Assert.Throws<InvalidOperationException>(() => tech.ModerateEntryStatus(null));
                Assert.Throws<InvalidOperationException>(() => tech.ChangeEntryStatus(null));
            }
        }



        [Fact]
        public static void IntensityTechniqueFullTest()
        {
            IntensityTechniqueRoot technique;

            for (int itest = 0; itest < ntests; itest++)
            {
                string name = RandomFieldGenerator.RandomTextValue(1, 20);
                string abbreviation = RandomFieldGenerator.RandomTextValue(2, 5);
                bool isLinkingTechnique = RandomFieldGenerator.RollEventWithProbability(0.5f);
                uint? ownerId = (uint?)(RandomFieldGenerator.RandomInt(1, 999999));
                PersonalNoteValue description = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));
                uint? id = (uint?)(RandomFieldGenerator.RandomInt(1, 999999));

                // Pad with spaces
                if(RandomFieldGenerator.RollEventWithProbability(0.05f))
                {
                    int leftSpacesNumber = RandomFieldGenerator.RandomInt(1, 5);
                    int rightSpacesNumber = RandomFieldGenerator.RandomInt(1, 5);

                    name = abbreviation.PadLeft(name.Length + leftSpacesNumber, ' ').PadRight(name.Length + rightSpacesNumber + leftSpacesNumber, ' ');
                    abbreviation = abbreviation.PadLeft(abbreviation.Length + leftSpacesNumber, ' ').PadRight(abbreviation.Length + rightSpacesNumber + leftSpacesNumber, ' ');

                    if (description.Length() + leftSpacesNumber + rightSpacesNumber < PersonalNoteValue.DefaultMaximumLength)
                        description = PersonalNoteValue.Write(
                            description.Body.PadLeft(description.Length() + leftSpacesNumber, ' ').PadRight(description.Length() + rightSpacesNumber + leftSpacesNumber, ' '));
                }

                float constructorProbability = RandomFieldGenerator.RandomFloat(0, 1);

                switch (constructorProbability)
                {
                    case var _ when constructorProbability < 0.33f:

                        technique = IntensityTechniqueRoot.CreatePublicIntensityTechnique(id, ownerId, name, abbreviation, description, isLinkingTechnique);
                        break;

                    case var _ when constructorProbability < 0.33f:

                        technique = IntensityTechniqueRoot.CreateNativeIntensityTechnique(id, ownerId, name, abbreviation, description, isLinkingTechnique);
                        break;

                    default:

                        technique = IntensityTechniqueRoot.CreatePrivateIntensityTechnique(id, ownerId, name, abbreviation, description, isLinkingTechnique);
                        break;
                }

                Assert.Equal(id, technique.Id);
                Assert.Equal(ownerId, technique.OwnerId);
                Assert.Equal(name.Trim(), technique.Name);
                Assert.Equal(abbreviation.Trim(), technique.Abbreviation);
                Assert.Equal(description, technique.Description);
                Assert.Equal(isLinkingTechnique, technique.IsLinkingTechnique);

                technique.AttachDescription(null);
                Assert.Null(technique.Description);

                description = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));
                technique.AttachDescription(null);
                technique.AttachDescription(description);
                Assert.Equal(description, technique.Description);

                EntryStatusTypeEnum newStatus = RandomFieldGenerator.ChooseAmong(
                    EntryStatusTypeEnum.List().Except(new List<EntryStatusTypeEnum>() { null }));

                technique.ChangeEntryStatus(newStatus);

                Assert.Equal(newStatus, technique.EntryStatus);
            }
        }






        #region Support Methods

        #endregion

    }
}