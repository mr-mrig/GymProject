//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;

//namespace GymProject.Application.Behaviors
//{

//    public sealed class DatabaseRetryBehavior<TCommand> : IRequestHandler<TCommand>
//    where TCommand : IRequest
//    {
//        private readonly IRequestHandler<TCommand> _handler;
//        //private readonly Config _config;

//        public DatabaseRetryBehavior(IRequestHandler<TCommand> handler)
//        {
//            //_config = config;
//            _handler = handler;
//        }

//        public async bool Handle(TCommand command, CancellationToken cancellationToken)
//        {
//            for (int i = 0; ; i++)
//            {
//                try
//                {
//                    return await _handler.Handle(command, cancellationToken);
//                }
//                catch (Exception ex)
//                {
//                    //if (i >= _config.NumberOfDatabaseRetries || !IsDatabaseException(ex))
//                    if (i >= _config.NumberOfDatabaseRetries || !IsDatabaseException(ex))
//                    {
//                        // Compensation Commands, if any
//                        throw;
//                    }
//                }
//            }
//        }

//        private bool IsDatabaseException(Exception exception)
//        {
//            string message = exception.InnerException?.Message;

//            if (message == null)
//                return false;

//            return message.Contains("The connection is broken and recovery is not possible")
//                || message.Contains("error occurred while establishing a connection");
//        }
//    }
//}
