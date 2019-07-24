using GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate;
using GymProject.Domain.SocialNetworkDomain.Common;
using GymProject.Domain.SocialNetworkDomain.PostAggregate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymProject.Test.Console
{

    //[Test]
    class Program
    {
        static void Main(string[] args)
        {


            FitnessDay fd = FitnessDay.StartTrackingDay(DateTime.Today, null);

            fd.TrackWellnessDay(TemperatureValue.MeasureCelsius(37.56f), GlycemiaValue.MeasureMg(110.7f));

            DailyWellnessValue wd = fd.DailyWellness;

            //wd.TrackTemperature(TemperatureValue.MeasureCelsius(11));

            Debug.WriteLine("");



            Post p = Post.Write(Author.Register("example", "picUrl"), "my caption", SharingPolicyEnum.Public);
            p.AddComment(Author.Register("another", "picUrl2"), "very nice comment for you!");

            List<Comment> comments = p.Comments.ToList();
            //comments[0].ModifyComment("changed!");

            comments.Add(Comment.Write(Author.Register("example", "picUrl"), "ciao"));

            if(p.Comments.FirstOrDefault().Body.Equals(comments[0].Body))
            {
                // Error
                System.Diagnostics.Debugger.Break();
            }

            if (p.Comments.Count == comments.Count)
            {
                // Error
                System.Diagnostics.Debugger.Break();
            }





            System.Diagnostics.Debugger.Break();
        }
    }
}
