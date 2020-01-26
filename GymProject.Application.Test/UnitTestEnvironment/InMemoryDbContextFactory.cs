using GymProject.Infrastructure.Persistence.EFContext;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    public class InMemoryDbContextFactory : DbContextFactory, IDisposable
    {

        private static ILoggerFactory _loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddDebug();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
        });

        /// <summary>
        /// The connection must be kept open to prevent the in-memory DB to be flushed
        /// </summary>
        private DbConnection _connection;



        /// <summary>
        /// Factory for creating an in-memory DB context instance ready-to-use for CQRS command tests.
        /// The Context is already seeded with ad-hoc test data and can be used for each unit test.
        /// </summary>
        public InMemoryDbContextFactory() { }


        protected virtual DbContextOptions<GymContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<GymContext>()
                .UseSqlite(_connection)
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .Options;
        }


        /// <summary>
        /// Set the in-memory database up by creating it from scratch or opening if already created.
        /// </summary>
        /// <returns>The DB Context</returns>
        public override async Task<GymContext> CreateContextAsync()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ConnectionString);
                _connection.Open();

                var options = CreateOptions();

                using (var context = new GymContext(options, new DummyMediator(), _loggerFactory.CreateLogger(typeof(GymContext))))
                {
                    context.Database.EnsureCreated();

                    ContextSeed = new InMemoryDatabaseSeed(context);

                    if (!ContextSeed.IsDbReadyForUnitTesting())
                        await ContextSeed.SeedTrainingDomain();
                }
            }

            return new GymContext(CreateOptions(), new DummyMediator(), _loggerFactory.CreateLogger(typeof(GymContext)));
        }


        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

    }
}
