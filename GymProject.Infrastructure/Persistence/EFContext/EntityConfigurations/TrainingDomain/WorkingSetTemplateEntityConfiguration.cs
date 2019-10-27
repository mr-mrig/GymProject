using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkingSetTemplateEntityConfiguration : IEntityTypeConfiguration<WorkingSetTemplateEntity>
    {


        public void Configure(EntityTypeBuilder<WorkingSetTemplateEntity> builder)
        {
            builder.ToTable("WorkingSetTemplate", GymContext.DefaultSchema);

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
                        .HasColumnName("TargetRepetitions")
                        .IsRequired();

                    r.Ignore(p => p.WorkType);
                });

            builder.OwnsOne(ws => ws.Rest,
                r =>
                {
                    r.Property(p => p.Value)
                        .HasColumnType("INTEGER")
                        .HasColumnName("Rest");

                    r.Ignore(p => p.MeasureUnit);
                });

            builder.OwnsOne(ws => ws.Tempo,
                r =>
                {
                    r.Property(p => p.TUT)
                        .HasColumnType("TEXT")
                        .HasColumnName("Cadence");
                });

            builder.OwnsOne(ws => ws.Effort,
                e =>
                {
                    e.Property(p => p.Value)
                    .HasColumnName("Effort")
                    .HasColumnType("INTEGER");

                    //e.Property(p => p.EffortType)
                    //   .HasColumnName("EffortTypeId");

                    e.HasOne(p => p.EffortType)
                        .WithMany()
                        .HasForeignKey("EffortTypeId")
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.NoAction);
                });

            // Intensity technique is handled in the Connection Class Configuration

            //builder.HasAlternateKey
            //(
            //    "WorkUnitTemplateId",
            //    "ProgressiveNumber"
            //);

            builder.HasIndex
            (
                "WorkUnitTemplateId",
                "ProgressiveNumber"
            )
            .IsUnique();
        }

    }
}
