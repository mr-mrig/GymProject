using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
{
    internal class TrainingPlanEntityTypeConfiguration : IEntityTypeConfiguration<TrainingPlan>
    {


        public void Configure(EntityTypeBuilder<TrainingPlan> planConfiguration)
        {
            planConfiguration.ToTable("TrainingPlan", GymContext.DefaultSchema);

            planConfiguration.Ignore(b => b.DomainEvents);
            planConfiguration.Ignore(b => b.TrainingPlanType);
            planConfiguration.Ignore(b => b.TrainingVolume);
            planConfiguration.Ignore(b => b.TrainingIntensity);
            planConfiguration.Ignore(b => b.TrainingDensity);
            planConfiguration.Ignore(b => b.AttachedMessageId);

            planConfiguration.HasMany<TrainingPhase>(x => x.TrainingPhaseIds)
                .WithOne
                

            planConfiguration.HasKey(o => o.Id);

            planConfiguration.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderitemseq");

            planConfiguration.Property<int>("OrderId")
                .IsRequired();

            planConfiguration.Property<decimal>("Discount")
                .IsRequired();

            planConfiguration.Property<int>("ProductId")
                .IsRequired();

            planConfiguration.Property<string>("ProductName")
                .IsRequired();

            planConfiguration.Property<decimal>("UnitPrice")
                .IsRequired();

            planConfiguration.Property<int>("Units")
                .IsRequired();

            planConfiguration.Property<string>("PictureUrl")
                .IsRequired(false);
        }

    }
}
