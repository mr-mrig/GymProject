﻿using Microsoft.EntityFrameworkCore;
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
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System;
using GymProject.Domain.Base;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GymProject.Infrastructure.Mediator;
using Microsoft.EntityFrameworkCore.Design;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Infrastructure.Persistence.EFContext
{
    public partial class GymContext : DbContext, IUnitOfWork
    {

        public const string DefaultSchema = "GymApp";

        private IDbContextTransaction _currentTransaction;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        //public static readonly LoggerFactory MyLoggerFactory
        //    = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });


        public GymContext() { }


        public GymContext(IMediator mediator, ILogger logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public GymContext(DbContextOptions options, IMediator mediator, ILogger logger) 
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public GymContext(DbContextOptions options) : base(options) { }



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


            // Should be configured elsewhere?
            //optionsBuilder.UseSqlite(@"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;")
            //optionsBuilder.UseSqlite(@"DataSource=C:\Users\Admin\Source\Repos\GymProject\GymProject.Infrastructure\test.db;");  // EF needs this - commaent again after the DB build
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
            //modelBuilder.ApplyConfiguration(new WorkoutReferenceEntityConfiguration());

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


        #region IUnitOfWork Implementation

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.PublishDomainEventsAsync(this, _logger);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"DB Context saved");

            return true;
        }

        #endregion


        #region Transactions Management

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public IDbContextTransaction BeginTransaction()
        {
            if (_currentTransaction != null)
                return null;

            _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public void CommitTransaction(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                SaveChanges();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion

        public bool HasActiveTransaction() => _currentTransaction != null;

    }



    #region Factory Class

    //public class GymContextDesignFactory : IDesignTimeDbContextFactory<GymContext>
    //{
    //    public GymContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<GymContext>()
    //            .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.OrderingDb;Integrated Security=true");

    //        return new GymContext(optionsBuilder.Options, new NoMediator());
    //    }

    //    class NoMediator : IMediatorService
    //    {
    //        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
    //        {
    //            return Task.CompletedTask;
    //        }

    //        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
    //        {
    //            return Task.FromResult<TResponse>(default(TResponse));
    //        }

    //        public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
    //        {
    //            return Task.CompletedTask;
    //        }
    //    }
    //}
    #endregion
}
