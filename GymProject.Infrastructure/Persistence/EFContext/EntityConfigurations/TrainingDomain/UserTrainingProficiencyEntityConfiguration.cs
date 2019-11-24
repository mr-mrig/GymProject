using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class UserTrainingProficiencyEntityConfiguration : IEntityTypeConfiguration<UserTrainingProficiencyRelation>
    {


        public void Configure(EntityTypeBuilder<UserTrainingProficiencyRelation> builder)
        {
            builder.ToTable("UserTrainingProficiency", GymContext.DefaultSchema);

            builder.Property("UserId");
            builder.Property(rel => rel.ProficiencyId);

            builder.HasKey("UserId", "ProficiencyId");

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
            builder.HasOne<TrainingProficiencyRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.ProficiencyId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            //builder.HasOne<AthleteRoot>()
            //    .WithMany()
            //    .HasForeignKey<AthleteRoot>(x => x.id)
            //    .OnDelete(DeleteBehavior.NoAction)
            //    .IsRequired();
        }
    }
}
