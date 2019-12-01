using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class UserTrainingPlanEntityConfiguration : IEntityTypeConfiguration<UserTrainingPlanEntity>
    {


        public void Configure(EntityTypeBuilder<UserTrainingPlanEntity> builder)
        {
            builder.ToTable("UserTrainingPlan", GymContext.DefaultSchema);

            builder.HasKey(plan => plan.Id);

            builder.Property(plan => plan.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(plan => plan.DomainEvents);
            
            builder.Property(plan => plan.IsBookmarked)
                .HasColumnType("INTEGER")
                .HasDefaultValue(false)
                .HasConversion(new BoolToZeroOneConverter<int>());

            builder.Property(plan => plan.Name)
                .HasColumnType("TEXT")
                .IsRequired(false);

            builder.HasMany<TrainingScheduleRoot>()
                .WithOne()
                .HasForeignKey(x => x.UserTrainingPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<TrainingPlanNoteRoot>()
                .WithMany()
                .HasForeignKey(plan => plan.TrainingPlanNoteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);


            builder.HasAlternateKey(
                "UserId",
                "TrainingPlanId"
            );

            builder.HasIndex(x => new
            {
                x.TrainingPlanId,
                x.ParentPlanId,
                x.TrainingPlanNoteId,
            });
        }

    }
}
