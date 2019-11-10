using GymProject.Domain.Base;
using GymProject.Infrastructure.Persistence.EFContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Runtime.CompilerServices;

namespace GymProject.Application.Test.Utils
{
    internal static class ApplicationTestService
    {


        //internal const string SQLiteDbTestConnectionString = @"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;";
        //internal const string SQLiteDbTestConnectionString = @"DataSource=..\..\..\..\GymProject.Infrastructure\test.db;";
        




        /// <summary>
        /// Generate the DbContextOptions for building an in-memory Db Context fully isolated fromone test to the other
        /// </summary>
        /// <typeparam name="T">DbContext child</typeparam>
        /// <param name="dbname">The name of the DB options, should be unique to achieve isolation</param>
        /// <returns>The DbContextOptions</returns>
        internal static DbContextOptions GetInMemoryIsolatedDbContextOptions<T>([CallerMemberName] string dbname = "") 
            where T : DbContext

            => new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(dbname)
                .Options;


        /// <summary>
        /// Command Unit test initial setup. It insulates each test environment according to the test name
        /// </summary>
        /// <typeparam name="T">The command handler class</typeparam>
        /// <param name="callerName">The name of the test. This is mandatory to insulate each context avoiding exceptions</param>
        /// <returns></returns>
        internal static (GymContext, IMediator, ILogger<T>) InitCommandTest<T>(string callerName)
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<T>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                GetInMemoryIsolatedDbContextOptions<GymContext>(callerName)
                , mediator.Object
                , logger.Object));

            seed.SeedTrainingDomain();

            return(seed.Context, mediator.Object, logger.Object);
        }


        /// <summary>
        /// Query Unit test initial setup. It insulates each test environment according to the test name
        /// </summary>
        /// <param name="callerName">The name of the test. This is mandatory to insulate each context avoiding exceptions</param>
        /// <returns></returns>
        internal static GymContext InitQueryTest()
        {
            // Application Test DB
            DatabaseSeed seed = new DatabaseSeed(new ApplicationUnitTestContext());

            // Seed it if necessary
            if (!seed.IsDbReadyForUnitTesting())
                seed.SeedTrainingDomain();

            return seed.Context;
        }


        internal static T GetSourceAggregate<T>(IRepository<T> repo, uint id) where T : class, IAggregateRoot

            => repo.Find(id) as T;


    }
}
