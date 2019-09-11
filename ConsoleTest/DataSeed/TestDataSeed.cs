using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Infrastructure.Persistence.EFContext;
using System.Linq;

namespace ConsoleTest.DataSeed
{
    internal static class TestDataSeed
    {



        /// <summary>
        /// Perform a query which has no result if the Test Environment has not been properly set up
        /// </summary>
        /// <returns>True if Data Seeding is suggested</returns>
        internal static bool IsSeedingRequired()
        {
            using (GymContext context = new GymContext())
            {
                // Perform a query which has no result if the Test Environment has not been properly set up
                TrainingProficiencyRoot entry = context.TrainingProficiencies.SingleOrDefault(x => x.Id == 1);

                return entry == null;
            }
        }


        internal static void WorkUnitNoteDataSeed()
        {
            using (GymContext context = new GymContext())
            {
                //EntryStatusTypeEnum native = context.EntryStatusTypes.Single(x => x.Id == EntryStatusTypeEnum.Native.Id);

                WorkUnitTemplateNoteRoot entry1 = WorkUnitTemplateNoteRoot.WriteTransient(PersonalNoteValue.Write("Note 1"));
                context.Add(entry1);
                context.SaveChanges();
            }
        }


        internal static void HashtagDataSeed()
        {
            using (GymContext context = new GymContext())
            {
                EntryStatusTypeEnum native = context.EntryStatusTypes.Single(x => x.Id == EntryStatusTypeEnum.Native.Id);

                TrainingHashtagRoot entry1 = TrainingHashtagRoot.TagWith(null, GenericHashtagValue.TagWith("Fitness"), native);
                context.Add(entry1);
                context.SaveChanges();

                TrainingHashtagRoot entry2 = TrainingHashtagRoot.TagWith(null, GenericHashtagValue.TagWith("Dummy"), native);
                context.Add(entry2);
                context.SaveChanges();

                TrainingHashtagRoot entry3 = TrainingHashtagRoot.TagWith(null, GenericHashtagValue.TagWith("Italian"), native);
                context.Add(entry3);
                context.SaveChanges();
            }
        }


        internal static void ProficiencyDataSeed()
        {
            using (GymContext context = new GymContext())
            {
                EntryStatusTypeEnum native = context.EntryStatusTypes.Single(x => x.Id == EntryStatusTypeEnum.Native.Id);

                TrainingProficiencyRoot entry1 = TrainingProficiencyRoot.CreateTrainingProficiency(1, "Newcomer", PersonalNoteValue.Write("dummy"), native);
                context.Add(entry1);
                context.SaveChanges();

                TrainingProficiencyRoot entry2 = TrainingProficiencyRoot.CreateTrainingProficiency(2, "Beginner", PersonalNoteValue.Write("dummy"), native);
                context.Add(entry2);
                context.SaveChanges();

                TrainingProficiencyRoot entry3 = TrainingProficiencyRoot.CreateTrainingProficiency(3, "Intermediate", PersonalNoteValue.Write("dummy"), native);
                context.Add(entry3);
                context.SaveChanges();

                TrainingProficiencyRoot entry4 = TrainingProficiencyRoot.CreateTrainingProficiency(4, "Advanced", PersonalNoteValue.Write("dummy"), native);
                context.Add(entry4);
                context.SaveChanges();
            }
        }


    }
}
