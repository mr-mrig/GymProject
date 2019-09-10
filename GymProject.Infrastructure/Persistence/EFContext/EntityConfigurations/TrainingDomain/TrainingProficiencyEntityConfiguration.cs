using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingProficiencyEntityConfiguration : IEntityTypeConfiguration<TrainingProficiencyRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingProficiencyRoot> builder)
        {
            builder.ToTable("TrainingProficiency", GymContext.DefaultSchema);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(p => p.DomainEvents);


            builder.OwnsOne(p => p.Description, pp =>
            {
                pp.Property(p => p.Body)
                    .HasColumnName("Description")
                    .HasColumnType("TEXT")
                    .HasMaxLength(PersonalNoteValue.DefaultMaximumLength)
                    .IsRequired();
            });
                //.HasData(GetTrainingProficiencyDescriptionNativeEntries());

            builder.HasOne(p => p.EntryStatus)
                .WithMany().IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasAlternateKey(p => p.Name);

            // Data Seeding
            //builder.HasData(GetTrainingProficiencyNativeEntries());
        }



        //private IEnumerable<TrainingProficiencyRoot> GetTrainingProficiencyNativeEntries()

        //    => new List<TrainingProficiencyRoot> ()
        //    {
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(1, "Newcomer", PersonalNoteValue.Write("Just stepped into the gym and/or very poor athletic capabilities")),
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(2, "Beginner", PersonalNoteValue.Write("Low training expirience and basic athletic capabilities")),
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(3, "Intermediate", PersonalNoteValue.Write("Intermediate training expirience in terms of time and skills")),
        //        TrainingProficiencyRoot.AddNativeTrainingProficiency(4, "Advanced", PersonalNoteValue.Write("High training expirience and solid skills")),
        //    };



        //private object[] GetTrainingProficiencyDescriptionNativeEntries()

        //    => new object[]
        //    {
        //        new
        //        {
        //            TrainingProficiencyRootId = 1,
        //            Body = "Just stepped into the gym and/or very poor athletic capabilities",
        //        },
        //        new
        //        {
        //            TrainingProficiencyRootId = 2,
        //            Body = "Low training expirience and basic athletic capabilities",
        //        },
        //        new
        //        {
        //            TrainingProficiencyRootId = 3,
        //            Body = "Intermediate training expirience in terms of time and skills",
        //        },
        //        new
        //        {
        //            TrainingProficiencyRootId = 4,
        //            Body = "High training expirience and solid skills",
        //        },
        //    };
    }
}
