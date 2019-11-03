using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanHashtagEntityConfiguration : IEntityTypeConfiguration<TrainingPlanHashtagRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanHashtagRelation> builder)
        {
            builder.ToTable("TrainingPlanHashtag", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.TrainingPlanId, rel.HashtagId });

            builder.Property(rel => rel.TrainingPlanId);
            builder.Property(rel => rel.HashtagId);
            //builder.Property(rel => rel.ProgressiveNumber);
            builder.Ignore(rel => rel.ProgressiveNumber);

            builder.HasOne(rel => rel.TrainingPlan)
                .WithMany("_trainingPlanHashtags")
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<TrainingHashtagRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.HashtagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
