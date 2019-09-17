using Microsoft.EntityFrameworkCore;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanMessageAggregate;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.UserAccountDomain.UserAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using System.Linq;
using GymProject.Domain.TrainingDomain.ExcerciseAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate;
using GymProject.Domain.BodyDomain.MuscleGroupAggregate;
using GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate;
using GymProject.Domain.TrainingDomain.WorkoutSessionAggregate;
using GymProject.Domain.TrainingDomain.WorkingSetNote;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    public partial class GymContext : DbContext
    {

        public const string DefaultSchema = "GymApp";

        //public static readonly LoggerFactory MyLoggerFactory
        //    = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });


        public GymContext()
        {
        }

        public GymContext(DbContextOptions<GymContext> options)
            : base(options)
        {
            
        }



        #region Entites

        public virtual DbSet<UserRoot> Users { get; set; }
        public virtual DbSet<AccountStatusTypeEnum> AccountStatusTypes { get; set; }

        public virtual DbSet<EntryStatusTypeEnum> EntryStatusTypes { get; set; }


        public virtual DbSet<TrainingPlanRoot> TrainingPlans { get; set; }
        public virtual DbSet<TrainingPlanNoteRoot> TrainingPlanNotes { get; set; }
        public virtual DbSet<TrainingPlanRelation> TrainingPlanRelations { get; set; }
        public virtual DbSet<TrainingPlanTypeEnum> TrainingPlanTypes { get; set; }

        public virtual DbSet<TrainingScheduleRoot> TrainingSchedules { get; set; }
        public virtual DbSet<TrainingScheduleFeedbackEntity> TrainingScheduleFeedbacks { get; set; }


        public virtual DbSet<WorkoutSessionRoot> WorkoutSessions { get; set; }
        public virtual DbSet<WorkUnitEntity> WorkUnits { get; set; }
        public virtual DbSet<WorkingSetEntity> WorkingSets { get; set; }
        public virtual DbSet<WorkingSetNoteRoot> WorkingSetNotes { get; set; }


        public virtual DbSet<WorkoutTemplateRoot> WorkoutTemplates { get; set; }
        public virtual DbSet<WorkUnitTemplateEntity> WorkUnitTemplates { get; set; }
        public virtual DbSet<WorkingSetTemplateEntity> WorkingSetTemplates { get; set; }
        public virtual DbSet<TrainingEffortTypeEnum> EffortTypes { get; set; }

        public virtual DbSet<WorkingSetIntensityTechniqueRelation> WorkingSetIntensityTechniqueRelations { get; set; }


        public virtual DbSet<TrainingWeekEntity> TrainingWeeks { get; set; }
        public virtual DbSet<WorkoutTemplateReferenceEntity> TrainingWeekWorkoutReferences { get; set; }
        public virtual DbSet<TrainingWeekTypeEnum> TrainingWeekTypes { get; set; }

        public virtual DbSet<TrainingPlanPhaseRelation> TrainingPlanPhases { get; set; }
        public virtual DbSet<TrainingPlanProficiencyRelation> TrainingPlanProficiencies { get; set; }
        public virtual DbSet<TrainingPlanHashtagRelation> TrainingPlanHashtags { get; set; }
        public virtual DbSet<TrainingPlanMuscleFocusRelation> TrainingPlanMusclesFocuses { get; set; }


        public virtual DbSet<TrainingHashtagRoot> TrainingHashtags { get; set; }
        public virtual DbSet<TrainingPhaseRoot> TrainingPhases { get; set; }
        public virtual DbSet<TrainingProficiencyRoot> TrainingProficiencies { get; set; }
        public virtual DbSet<TrainingPlanMessageRoot> TrainingPlanMessages { get; set; }
        public virtual DbSet<IntensityTechniqueRoot> IntensityTechniques { get; set; }


        public virtual DbSet<ExcerciseRoot> Excercises { get; set; }

        public virtual DbSet<MuscleGroupRoot> MuscleGroups { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    //builder.AddFilter("Microsoft", LogLevel.Warning)
            //    //       .AddFilter("System", LogLevel.Warning)
            //    //       .AddFilter("SampleApp.Program", LogLevel.Debug)
            //    builder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
            //           .AddConsole();
            //});

            optionsBuilder.UseSqlite(@"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;")
                ;/*.UseLoggerFactory(loggerFactory);*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.ApplyConfiguration(new EntryStatusEntityConfiguration());

            // User Aggregate
            modelBuilder.ApplyConfiguration(new AccountStatusTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());

            // Training Plan Aggregate
            modelBuilder.ApplyConfiguration(new TrainingPlanEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingPlanNoteEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingPlanRelationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingPlanTypeEntityConfiguration());

            modelBuilder.ApplyConfiguration(new TrainingWeekEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingWeekTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkoutReferenceEntityConfiguration());

            // Notes and minor Aggregates
            modelBuilder.ApplyConfiguration(new TrainingPlanFocusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingPlanHashtagEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingPlanPhaseEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingPlanProficiencyEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkUnitTemplateNoteEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkingSetNoteEntityConfiguration());

            // Workout Template Aggregate
            modelBuilder.ApplyConfiguration(new WorkoutTemplateEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkUnitTemplateEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkingSetTemplateEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkingSetIntensityTechniqueEntityConfiguration());

            // Intensity Technique Aggregate
            modelBuilder.ApplyConfiguration(new IntensityTechniqueEntityConfiguration());

            // Workout Session Aggregate
            modelBuilder.ApplyConfiguration(new WorkoutSessionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkUnitEntityConfiguration());
            modelBuilder.ApplyConfiguration(new WorkingSetEntityConfiguration());

            // Minor Training Aggregates
            modelBuilder.ApplyConfiguration(new TrainingHashtagEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingMessageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingProficiencyEntityConfiguration());
            modelBuilder.ApplyConfiguration(new EffortTypeEntityConfiguration());

            // Training Phase
            modelBuilder.ApplyConfiguration(new TrainingPhaseEntityConfiguration());

            // Training Schedule Aggregate
            modelBuilder.ApplyConfiguration(new TrainingScheduleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingFeedbackEntityConfiguration());

            // Excercise Aggregate
            modelBuilder.ApplyConfiguration(new ExcerciseEntityConfiguration());
            
            // Muscle Group Aggregate
            modelBuilder.ApplyConfiguration(new MuscleGroupEntityConfiguration());






            //// Disable conventions--warning: this uses internal code and may break on any EF release
            ////((Model)modelBuilder.Model).ConventionDispatcher.StartBatch();

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    foreach (var index in entityType.GetIndexes().ToList())
            //    {
            //        if (index.DeclaringEntityType.Name.Contains("InsertAFilterHere"))   // Remove the useless indexes
            //            entityType.RemoveIndex(index.Properties);
            //    }
            //}
        }
    }
}
