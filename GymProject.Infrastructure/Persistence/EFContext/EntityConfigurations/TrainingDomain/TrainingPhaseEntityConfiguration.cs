using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPhaseEntityConfiguration : IEntityTypeConfiguration<TrainingPhaseRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingPhaseRoot> builder)
        {
            builder.ToTable("TrainingPhase", GymContext.DefaultSchema);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(p => p.DomainEvents);

            builder.HasOne(p => p.EntryStatus)
                .WithMany().IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            //builder.HasOne<UserRoot>()
            //    .WithMany()
            //    .HasForeignKey(p => p.OwnerId)
            //    .OnDelete(DeleteBehavior.NoAction);     // Cascade must occur only when the phase is Private -> SW managed

            builder.HasAlternateKey(p => p.Name);       // Should be changed to (Owner, Name) when Owners will be enabled

            //// Data Seeding
            //builder.HasData(DataSeeding.GetTrainingPhaseNativeEntries());
        }

    }
}
