using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanTypeEntityConfiguration : IEntityTypeConfiguration<TrainingPlanTypeEnum>
    {

        public void Configure(EntityTypeBuilder<TrainingPlanTypeEnum> builder)
        {
            builder.ToTable("TrainingPlanType", GymContext.DefaultSchema);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.HasAlternateKey(t => t.Name);

            builder.Property(t => t.Description)
                .HasColumnType("TEXT");

            // Data Seeding
            builder.HasData(TrainingPlanTypeEnum.List());
        }
    }
}
