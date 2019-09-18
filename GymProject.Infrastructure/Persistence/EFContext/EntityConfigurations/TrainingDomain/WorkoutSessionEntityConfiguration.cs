using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkoutSessionEntityConfiguration : IEntityTypeConfiguration<WorkoutSessionRoot>
    {


        public void Configure(EntityTypeBuilder<WorkoutSessionRoot> builder)
        {
            builder.ToTable("WorkoutSession", GymContext.DefaultSchema);

            builder.HasKey(wo => wo.Id);

            builder.Property(wo => wo.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(wo => wo.DomainEvents);
            builder.Ignore(wo => wo.TrainingVolume);
            builder.Ignore(wo => wo.PlannedDate);       // Maybe later

            builder.Property(p => p.StartTime)
                .HasColumnType("INTEGER")
                .HasConversion(new UnixTimestampValueConverter())
                .HasDefaultValueSql(DatabaseEnvironmentConfiguration.SqlLiteNowTimestamp)
                .IsRequired();

            builder.Property(p => p.EndTime)
                .HasColumnType("INTEGER")
                .HasConversion(new UnixTimestampValueConverter())
                .IsRequired(false);

            builder.HasOne<WorkoutTemplateRoot>()      // Why do EFCore 1-to-1 does not work?
                .WithMany()
                .HasForeignKey(nameof(WorkoutSessionRoot.WorkoutTemplateId))
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);     // Don't remove Workout Data when the Program is deleted

            builder.HasMany(w => w.WorkUnits)
                .WithOne()
                .HasForeignKey("WorkoutSessionId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(WorkoutSessionRoot.WorkUnits));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

        }

    }
}
