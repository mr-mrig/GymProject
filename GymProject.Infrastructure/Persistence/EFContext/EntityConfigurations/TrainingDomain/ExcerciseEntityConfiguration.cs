using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class ExcerciseEntityConfiguration : IEntityTypeConfiguration<ExcerciseRoot>
    {


        public void Configure(EntityTypeBuilder<ExcerciseRoot> builder)
        {
            builder.ToTable("Excercise", GymContext.DefaultSchema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(e => e.DomainEvents);


            builder.Property(e => e.Name)
                .HasColumnType("TEXT")
                .IsRequired();

            builder.OwnsOne(e => e.Description, pn =>
            {
                pn.Property(p => p.Body)
                    .HasColumnName("Description")
                    .HasColumnType("TEXT")
                    .IsRequired(false);
            });

            builder.HasOne(e => e.EntryStatus)
                .WithMany().IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasAlternateKey(e => e.Name);
        }

    }
}
