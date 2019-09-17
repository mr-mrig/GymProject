using GymProject.Domain.BodyDomain.MuscleGroupAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class MuscleGroupEntityConfiguration : IEntityTypeConfiguration<MuscleGroupRoot>
    {


        public void Configure(EntityTypeBuilder<MuscleGroupRoot> builder)
        {
            builder.ToTable("MuscleGroup", GymContext.DefaultSchema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(e => e.DomainEvents);

            builder.Property(e => e.Name)
                .HasColumnType("TEXT")
                .IsRequired();

            builder.Property(e => e.Abbreviation)
                .HasColumnType("TEXT")
                .HasMaxLength(DatabaseEnvironmentConfiguration.MuscleAbbreviationMaxLength)
                .IsRequired();

            builder.HasAlternateKey(e => e.Name);
            builder.HasAlternateKey(e => e.Abbreviation);

            // Data Seeding
            builder.HasData(DataSeeding.GetMuscleGroupNativeEntries());
        }

    }
}
