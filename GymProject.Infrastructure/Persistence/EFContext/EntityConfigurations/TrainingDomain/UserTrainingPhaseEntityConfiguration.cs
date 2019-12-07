using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class UserTrainingPhaseEntityConfiguration : IEntityTypeConfiguration<UserTrainingPhaseRelation>
    {


        public void Configure(EntityTypeBuilder<UserTrainingPhaseRelation> builder)
        {
            builder.ToTable("UserTrainingPhase", GymContext.DefaultSchema);

            builder.Property("UserId");
            builder.Property(rel => rel.PhaseId);

            builder.HasKey("UserId", "PhaseId", "StartDate");


            builder.OwnsOne(p => p.OwnerNote, n =>
                n.Property(x => x.Body)
                    .HasColumnName("OwnerNote")
                    .HasMaxLength(PersonalNoteValue.DefaultMaximumLength)
            );

            builder.HasOne(e => e.EntryStatus)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.OwnsOne(p => p.Period, per =>
            {
                per.Property(p => p.Start)
                    .HasColumnName("StartDate")
                    .HasColumnType("INTEGER")
                    .IsRequired(true);

                per.Property(p => p.End)
                    .HasColumnName("EndDate")
                    .HasColumnType("INTEGER")
                    .IsRequired(false);
            });
            builder.HasOne<TrainingPhaseRoot>()
                .WithMany()
                .HasForeignKey(tp => tp.PhaseId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            //builder.HasOne<AthleteRoot>()
            //    .WithMany()
            //    .HasForeignKey(u => u.AthleteId)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .IsRequired();
        }
    }
}
