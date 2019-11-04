using FluentValidation;
using GymProject.Domain.Base;
using GymProject.Infrastructure.Persistence.EFContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GymProject.Application.Test.Utils
{
    public static class StaticUtilities
    {



        /// <summary>
        /// Generate the DbContextOptions for building an in-memory Db Context fully isolated fromone test to the other
        /// </summary>
        /// <typeparam name="T">DbContext child</typeparam>
        /// <param name="dbname">The name of the DB options, should be unique to achieve isolation</param>
        /// <returns>The DbContextOptions</returns>
        public static DbContextOptions GetInMemoryIsolatedDbContextOptions<T>([CallerMemberName] string dbname = "") 
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
        public static (GymContext, IMediator, ILogger<T>) InitCommandTest<T>(string callerName)
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<T>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>(callerName)
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;

            return(context, mediator.Object, logger.Object);
        }


        /// <summary>
        /// Query Unit test initial setup. It insulates each test environment according to the test name
        /// </summary>
        /// <param name="callerName">The name of the test. This is mandatory to insulate each context avoiding exceptions</param>
        /// <returns></returns>
        public static GymContext InitQueryTest(string callerName)
        {

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>(callerName)));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;

            return context;
        }


        public static T GetSourceAggregate<T>(IRepository<T> repo, uint id) where T : class, IAggregateRoot

            => repo.Find(id) as T;


        //public static void TestcommandValidator<TCommand,TValidator,Z>() where TValidator : AbstractValidator<TCommand>
        //{
        //    ILogger<TCommand> loggerValidator;
        //    Mock<ILogger<TValidator>> logger = new Mock<ILogger<TValidator>>();
        //    TValidator validator = new TValidator(logger.Object);
        //    Assert.False(validator.Validate(command).IsValid);
        //}

    }
}
