using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class IntensityTechniqueEntityConfiguration : IEntityTypeConfiguration<IntensityTechniqueRoot>
    {


        public void Configure(EntityTypeBuilder<IntensityTechniqueRoot> builder)
        {
            builder.ToTable("IntensityTechnique", GymContext.DefaultSchema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(e => e.DomainEvents);

            builder.Property(e => e.Name)
                .HasColumnType("TEXT")
                .IsRequired();

            builder.Property(e => e.Abbreviation)
                .HasColumnType("TEXT")
                .IsRequired()
                .HasMaxLength(DatabaseEnvironmentConfiguration.AbbreviationDefaultMaxLength);

            builder.Property(e => e.IsLinkingTechnique)
                .HasColumnType("INTEGER")
                .IsRequired()
                .HasDefaultValue(false)
                .HasConversion(new BoolToZeroOneConverter<int>());

            builder.HasOne<UserRoot>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.OwnsOne(e => e.Description, pn =>
            {
                pn.Property(p => p.Body)
                    .HasColumnName("Description")
                    .HasColumnType("TEXT")
                    .IsRequired(false);
            });
            //.HasData(NativeExcerciseSeed.GetDataSeedOwnedEntity());

            builder.HasOne(e => e.EntryStatus)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasAlternateKey(e => e.Name);
            builder.HasAlternateKey(e => e.Abbreviation);

            // Data Seeding
            //builder.HasData(DataSeeding.GetExcerciseNativeEntries());
        }

    }
}
