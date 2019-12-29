using GymProject.Domain.TrainingDomain.AthleteAggregate;
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

            builder.HasKey(rel => new { rel.UserTrainingPlanId, rel.TrainingProficiencyId });

            builder.Ignore(rel => rel.Id);
            builder.Property(rel => rel.TrainingProficiencyId);

            builder.HasOne(rel => rel.UserTrainingPlan)
                .WithMany("_trainingPlanProficiencies")
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<TrainingProficiencyRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.TrainingProficiencyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
