using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkoutReferenceEntityConfiguration : IEntityTypeConfiguration<WorkoutTemplateReferenceValue>
    {


        public void Configure(EntityTypeBuilder<WorkoutTemplateReferenceValue> builder)
        {
            builder.ToTable("WorkoutTemplate", GymContext.DefaultSchema)
                .HasKey("_id");

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
