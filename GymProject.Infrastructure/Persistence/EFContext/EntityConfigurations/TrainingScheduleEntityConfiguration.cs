using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingScheduleEntityConfiguration : IEntityTypeConfiguration<TrainingScheduleRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingScheduleRoot> builder)
        {
            builder.ToTable("TrainingSchedule", GymContext.DefaultSchema);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();


            //builder.Ignore(s => s.DomainEvents);


            builder.OwnsOne(s => s.ScheduledPeriod, sp =>
            {
                sp.Property(p => p.Start)
                    .HasColumnName("StartDate")
                    .IsRequired();

                sp.Property(p => p.End)
                    .HasColumnName("EndDate");
            });


            //builder.HasOne<TrainingPlanRoot>()
            //    .WithMany()
            //    .HasForeignKey(x => x.TrainingPlanId)
            //    .IsRequired();
        }

    }
}
