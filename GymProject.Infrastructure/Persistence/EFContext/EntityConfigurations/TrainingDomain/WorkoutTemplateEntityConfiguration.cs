using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkoutTemplateEntityConfiguration : IEntityTypeConfiguration<WorkoutTemplateRoot>
    {


        public void Configure(EntityTypeBuilder<WorkoutTemplateRoot> builder)
        {
            builder.ToTable("WorkoutTemplate", GymContext.DefaultSchema);

            builder.HasKey(wo => wo.Id);

            builder.Property(wo => wo.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(wo => wo.DomainEvents);
            builder.Ignore(wo => wo.TrainingVolume);
            builder.Ignore(wo => wo.TrainingIntensity);
            builder.Ignore(wo => wo.TrainingDensity);

            builder.Property(wo => wo.Name)
                .HasColumnType("TEXT");

            builder.OwnsOne(wo => wo.SpecificWeekday,
                sw =>
                {
                    sw.Property(p => p.Id)
                        .IsRequired()
                        .HasColumnName("SpecificWeekday")
                        .HasDefaultValue(0);

                    sw.Ignore(p => p.Name)
                        .Ignore(p => p.Abbreviation);
                });

            builder.HasMany(wo => wo.WorkUnits)
                .WithOne()
                .HasForeignKey("WorkoutTemplateId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
