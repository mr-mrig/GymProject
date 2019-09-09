using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingFeedbackEntityConfiguration : IEntityTypeConfiguration<TrainingScheduleFeedbackEntity>
    {


        public void Configure(EntityTypeBuilder<TrainingScheduleFeedbackEntity> builder)
        {
            builder.ToTable("TrainingScheduleFeedback", GymContext.DefaultSchema);

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();


            builder.Ignore(f => f.UserId);      // TODO

            builder.OwnsOne(f => f.Rating, 
                r => r.Property(x => x.Value)
                    //.IsRequired(false)
                    .HasColumnName("Rating"));

            builder.OwnsOne(f => f.Comment, 
                c => c.Property(x => x.Body)
                    .HasColumnName("Comment")
                    //.IsRequired(false)
                    .HasMaxLength(PersonalNoteValue.DefaultMaximumLength));

            builder.HasOne<TrainingScheduleRoot>()
                .WithMany(s => s.Feedbacks)
                .IsRequired()
                .HasForeignKey("TrainingScheduleId")
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
