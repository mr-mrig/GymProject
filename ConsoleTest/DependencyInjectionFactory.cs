using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Collections.Generic;
using System.Text;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR.Extensions.Microsoft.DependencyInjection;

namespace ConsoleTest
{
    internal static class DependencyInjectionFactory
    {


        internal static IServiceCollection serviceProvider

            => new ServiceCollection()
                .AddDbContext<GymContext>(options 
                    => options.UseSqlite(@"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;")
                )
                .AddLogging(LoggerFactory.Create(config
                    => config.AddDebug())
                .CreateLogger("Test Logger")
                )
                .AddMediatR(typeof(ProcessAdSectionsRssCommand).GetType().Assembly)
    }
}
