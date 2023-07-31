namespace XFrame.Resilience
{
    public interface IResilientStrategy
    {
        Repeat CheckRetry(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount);
    }
}
