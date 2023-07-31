using XFrame.Common.Exceptions;

namespace XFrame.Resilience
{
    public class OptimisticConcurrencyResilientStrategy : IOptimisticConcurrencyResilientStrategy
    {
        private readonly ISetup _setup;

        public OptimisticConcurrencyResilientStrategy(
            ISetup setup)
        {
            _setup = setup;
        }

        public Repeat CheckRetry(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount)
        {
            if (!(exception is OptimisticConcurrencyException))
            {
                return Repeat.No;
            }

            return _setup.RetryCountOnOptimisticConcurrencyExceptions >= currentRetryCount
                ? Repeat.YesAfter(_setup.WaitThenTryAfterOnOptimisticConcurrencyExceptions)
                : Repeat.No;
        }
    }
}
