using GymProject.Domain.TrainingDomain.WorkingSetNote;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkingSetEntityConfiguration : IEntityTypeConfiguration<WorkingSetEntity>
    {


        public void Configure(EntityTypeBuilder<WorkingSetEntity> builder)
        {
            builder.ToTable("WorkingSet", GymContext.DefaultSchema);

            builder.HasKey(ws => ws.Id);

            builder.Property(ws => ws.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(ws => ws.DomainEvents);

            builder.Property(ws => ws.ProgressiveNumber)
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.OwnsOne(ws => ws.Repetitions,
                r =>
                {
                    r.Property(p => p.Value)
                        .HasColumnType("INTEGER")
                        .HasColumnName("Repetitions")
                        .IsRequired();

                    r.Ignore(p => p.WorkType);
                });

            builder.Property(p => p.Load)
                .HasColumnType("DECIMAL")
                .HasColumnName("WeightKg")
                .HasConversion(new WeightLoadToDefaultUnitValueConverter())
                .IsRequired(false);

            builder.HasOne<WorkingSetNoteRoot>()
                .WithMany()
                .HasForeignKey(ws => ws.NoteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasAlternateKey
            (
                "WorkUnitId",
                "ProgressiveNumber"
            );
        }

    }
}
