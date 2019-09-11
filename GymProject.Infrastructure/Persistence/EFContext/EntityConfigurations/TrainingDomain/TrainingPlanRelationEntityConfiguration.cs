﻿using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class TrainingPlanRelationEntityConfiguration : IEntityTypeConfiguration<TrainingPlanRelation>
    {


        public void Configure(EntityTypeBuilder<TrainingPlanRelation> builder)
        {
            builder.ToTable("TrainingPlanRelation", GymContext.DefaultSchema);

            builder.HasKey(r => new
            {
                r.ParentPlanId,
                r.ChildPlanId,
            });

            builder.Property(r => r.ParentPlanId);
            builder.Property(r => r.ChildPlanId);

            builder.HasOne(r => r.ChildPlanType)
                .WithMany()
                .IsRequired()
                .HasForeignKey(x => x.ChildPlanTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(r => r.ParentPlan)
                .WithMany(x => x.RelationsWithChildPlans)
                //.WithMany("_relationsWithChildPlans")
                //.WithMany()
                .IsRequired()
                .HasForeignKey(x => x.ParentPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne(r => r.ChildPlan)
                //.WithMany("_relationsWithParentPlans")
                .WithMany(x => x.RelationsWithParentPlans)
                .IsRequired()
                .HasForeignKey(x => x.ChildPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<TrainingPlanMessageRoot>()
                .WithMany()
                .HasForeignKey(r => r.MessageId)
                .IsRequired(false);
        }

    }
}