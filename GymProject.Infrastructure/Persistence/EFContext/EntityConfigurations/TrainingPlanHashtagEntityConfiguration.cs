using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingPlanHashtagEntityConfiguration : IEntityTypeConfiguration<TrainingPlanHashtagRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanHashtagRelation> builder)
        {
            builder.ToTable("TrainingPlanHashtag", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.TrainingPlanId, rel.HashtagId});

            builder.Property(rel => rel.TrainingPlanId)
                .HasConversion(new IdTypeValueConverter());

            builder.Property(rel => rel.HashtagId)
                .HasConversion(new IdTypeValueConverter());

            builder.HasOne(rel => rel.TrainingPlan)
                .WithMany("_trainingPlanHashtags")
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(rel => rel.HashtagId)
            //    .WithMany();
        }

    }
}
