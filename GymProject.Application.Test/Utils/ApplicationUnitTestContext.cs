using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Test.Utils
{
    public class ApplicationUnitTestContext : GymContext
    {

        /// <summary>
        /// The connection string to the SQLite DB loaded with the application test cases
        /// </summary>
        public const string SQLiteDbTestConnectionString = @"DataSource=..\..\..\applicationUnitTestDb.db;";




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(SQLiteDbTestConnectionString);
        }



        public override async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            //No MediatR

            // Preliminary operations to ensure consistent operations
            IgnoreStaticEnumTables();
            //IgnoreEmbeddedValueObjects();

            await base.SaveChangesAsync(cancellationToken);

            // No logging
            return true;
        }


    }
}
    