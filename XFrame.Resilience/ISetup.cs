namespace XFrame.Resilience
{
    public interface ISetup
    {
        int RetryCountOnOptimisticConcurrencyExceptions { get; }

        TimeSpan WaitThenTryAfterOnOptimisticConcurrencyExceptions { get; }

        bool ThrowSubscriberExceptions { get; }

        bool IsAsynchronousSubscribersEnabled { get; }

        CancellationBoundary CancellationBoundary { get; }
    }
}
