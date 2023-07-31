using Microsoft.Extensions.Logging;
using System.Diagnostics;
using XFrame.Common;
using XFrame.Common.Extensions;

namespace XFrame.Resilience
{
    public class TransientFaultHandler<TResilientStrategy> : ITransientFaultHandler<TResilientStrategy>
        where TResilientStrategy : IResilientStrategy
    {
        private readonly ILogger<TransientFaultHandler<TResilientStrategy>> _logger;
        private readonly TResilientStrategy _resilientStrategy;

        public TransientFaultHandler(
            ILogger<TransientFaultHandler<TResilientStrategy>> logger,
            TResilientStrategy resilientStrategy)
        {
            _logger = logger;
            _resilientStrategy = resilientStrategy;
        }

        public void ConfigureResilientStrategy(Action<TResilientStrategy> setupResilientStrategy)
        {
            if (setupResilientStrategy == null)
            {
                throw new ArgumentNullException(nameof(setupResilientStrategy));
            }

            setupResilientStrategy(_resilientStrategy);
        }

        public Task TryAsync(
            Func<CancellationToken, Task> action,
            Label label,
            CancellationToken cancellationToken)
        {
            return TryAsync<object>(
                async c =>
                {
                    await action(c).ConfigureAwait(false);
                    return null;
                },
                label,
                cancellationToken);
        }

        public async Task<T> TryAsync<T>(Func<CancellationToken, Task<T>> action, Label label, CancellationToken cancellationToken)
        {
            if (_resilientStrategy == null)
            {
                throw new InvalidOperationException("You need to setup the resilient strategy");
            }

            var stopwatch = Stopwatch.StartNew();
            var currentRetryCount = 0;

            while (true)
            {
                Exception currentException;
                Repeat retry;
                try
                {
                    var result = await action(cancellationToken).ConfigureAwait(false);

                    if(_logger.IsEnabled(LogLevel.Trace))
                    _logger.LogTrace(
                        "Finished execution of '{0}' after {1} retries and {2:0.###} seconds",
                        label,
                        currentRetryCount,
                        stopwatch.Elapsed.TotalSeconds);

                    return result;
                }
                catch (Exception exception)
                {
                    currentException = exception;
                    var currentTime = stopwatch.Elapsed;
                    retry = _resilientStrategy.CheckRetry(currentException, currentTime, currentRetryCount);
                    if (!retry.CanRepeat)
                    {
                        throw;
                    }
                }

                currentRetryCount++;
                if (retry.RepeatAfter != TimeSpan.Zero)
                {
                    if (_logger.IsEnabled(LogLevel.Trace))
                        _logger.LogTrace(
                        "Exception {0} with message '{1} 'is transient, retrying action '{2}' after {3:0.###} seconds for retry count {4}",
                        currentException.GetType().PrettyPrint(),
                        currentException.Message,
                        label,
                        retry.RepeatAfter.TotalSeconds,
                        currentRetryCount);
                    await Task.Delay(retry.RepeatAfter, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Trace))
                        _logger.LogTrace(
                        "Exception {0} with message '{1}' is transient, retrying action '{2}' NOW for retry count {3}",
                        currentException.GetType().PrettyPrint(),
                        currentException.Message,
                        label,
                        currentRetryCount);
                }
            }
        }
    }
}
