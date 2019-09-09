using GymProject.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class EntryStatusEntityConfiguration : IEntityTypeConfiguration<EntryStatusTypeEnum>
    {


        public void Configure(EntityTypeBuilder<EntryStatusTypeEnum> builder)
        {
            builder.ToTable("EntryStatusType", GymContext.DefaultSchema);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.HasAlternateKey(e => e.Name);

            // Data Seeding
            builder.HasData(EntryStatusTypeEnum.List());
        }

    }
}
