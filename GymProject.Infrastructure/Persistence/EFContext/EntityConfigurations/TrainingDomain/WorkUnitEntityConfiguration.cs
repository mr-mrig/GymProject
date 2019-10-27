using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkUnitEntityConfiguration : IEntityTypeConfiguration<WorkUnitEntity>
    {


        public void Configure(EntityTypeBuilder<WorkUnitEntity> builder)
        {
            builder.ToTable("WorkUnit", GymContext.DefaultSchema);

            builder.HasKey(wu => wu.Id);

            builder.Property(wu => wu.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(wu => wu.DomainEvents);
            builder.Ignore(wu => wu.TrainingVolume);

            builder.Property(wu => wu.ProgressiveNumber)
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.OwnsOne(wu => wu.UserRating,
                ur =>
                {
                    ur.Property(p => p.Value)
                    .HasColumnName("Rating")
                    .HasColumnType("DECIMAL");
                });

            builder.HasOne<ExcerciseRoot>()
                .WithMany()
                .HasForeignKey(wu => wu.ExcerciseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(w => w.WorkingSets)
                .WithOne()
                .HasForeignKey("WorkUnitId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(WorkUnitEntity.WorkingSets));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //builder.HasAlternateKey
            //(
            //    "WorkoutSessionId",
            //    "ProgressiveNumber"
            //);
            builder.HasIndex
            (
                "WorkoutSessionId",
                "ProgressiveNumber"
            )
            .IsUnique();
        }

    }
}
