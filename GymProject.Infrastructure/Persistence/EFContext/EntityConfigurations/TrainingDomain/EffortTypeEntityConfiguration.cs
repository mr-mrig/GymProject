using GymProject.Domain.TrainingDomain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class EffortTypeEntityConfiguration : IEntityTypeConfiguration<TrainingEffortTypeEnum>
    {

        public void Configure(EntityTypeBuilder<TrainingEffortTypeEnum> builder)
        {
            builder.ToTable("TrainingEffortType", GymContext.DefaultSchema);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.Property(t => t.Abbreviation)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.HasAlternateKey(t => t.Name);
            builder.HasAlternateKey(t => t.Abbreviation);

            builder.Property(t => t.Description)
                .HasColumnType("TEXT");

            // Data Seeding
            builder.HasData(TrainingEffortTypeEnum.List());
        }
    }
}
