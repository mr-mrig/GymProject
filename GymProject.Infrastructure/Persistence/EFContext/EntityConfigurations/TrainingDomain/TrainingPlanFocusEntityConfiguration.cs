using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanFocusEntityConfiguration : IEntityTypeConfiguration<TrainingPlanMuscleFocusRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanMuscleFocusRelation> builder)
        {
            builder.ToTable("TrainingPlanMuscleFocus", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.TrainingPlanId, rel.MuscleGroupId });

            builder.Property(rel => rel.TrainingPlanId);

            builder.Property(rel => rel.MuscleGroupId);

            builder.HasOne(rel => rel.TrainingPlan)
                .WithMany("_trainingPlanMuscleFocusIds")
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<TrainingHashtagRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.MuscleGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
