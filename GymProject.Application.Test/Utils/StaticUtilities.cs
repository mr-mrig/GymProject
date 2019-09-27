using Microsoft.EntityFrameworkCore;
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



    }
}
