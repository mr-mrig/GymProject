using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingHashtagEntityConfiguration : IEntityTypeConfiguration<TrainingHashtagRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingHashtagRoot> builder)
        {
            builder.ToTable("TrainingHashtag", GymContext.DefaultSchema);

            builder.HasKey(hashtag => hashtag.Id);

            builder.Property(hashtag => hashtag.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(hashtag => hashtag.DomainEvents);


            builder.OwnsOne(hashtag => hashtag.Hashtag, hh =>
            {
                hh.Property(p => p.Body)
                    .HasColumnName("Body")
                    .HasColumnType("TEXT")
                    .IsRequired();
            });

            builder.HasOne(hashtag => hashtag.EntryStatus)
                .WithMany();

        }

    }
}
