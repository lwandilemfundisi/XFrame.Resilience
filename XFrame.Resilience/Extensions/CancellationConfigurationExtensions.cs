namespace XFrame.Resilience.Extensions
{
    public static class CancellationConfigurationExtensions
    {
        public static CancellationToken Limit(
            this ICancellationConfiguration configuration, 
            CancellationToken token, 
            CancellationBoundary currentBoundary)
        {
            token.ThrowIfCancellationRequested();
            return currentBoundary < configuration.CancellationBoundary ? token : CancellationToken.None;
        }
    }
}
