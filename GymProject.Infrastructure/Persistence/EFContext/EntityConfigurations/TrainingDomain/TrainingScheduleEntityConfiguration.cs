using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingScheduleEntityConfiguration : IEntityTypeConfiguration<TrainingScheduleRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingScheduleRoot> builder)
        {
            builder.ToTable("TrainingSchedule", GymContext.DefaultSchema);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(s => s.DomainEvents);

            //builder.OwnsOne(s => s.ScheduledPeriod, sp =>
            //{
            //    //// Shadow property - Workoround to build Indexes with values from Owned Entities, See the fllowing HasIndex
            //    //sp.Property<uint>("TrainingPlanId")
            //    //    .HasColumnName("TrainingPlanId")
            //    //    .IsRequired()
            //    //    .HasColumnType("INTEGER");

            //    sp.Property(p => p.Start)
            //        .HasColumnName("StartDate")
            //        .HasColumnType("INTEGER")
            //        .IsRequired(true);

            //    sp.Property(p => p.End)
            //        .HasColumnName("EndDate")
            //        .HasColumnType("INTEGER")
            //        .IsRequired(false);

            //    // DOES NOT WORK
            //    //sp.HasIndex(
            //    //    "Start",
            //    //    "TrainingPlanId"
            //    //);
            //});
            builder.Property(s => s.StartDate)
                    .HasColumnType("INTEGER")
                    .IsRequired(true);
            builder.Property(s => s.EndDate)
                    .HasColumnType("INTEGER")
                    .IsRequired(false);

            builder.HasMany(s => s.Feedbacks)
                .WithOne()
                .HasForeignKey("TrainingScheduleId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(TrainingScheduleRoot.Feedbacks));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<TrainingPlanRoot>()
                .WithMany()
                .HasForeignKey(x => x.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<AthleteRoot>()
                .WithMany()
                .HasForeignKey(x => x.AthleteId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new
            {
                x.TrainingPlanId,
                x.AthleteId,
                x.StartDate
            }).IsUnique();
            // GetCurrentScheduleByAthleteOrDefault queries for the EndDate. Should we add a specific index?


            //builder.HasIndex(s => s.TrainingPlanId)
            //    .HasName("IX_TrainingSchedule_TrainingPlanId");


            //builder.HasIndex(s => new
            //{
            //    s.ScheduledPeriod.Start,
            //    s.TrainingPlanId
            //});
        }

    }
}
