﻿using GymProject.Application.Test.UnitTestEnvironment;
using GymProject.Domain.Base;
using GymProject.Infrastructure.Persistence.EFContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymProject.Application.Test.Utils
{
    internal static class ApplicationTestService
    {


        public const string SeedDbSetupScriptName = "ApplicationUnitTestDbSetup.bat";
        public const string SeedDbSetupScripFolderPath = @"C:\Users\Admin\Source\Repos\GymProject\GymProject.Application.Test\";

        public const string ApplicationDbRelativePath = @"..\..\..\applicationUnitTestDb.db";



        /// <summary>
        /// Generate the DbContextOptions for building an in-memory Db Context fully isolated from one test to the other
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
        /// Command Unit test initial setup on a in-memory DB. It insulates each test environment according to the test name.
        /// </summary>
        /// <typeparam name="T">The command handler class</typeparam>
        /// <param name="callerName">The name of the test. This is mandatory to insulate each context avoiding exceptions</param>
        /// <returns></returns>
        internal async static Task<(GymContext, IMediator, ILogger<T>)> InitInMemoryCommandTest<T>(string callerName)
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<T>>();

            DatabaseSeed seed = new InMemoryDatabaseSeed(new GymContext(
                GetInMemoryIsolatedDbContextOptions<GymContext>(callerName), mediator.Object, logger.Object));

            await seed.SeedTrainingDomain();

            return (seed.Context, mediator.Object, logger.Object);
        }


        internal static T GetSourceAggregate<T>(IRepository<T> repo, uint id) where T : class, IAggregateRoot

            => repo.Find(id) as T;



        /// <summary>
        /// Unit test ad-hoc JSON serializer which ignores all the properties marked in the <cref="ApplicationTestJsonContractResolver"></cref>
        /// As the Unit Test DB seeding does not guarantee insertion orders, most of the IDs might differ from one seedin to the other,
        /// hence comparing the IDs to pre-set ones might lead to false negatives.
        /// </summary>
        /// <param name="input">The object to be serialized to a JSON string</param>
        /// <returns>The string representation of the output JSON</returns>
        internal static string JsonUnitTestSafeSerializer(object input)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = ApplicationTestJsonContractResolver.Instance,
            };

            return JsonConvert.SerializeObject(input, settings);
        }


    }
}
