using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkUnitTemplateNoteEntityConfiguration : IEntityTypeConfiguration<WorkUnitTemplateNoteRoot>
    {


        public void Configure(EntityTypeBuilder<WorkUnitTemplateNoteRoot> builder)
        {
            builder.ToTable("WorkUnitTemplateNote", GymContext.DefaultSchema);

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
