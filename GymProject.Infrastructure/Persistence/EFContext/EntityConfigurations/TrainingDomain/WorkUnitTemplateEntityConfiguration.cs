using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class WorkUnitTemplateEntityConfiguration : IEntityTypeConfiguration<WorkUnitTemplateEntity>
    {


        public void Configure(EntityTypeBuilder<WorkUnitTemplateEntity> builder)
        {
            builder.ToTable("WorkUnitTemplate", GymContext.DefaultSchema);

            builder.HasKey(wu => wu.Id);

            builder.Property(wu => wu.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(wu => wu.DomainEvents);
            builder.Ignore(wu => wu.TrainingVolume);
            builder.Ignore(wu => wu.TrainingIntensity);
            builder.Ignore(wu => wu.TrainingDensity);

            builder.Property(wu => wu.ProgressiveNumber)
                .HasColumnType("INTEGER")
                .IsRequired();

            //builder.Property<uint?>("WorkUnitTemplateNoteId").IsRequired(false);

            builder.HasOne<ExcerciseRoot>()
                .WithMany()
                .HasForeignKey(wu => wu.ExcerciseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<WorkUnitTemplateNoteRoot>()
                .WithMany()
                .HasForeignKey(wu => wu.WorkUnitNoteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(w => w.WorkingSets)
                .WithOne()
                .HasForeignKey("WorkUnitTemplateId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(WorkUnitTemplateEntity.WorkingSets));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);


            //intesity technique
            //throw new System.NotImplementedException("Intensity techinque N-to-N");


            builder.HasAlternateKey
            (
                "WorkoutTemplateId",
                "ProgressiveNumber"
            );
        }

    }
}
