using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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


            builder.OwnsOne(s => s.ScheduledPeriod, sp =>
            {
                //// Shadow property - Workoround to build Indexes with values from Owned Entities, See the fllowing HasIndex
                //sp.Property<uint>("TrainingPlanId")
                //    .HasColumnName("TrainingPlanId")
                //    .IsRequired()
                //    .HasColumnType("INTEGER");

                sp.Property(p => p.Start)
                    .HasColumnName("StartDate")
                    .HasColumnType("INTEGER")
                    .IsRequired(true);

                sp.Property(p => p.End)
                    .HasColumnName("EndDate")
                    .HasColumnType("INTEGER")
                    .IsRequired(false);

                // DOES NOT WORK
                //sp.HasIndex(
                //    "Start",
                //    "TrainingPlanId"
                //);
            });

            builder.HasIndex(s => s.TrainingPlanId)
                .HasName("IX_TrainingSchedule_TrainingPlanId");


            //builder.HasIndex(s => new
            //{
            //    s.ScheduledPeriod.Start,
            //    s.TrainingPlanId
            //});
        }

    }
}
