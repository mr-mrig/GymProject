using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class UserTrainingPhaseEntityConfiguration : IEntityTypeConfiguration<UserTrainingPhaseRelation>
    {

        private string _thisTableName = "UserTrainingPhase";


        public void Configure(EntityTypeBuilder<UserTrainingPhaseRelation> builder)
        {
            builder.ToTable(_thisTableName, GymContext.DefaultSchema);
            builder.HasKey("UserId", "StartDate");

            builder.Property("UserId");
            builder.Property(rel => rel.PhaseId);

            builder.Property(rel => rel.StartDate)
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.Property(rel => rel.EndDate)
                .HasColumnType("INTEGER");

            builder.OwnsOne(p => p.OwnerNote, n =>
                n.Property(x => x.Body)
                    .HasColumnName("OwnerNote")
                    .HasMaxLength(PersonalNoteValue.DefaultMaximumLength)
            );

            builder.HasOne(e => e.EntryStatus)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex("UserId", "EndDate", "StartDate");


            //var ownedBuilder = builder.OwnsOne(p => p.Period, per =>
            //{
            //    per.Property(p => (DateTime)p.Start)
            //        .HasColumnName("StartDate")
            //        .HasColumnType("INTEGER")
            //        .IsRequired(true)
            //        .Metadata.IsNullable = false;


            //    per.Property(p => p.End)
            //        .HasColumnName("EndDate")
            //        .HasColumnType("INTEGER")
            //        .IsRequired(false);

            //    //per.HasKey("UserId", "Start");
            //    per.HasKey(p => (DateTime)p.Start);


            //    per.ToTable(_thisTableName);
            //    per.WithOwner().HasForeignKey(x => x.Start);
            //});
            //ownedBuilder.Metadata.FindOwnership().IsRequired = true;
            //ownedBuilder.Metadata.FindProperty("Start").IsNullable = false;

            builder.HasOne<TrainingPhaseRoot>()
                .WithMany()
                .HasForeignKey(tp => tp.PhaseId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            // Trying to make EF build the proper Key without adding superfluous navigation poperties, namely the AthleteId
            // The issue is that StartDate is not recognized at "compile-time", hence a new property with the same name must be defined
            // However, we cannot have properties with the same name and different attributes. Specifically we have:
            // 1. StartDate from the ValueObject which is Nullable
            // 2. StartDate defined below which is not
            // The solution would be to make the 1. not Nullable, but cannot find the way to do it...
            // Update: the only solution found so far is to make StartDate Key of the Owned Entity
            //         this has the drawback that the Parent Entity cannot declare a key different than StartDate, which is wrong

            //builder.Metadata.FindProperty("Period#DateRangeValue.Start").IsNullable = false;
            //builder.Property<DateTime?>("StartDate")
            //    .HasColumnType("INTEGER")
            //    .IsRequired();

            //builder.Metadata.FindProperty("StartDate").IsNullable = false;

            //builder.HasKey("StartDate");        // This is wrong - to be fixed via scripting
        }
    }
}
