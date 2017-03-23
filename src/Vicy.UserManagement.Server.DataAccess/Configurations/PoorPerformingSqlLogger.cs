using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Vicy.UserManagement.Server.DataAccess.Configurations
{
    public class PoorPerformingSqlLogger : IDbCommandInterceptor
    {
        private readonly ILogger _logger;
        private readonly int _thresholdInMilliseconds;

        public PoorPerformingSqlLogger(
            ILoggerFactory loggerFactory,
            int thresholdInMilliseconds)
        {
            _logger = loggerFactory.CreateLogger(nameof(PoorPerformingSqlLogger));

            _thresholdInMilliseconds = thresholdInMilliseconds;
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Executed(command, interceptionContext);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Executing(interceptionContext);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Executed(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Executing(interceptionContext);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Executed(command, interceptionContext);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Executing(interceptionContext);
        }

        private void Executing<T>(DbCommandInterceptionContext<T> interceptionContext)
        {
            var timer = new Stopwatch();
            interceptionContext.UserState = timer;
            timer.Start();
        }

        private void Executed<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            var timer = (Stopwatch)interceptionContext.UserState;
            timer.Stop();

            if (interceptionContext.Exception != null)
            {
                _logger.LogInformation("Execute SQL command failed.");
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(command.CommandText);
                    _logger.LogDebug(Environment.StackTrace);
                }
            }
            else if (timer.ElapsedMilliseconds >= _thresholdInMilliseconds)
            {
                _logger.LogInformation(
                    $"Slow SQL command detected: {timer.ElapsedMilliseconds} ms (>= {_thresholdInMilliseconds} ms).");
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(command.CommandText);
                    _logger.LogDebug(Environment.StackTrace);
                }
            }
        }
    }
}
