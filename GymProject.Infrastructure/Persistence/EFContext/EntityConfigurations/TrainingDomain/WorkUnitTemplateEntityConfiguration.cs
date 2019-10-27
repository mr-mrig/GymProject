using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkUnitTemplateEntityConfiguration : IEntityTypeConfiguration<WorkUnitTemplateEntity>
    {


        public void Configure(EntityTypeBuilder<WorkUnitTemplateEntity> builder)
        {
            builder.ToTable("WorkUnitTemplate", GymContext.DefaultSchema);

            builder.HasKey(wu => wu.Id);

            builder.Property(wu => wu.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(wu => wu.DomainEvents);
            builder.Ignore(wu => wu.TrainingVolume);
            builder.Ignore(wu => wu.TrainingIntensity);
            builder.Ignore(wu => wu.TrainingDensity);

            builder.Property(wu => wu.ProgressiveNumber)
                .HasColumnType("INTEGER")
                .IsRequired();

            //builder.Property<uint?>("WorkUnitTemplateNoteId").IsRequired(false);

            builder.HasOne<ExcerciseRoot>()
                .WithMany()
                .HasForeignKey(wu => wu.ExcerciseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<WorkUnitTemplateNoteRoot>()
                .WithMany()
                .HasForeignKey(wu => wu.WorkUnitNoteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            //builder.OwnsOne(ws => ws.LinkedWorkUnit,

            //     l =>
            //     {
            //         l.Property(p => p.LinkedWorkId)
            //            .HasColumnName("LinkedWorkUnitId");

            //         l.HasOne<WorkUnitTemplateEntity>()
            //             .WithMany()
            //             .HasForeignKey(p => p.LinkedWorkId)
            //             .IsRequired(false)
            //             .OnDelete(DeleteBehavior.Cascade);

            //         l.Property(p => p.LinkingIntensityTechniqueId)
            //            .HasColumnName("IntensityTechniqueId");

            //         l.HasOne<IntensityTechniqueRoot>()
            //             .WithMany()
            //             .HasForeignKey(p => p.LinkingIntensityTechniqueId)
            //             .IsRequired(false)
            //             .OnDelete(DeleteBehavior.SetNull);
            //     });

            //builder.HasOne<WorkUnitTemplateEntity>()
            //    .WithMany()
            //    .HasForeignKey(w => w.LinkedWorkUnitId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.SetNull);

            //builder.Ignore(w => w.LinkingIntensityTechniqueId);     // Use the FK with renamed column, see later

            builder.HasOne<IntensityTechniqueRoot>()
                .WithMany()
                .HasForeignKey(w => w.LinkingIntensityTechniqueId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(w => w.WorkingSets)
                .WithOne()
                .HasForeignKey("WorkUnitTemplateId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(WorkUnitTemplateEntity.WorkingSets));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //builder.HasAlternateKey
            //(
            //    "WorkoutTemplateId",
            //    "ProgressiveNumber"
            //);

            builder.HasIndex
            (
                "WorkoutTemplateId",
                "ProgressiveNumber"
            )
            .IsUnique();
        }

    }
}
