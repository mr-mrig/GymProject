using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingWeekEntityConfiguration : IEntityTypeConfiguration<TrainingWeekEntity>
    {


        public void Configure(EntityTypeBuilder<TrainingWeekEntity> builder)
        {
            builder.ToTable("TrainingWeek", GymContext.DefaultSchema);

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(w => w.DomainEvents)
                .Ignore(w => w.TrainingVolume)
                .Ignore(w => w.TrainingIntensity)
                .Ignore(w => w.TrainingDensity)
                .Ignore(w => w.Workouts);

            builder.Property(w => w.ProgressiveNumber)
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.HasOne(w => w.TrainingWeekType)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(w => w.Workouts)
                .WithOne()
                .HasForeignKey("TrainingWeekId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasAlternateKey
            (
                "TrainingPlanId",
                "ProgressiveNumber"
            );

            //builder.HasIndex          // Redundant??
            //(
            //    "TrainingPlanId",
            //    "ProgressiveNumber"
            //);
        }

    }
}
