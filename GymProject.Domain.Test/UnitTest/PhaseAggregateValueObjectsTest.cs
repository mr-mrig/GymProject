using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class PhaseAggregateValueObjectsTest
    {



        [Fact]
        public void DescriptiveNameValid()
        {
            int minLength = 1, maxLength = 50;
            string validName = "ThisIsValid";

            DescriptiveNameValue name = DescriptiveNameValue.Write(validName, minLength, maxLength);

            Assert.Equal(validName, name.Name);
            Assert.Equal(11, name.Length());
        }


        [Fact]
        public void DescriptiveNameLengthInvalid()
        {
            int minLength = 2, maxLength = 5;
            string fake = "ThisIsValid";

            Assert.Throws<ValueObjectInvariantViolationException>(() => DescriptiveNameValue.Write(fake, minLength, maxLength));

            fake = "a";

            Assert.Throws<ValueObjectInvariantViolationException>(() => DescriptiveNameValue.Write(fake, minLength, maxLength));
        }


        [Fact]
        public void DescriptiveNameSpacesValid()
        {
            int minLength = 1, maxLength = 50;
            string validName = "This Is Valid";

            DescriptiveNameValue name = DescriptiveNameValue.Write(validName, minLength, maxLength);

            Assert.Equal(validName, name.Name);
            Assert.Equal(13, name.Length());
        }


        [Fact]
        public void DescriptiveNameHyphenUnderscoreValid()
        {
            int minLength = 1, maxLength = 50;
            string fake = "This_Is-Valid";

            DescriptiveNameValue name = DescriptiveNameValue.Write(fake, minLength, maxLength);

            Assert.Equal(fake, name.Name);
            Assert.Equal(13, name.Length());
        }


        [Fact]
        public void DescriptiveNamePunctuationFail()
        {
            int minLength = 1, maxLength = 50;
            string fake = "This Is.Valid";

            Assert.Throws<ValueObjectInvariantViolationException>(() => DescriptiveNameValue.Write(fake, minLength, maxLength));

            fake = "This Is,Valid";
            Assert.Throws<ValueObjectInvariantViolationException>(() => DescriptiveNameValue.Write(fake, minLength, maxLength));
        }


        [Fact]
        public void DescriptiveNameNullFail()
        {
            int minLength = 1, maxLength = 50;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DescriptiveNameValue.Write(null, minLength, maxLength));
        }
    }
}
