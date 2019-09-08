//using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations
//{
//    internal class TrainingPlanRelationEntityConfiguration : IEntityTypeConfiguration<TrainingPlanRelation>
//    {


//        public void Configure(EntityTypeBuilder<TrainingPlanRelation> builder)
//        {
//            builder.ToTable("TrainingPlanRelation", GymContext.DefaultSchema);

//            builder.HasKey(r => new
//            {
//                r.ParentPlanId,
//                r.ChildPlanId,
//            });

//            builder.Property(r => r.ParentPlanId)
//;

//            builder.Property(r => r.ChildPlanId)
//;

//            builder.HasOne(r => r.ChildPlanType)
//                .WithMany()
//                .IsRequired();

//            builder.HasOne(r => r.ParentPlan)
//                .WithMany(x => x.RelationsWithChildPlans)
//                //.WithMany("_relationsWithChildPlans")
//                //.WithMany()
//                .IsRequired()
//                .HasForeignKey(x => x.ParentPlanId)
//                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

//            builder.HasOne(r => r.ChildPlan)
//                //.WithMany("_relationsWithParentPlans")
//                .WithMany(x => x.RelationsWithParentPlans)
//                .IsRequired()
//                .HasForeignKey(x => x.ChildPlanId)
//                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

//            builder.Ignore(r => r.MessageId);
//        }

//    }
//}
