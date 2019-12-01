using Microsoft.Extensions.DependencyInjection;
using MediatR;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using GymProject.Infrastructure.Mediator;
using GymProject.Domain.Base.Mediator;
using GymProject.Application.DomainEventHandler;
using TinyIoC;
using Autofac;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
using GymProject.Application.MediatorBehavior;

namespace ConsoleTest
{
    internal static class DependencyInjectionFactory
    {


        /// <summary>
        /// Service Provider to be used with Microsoft.Extensions.DependencyInjection 
        /// </summary>
        internal static ServiceProvider DefaultMicrosoftDIServiceProvider

            => new ServiceCollection()

                    .AddDbContext<GymContext>(options =>
                        {
                            options.UseSqlite(@"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;");
                        }
                    )
                    .AddLogging(loggingBuilder =>
                        {
                            loggingBuilder.AddDebug();
                            loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                        }
                    )

                    .AddTransient<IMediatorService, MediatorWrapper>()      // My MediatR

                     //.AddTransient(typeof(IDomainNotificationHandler<>) ,typeof(LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler))       // Crashes

                    //.AddTransient<IDomainNotificationHandler<IDomainNotification>, NotificationHandlerWrapper<INotificationHandler, IDomainNotificationHandler>()
                    //.AddTransient<IDomainNotificationHandler<IDomainNotification>, NotificationHandlerWrapper<NotificationWrapper<IDomainNotification>, IDomainNotification>>()      // My MediatR
                    .AddMediatR(Assembly.GetExecutingAssembly())            // MediatR

                .BuildServiceProvider();


        /// <summary>
        /// Container to be used with TinyIoC
        /// </summary>
        internal static TinyIoCContainer DefaultTinyIoCContainer
        {
            get
            {
                TinyIoCContainer container = new TinyIoCContainer();


                container.Register<ILoggerFactory>(LoggerFactory.Create(loggingBuilder =>
                {
                    loggingBuilder.AddDebug();
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                }));

                container.AutoRegister(new List<Assembly>() { typeof(LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler).GetTypeInfo().Assembly });

                container.Register<IMediator>((x, overloads) => new Mediator(x.Resolve));
                container.Register<IMediatorService, MediatorWrapper>();

                //container.Register(typeof(INotificationHandler<>), typeof(NotificationWrapper<>));
                container.Register<ITrainingProgramRepository, SQLTrainingPlanRepository>();

                //container.Register<INotificationHandler<>, LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler>();

                //container.Register(typeof(IDomainNotificationHandler<>), typeof(LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler));
                //container.Register(typeof(INotificationHandler<>), typeof(IDomainNotificationHandler<>));

                //container.Register(typeof(INotificationHandler<INotification>), typeof(LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler)).AsMultiInstance();
                //container.Register(typeof(IDomainNotificationHandler<>), typeof(NotificationHandlerWrapper<>));

                return container;
            }
        }

        internal static AutofacServiceProvider DefaultAutofacServiceProvider
        {
            get
            {
                var container = new ContainerBuilder();
                //container.Populate(services);

                container.RegisterInstance(LoggerFactory.Create(loggingBuilder =>
                    {
                        loggingBuilder.AddDebug();
                        loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                    }))
                    .As<ILogger>();

                container.RegisterModule(new MediatorModule());

                return new AutofacServiceProvider(container.Build());
            }
        }

        internal static IContainer DefaultAutofacContainer
        {
            get
            {
                ContainerBuilder container = new ContainerBuilder();
                //container.Populate(services);

                container.RegisterInstance(LoggerFactory.Create(loggingBuilder =>
                    {
                        loggingBuilder.AddDebug();
                        loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                    }))
                    .As<ILoggerFactory>();

                container.RegisterModule(new MediatorModule());
                container.RegisterModule(new DatabaseModule());

                return container.Build();
            }
        }


    }


    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
            
            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            //builder.RegisterAssemblyTypes(typeof(CreateOrderCommand).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
            builder.RegisterAssemblyTypes(typeof(LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler)
                .GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Add Behaviors here
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(ValidatorBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerDependency();

            //builder.RegisterType(typeof(IPipelineBehavior<,>)).As(typeof(TransactionBehaviour<,>)).InstancePerDependency();



            //builder.RegisterAssemblyTypes(typeof(MediatorWrapper).GetTypeInfo().Assembly)
            //    .AsImplementedInterfaces().InstancePerLifetimeScope();

            //builder.RegisterGeneric(typeof(NotificationHandlerWrapper<,>)).As(typeof(INotificationHandler<>));


            //builder.RegisterType(typeof(NotificationWrapper<>)).AsImplementedInterfaces();
            //builder.RegisterGeneric(typeof(NotificationHandlerWrapper<,>)).AsImplementedInterfaces();


            //builder.RegisterType<NotificationHandlerWrapper>().AsImplementedInterfaces();

            // Register the Command's Validators (Validators based on FluentValidation library)
            //builder
            //    .RegisterAssemblyTypes(typeof(CreateOrderCommandValidator).GetTypeInfo().Assembly)
            //    .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
            //    .AsImplementedInterfaces();


            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            //builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }


    public class DatabaseModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            DbContextOptions options = new DbContextOptionsBuilder<GymContext>()
                .UseSqlite(@"DataSource=C:\Users\rigom\source\repos\GymProject\GymProject.Infrastructure\test.db;")
                .Options;

            builder.RegisterType<GymContext>()
                .WithParameter("options", options)
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(SQLTrainingPlanRepository)
                .GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
        }
    }


}
