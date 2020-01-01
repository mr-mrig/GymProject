using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanPhaseEntityConfiguration : IEntityTypeConfiguration<TrainingPlanPhaseRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanPhaseRelation> builder)
        {
            builder.ToTable("TrainingPlanPhase", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.UserTrainingPlanId, rel.TrainingPhaseId });

            builder.Property(rel => rel.TrainingPhaseId);

            builder.HasOne(rel => rel.UserTrainingPlan)
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
