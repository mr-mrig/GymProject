using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;


namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingPlanEntityConfiguration : IEntityTypeConfiguration<TrainingPlanRoot>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanRoot> builder)
        {
            builder.ToTable("TrainingPlan", GymContext.DefaultSchema);

            builder.HasKey(plan => plan.Id);

            builder.Property(plan => plan.Id)
                //.HasConversion(new IdTypeValueConverter())
                .ValueGeneratedOnAdd();


            //builder.Ignore(plan => plan.DomainEvents);
            builder.Ignore(plan => plan.TrainingVolume);
            builder.Ignore(plan => plan.TrainingIntensity);
            builder.Ignore(plan => plan.TrainingDensity);

            builder.Ignore(plan => plan.TrainingPhaseIds);
            builder.Ignore(plan => plan.HashtagIds);
            builder.Ignore(plan => plan.TrainingProficiencyIds);
            builder.Ignore(plan => plan.IsTemplate);

            builder.Ignore(plan => plan.OwnerId);
            builder.Ignore(plan => plan.PersonalNoteId);
            builder.Ignore(plan => plan.TrainingWeeks);
            builder.Ignore(plan => plan.TrainingScheduleIds);


            //builder.Property(plan => plan.PersonalNoteId)
            //    .HasColumnName("TrainingNoteId");

            //builder.HasOne(plan => plan.PersonalNoteId)
            //    .WithOne();

            builder.Property(plan => plan.IsBookmarked)
                .HasColumnType("INTEGER")
                //.IsRequired(false)
                .HasConversion(new BoolToZeroOneConverter<int>());

            builder.Property(plan => plan.Name)
                .HasColumnType("TEXT")
                .IsRequired(false)
                .IsRequired();

            builder.HasMany<TrainingScheduleRoot>()
                .WithOne()
                .HasForeignKey(x => x.TrainingPlanId)
                .IsRequired();

            //builder.HasMany(plan => plan.TrainingWeeks)
            //    .WithOne()
            //    .HasForeignKey(week => week)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasIndex(plan => new { plan.OwnerId, plan.PersonalNoteId });



            //builder.Metadata.FindNavigation(nameof(TrainingPlanRoot.RelationsWithChildPlans))
            //    .SetPropertyAccessMode(PropertyAccessMode.Field);

            //builder.Metadata
            //    .FindNavigation(nameof(TrainingPlanRoot.RelationsWithParentPlans))
            //    .SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
