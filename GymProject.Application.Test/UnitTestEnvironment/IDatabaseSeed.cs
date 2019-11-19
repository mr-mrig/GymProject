using GymProject.Infrastructure.Persistence.EFContext;
using System.Threading.Tasks;

namespace GymProject.Application.Test.UnitTestEnvironment
{
    public interface IDatabaseSeed
    {

        public GymContext Context { get; }

        Task SeedTrainingDomain();

        /// <summary>
        /// Check whether the Unit Test Database has the test cases loaded or it must be seeded in order to start the tests.
        /// This method just checks a sample query, hence the developer must ensure that all the queries have been seeded.
        /// </summary>
        /// <returns>True if Db ready, false if seeding is needed</returns>
        bool IsDbReadyForUnitTesting();

    }
}
