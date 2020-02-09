using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using GymProject.Infrastructure.Persistence.EFContext;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Application.Behaviors
{

    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {


        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly GymContext _dbContext;




        public TransactionBehavior(GymContext dbContext,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }



        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetType().Name;
            
            try
            {
                if (_dbContext.HasActiveTransaction())
                {
                    return await next();
                }

                IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync())
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    // Dispatch Integration Events
                    //await _integrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
