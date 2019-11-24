using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    internal class AthleteEntityConfiguration : IEntityTypeConfiguration<AthleteRoot>
    {
        public void Configure(EntityTypeBuilder<AthleteRoot> builder)
        {
            builder.ToTable("User", GymContext.DefaultSchema);

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne<UserRoot>()
                .WithOne()
                .HasForeignKey<AthleteRoot>(x => x.Id);

            builder.HasMany(x => x.TrainingProficiencies)
                .WithOne()
                .HasForeignKey("UserId");

            builder.HasMany(x => x.TrainingPhases)
                .WithOne()
                .HasForeignKey("UserId");
        }
    }
}