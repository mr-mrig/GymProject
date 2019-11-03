using GymProject.Domain.SharedKernel;

namespace GymProject.Application.Validator.StaticRule
{
    public static class GenericHashtagRule
    {

        public static bool IsValidHashtag(string hashtag)

            => !hashtag.Contains("#") && !hashtag.Contains(" ")
                && hashtag.Length >= GenericHashtagValue.DefaultMinimumLength
                && hashtag.Length <= GenericHashtagValue.DefaultMaximumLength;
    }
}
