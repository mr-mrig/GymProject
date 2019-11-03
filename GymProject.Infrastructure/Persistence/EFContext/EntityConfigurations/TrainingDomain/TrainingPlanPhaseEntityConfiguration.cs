using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanPhaseEntityConfiguration : IEntityTypeConfiguration<TrainingPlanPhaseRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanPhaseRelation> builder)
        {
            builder.ToTable("TrainingPlanPhase", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.TrainingPlanId, rel.TrainingPhaseId });

            builder.Property(rel => rel.TrainingPlanId);

            builder.Property(rel => rel.TrainingPhaseId);

            builder.HasOne(rel => rel.TrainingPlan)
                .WithMany("_trainingPlanPhases")
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<TrainingPhaseRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.TrainingPhaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
