using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanProficiencyEntityConfiguration : IEntityTypeConfiguration<TrainingPlanProficiencyRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanProficiencyRelation> builder)
        {
            builder.ToTable("TrainingPlanProficiency", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.TrainingPlanId, rel.TrainingProficiencyId });

            builder.Property(rel => rel.TrainingPlanId);

            builder.Property(rel => rel.TrainingProficiencyId);

            builder.HasOne(rel => rel.TrainingPlan)
                .WithMany("_trainingPlanProficiencies")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<TrainingProficiencyRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.TrainingProficiencyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
