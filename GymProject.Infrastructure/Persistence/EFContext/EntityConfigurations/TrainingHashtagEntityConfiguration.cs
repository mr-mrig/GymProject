using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingHashtagEntityConfiguration : IEntityTypeConfiguration<TrainingHashtagRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingHashtagRoot> builder)
        {
            builder.ToTable("TrainingHashtag", GymContext.DefaultSchema);

            builder.HasKey(hashtag => hashtag.Id);

            builder.Property(hashtag => hashtag.Id)
                .HasConversion(new IdTypeValueConverter())
                .ValueGeneratedOnAdd();

            builder.Ignore(hashtag => hashtag.DomainEvents);


            builder.HasOne(hashtag => hashtag.EntryStatus)
                .WithMany();

        }

    }
}
