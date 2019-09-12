using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkoutReferenceEntityConfiguration : IEntityTypeConfiguration<WorkoutTemplateReferenceEntity>
    {


        public void Configure(EntityTypeBuilder<WorkoutTemplateReferenceEntity> builder)
        {
            builder.ToTable("WorkoutTemplate", GymContext.DefaultSchema);

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            // Build relation to the other POCO which builds the same table
            builder.HasOne<WorkoutTemplateRoot>()
                .WithOne()
                .HasForeignKey<WorkoutTemplateRoot>(w => w.Id);

            builder.Ignore(wo => wo.WorkingSets);

            builder.Property(wo => wo.ProgressiveNumber);


            builder.HasAlternateKey
            (
                "TrainingWeekId",
                "ProgressiveNumber"
            );

            //builder.HasIndex              // Redundant?
            //(
            //    "TrainingWeekId",
            //    "ProgressiveNumber"
            //);
        }

    }
}
