using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class UserTrainingProficiencyEntityConfiguration : IEntityTypeConfiguration<UserTrainingProficiencyRelation>
    {

        private string _thisTableName = "UserTrainingProficiency";



        public void Configure(EntityTypeBuilder<UserTrainingProficiencyRelation> builder)
        {
            builder.ToTable(_thisTableName, GymContext.DefaultSchema);
            builder.HasKey("UserId", "StartDate");

            builder.Property("UserId");
            builder.Property(rel => rel.ProficiencyId).HasColumnName("TrainingProficiencyId");

            builder.Property(rel => rel.StartDate)
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.Property(rel => rel.EndDate)
                .HasColumnType("INTEGER");

            builder.HasOne<TrainingProficiencyRoot>()
                .WithMany()
                .HasForeignKey(rel => rel.ProficiencyId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasIndex("UserId", "EndDate", "StartDate");

        }
    }
}
