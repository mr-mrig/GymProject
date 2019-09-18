using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanEntityConfiguration : IEntityTypeConfiguration<TrainingPlanRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanRoot> builder)
        {
            builder.ToTable("TrainingPlan", GymContext.DefaultSchema);

            builder.HasKey(plan => plan.Id);

            builder.Property(plan => plan.Id)
                .ValueGeneratedOnAdd();

            builder.Ignore(plan => plan.DomainEvents);
            builder.Ignore(plan => plan.IsTemplate);
            builder.Ignore(plan => plan.WorkoutIds);

            builder.Property(plan => plan.IsBookmarked)
                .HasColumnType("INTEGER")
                .HasDefaultValue(false)
                .HasConversion(new BoolToZeroOneConverter<int>());

            builder.Property(plan => plan.Name)
                .HasColumnType("TEXT")
                .IsRequired(false);

            builder.HasMany<TrainingScheduleRoot>()
                .WithOne()
                .HasForeignKey(x => x.TrainingPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<UserRoot>()
                .WithMany()
                .HasForeignKey(plan => plan.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<TrainingPlanNoteRoot>()
                .WithMany()
                .HasForeignKey(plan => plan.TrainingPlanNoteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(plan => plan.TrainingWeeks)
                .WithOne()
                .HasForeignKey("TrainingPlanId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(TrainingPlanRoot.TrainingWeeks));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(plan => new
            {
                plan.OwnerId,
                plan.TrainingPlanNoteId,
            });
        }

    }
}
