using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkingSetIntensityTechniqueEntityConfiguration : IEntityTypeConfiguration<WorkingSetIntensityTechniqueRelation>
    {


        public void Configure(EntityTypeBuilder<WorkingSetIntensityTechniqueRelation> builder)
        {
            builder.ToTable("WorkingSetIntensityTechnique", GymContext.DefaultSchema);

            builder.HasKey(rel => new { rel.WorkingSetId, rel.IntensityTechniqueId });

            //builder.Property(rel => rel.LinkedWorkingSetId).IsRequired(false);
            builder.Ignore(rel => rel.LinkedWorkingSetId);
            builder.Property(rel => rel.WorkingSetId);
            builder.Property(rel => rel.IntensityTechniqueId);

            builder.HasOne(rel => rel.WorkingSet)
                .WithMany("_intensityTechniquesRelations")
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<IntensityTechniqueRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.IntensityTechniqueId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
