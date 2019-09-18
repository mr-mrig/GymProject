using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
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
                .Ignore(w => w.WorkoutIds);

            builder.Property(w => w.ProgressiveNumber)
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.HasOne(w => w.TrainingWeekType)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property<uint>("TrainingPlanId")
                .IsRequired();

            //builder.HasMany<WorkoutTemplateRoot>()
            //    .WithOne()
            //    .HasForeignKey(w => w.Id)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);

            //var navigation = builder.Metadata.FindNavigation(nameof(TrainingWeekEntity.WorkoutIds));
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);


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
