using Microsoft.EntityFrameworkCore;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.SharedKernel;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    public partial class GymContext : DbContext
    {

        public const string DefaultSchema = "GymApp";


        public GymContext()
        {
        }

        public GymContext(DbContextOptions<GymContext> options)
            : base(options)
        {
            
        }


        #region Entites

        public virtual DbSet<TrainingPlanRoot> TrainingPlans { get; set; }
        public virtual DbSet<TrainingPlanRelation> TrainingPlanRelations { get; set; }
        public virtual DbSet<TrainingPlanTypeEnum> TrainingPlanTypes { get; set; }
        

        //public virtual DbSet<TrainingWeekEntity> TrainingWeeks  { get; set; }
        //public virtual DbSet<TrainingWeekTypeEnum> TrainingWeekTypes  { get; set; }
        //public virtual DbSet<TrainingPlanPhaseRelation> TrainingPlanPhases  { get; set; }
        //public virtual DbSet<TrainingPlanProficiencyRelation> TrainingPlanProficiencies  { get; set; }
        public virtual DbSet<TrainingPlanHashtagRelation> TrainingPlanHashtags { get; set; }
        //public virtual DbSet<TrainingPlanMuscleFocusRelation> TrainingPlanMuscleFocuses  { get; set; }

        public virtual DbSet<TrainingHashtagRoot> TrainingHashtags { get; set; }

        public virtual DbSet<EntryStatusTypeEnum> EntryStatusType { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new EntryStatusEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TrainingPlanEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TrainingPlanRelationEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TrainingPlanTypeEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TrainingPlanHashtagEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TrainingHashtagEntityConfiguration());
        }
    }
}
