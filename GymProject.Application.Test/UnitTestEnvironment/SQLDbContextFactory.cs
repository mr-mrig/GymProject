using GymProject.Application.Test.Utils;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    public class SQLDbContextFactory : DbContextFactory, IDisposable
    {

        private static ILoggerFactory _loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddDebug();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
        });



        /// <summary>
        /// Factory for creating an in-memory DB context instance ready-to-use for CQRS command tests.
        /// The Context is already seeded with ad-hoc test data and can be used for each unit test.
        /// </summary>
        public SQLDbContextFactory() { }


        protected virtual DbContextOptions<GymContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<GymContext>()
                .UseSqlite(new SQLiteConnectionStringBuilder { DataSource = ApplicationTestService.ApplicationDbRelativePath, }.ConnectionString)
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .Options;
        }


        /// <summary>
        /// Creates the DB Context and seeds it if necessary.
        /// The physical DB - <see cref="ApplicationTestService.ApplicationCommandDbRelativePath"/> - must already exist.
        /// </summary>
        /// <returns>The DB Context</returns>
        public override async Task<GymContext> CreateContextAsync()
        {
            var options = CreateOptions();
            using (var context = new GymContext(options, new DummyMediator(), _loggerFactory.CreateLogger(typeof(GymContext))))
            {
                //context.Database.EnsureCreated();

                ContextSeed = new SqlDatabaseSeed(context);

                if (!ContextSeed.IsDbReadyForUnitTesting())
                    await ContextSeed.SeedTrainingDomain();
            }

            return new GymContext(CreateOptions(), new DummyMediator(), _loggerFactory.CreateLogger(typeof(GymContext)));
        }

        public void Dispose()
        {
            // No connection to close here
            // IDisposable implemented just for supporting the 'using' block
        }
    }
}
