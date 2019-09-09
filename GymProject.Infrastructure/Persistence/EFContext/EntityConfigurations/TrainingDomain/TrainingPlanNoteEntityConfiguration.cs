using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanNoteEntityConfiguration : IEntityTypeConfiguration<TrainingPlanNoteRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanNoteRoot> builder)
        {
            builder.ToTable("TrainingPlanNote", GymContext.DefaultSchema);

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(m => m.DomainEvents);

            builder.OwnsOne(m => m.Body,
                n => n.Property(x => x.Body)
                        .HasColumnName("Body")
                        .IsRequired(true)
                        .HasMaxLength(PersonalNoteValue.DefaultMaximumLength)
            );


        }

    }
}
