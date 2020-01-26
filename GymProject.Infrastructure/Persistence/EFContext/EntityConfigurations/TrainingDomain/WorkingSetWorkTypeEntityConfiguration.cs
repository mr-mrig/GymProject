using GymProject.Domain.TrainingDomain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkingSetWorkTypeEntityConfiguration : IEntityTypeConfiguration<WSWorkTypeEnum>
    {

        public void Configure(EntityTypeBuilder<WSWorkTypeEnum> builder)
        {
            builder.ToTable("WorkingSetWorkType", GymContext.DefaultSchema);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.Ignore(t => t.MeasUnit);

            builder.HasAlternateKey(t => t.Name);

            // Data Seeding
            builder.HasData(WSWorkTypeEnum.List());
        }
    }
}
