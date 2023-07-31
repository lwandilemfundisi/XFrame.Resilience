namespace XFrame.Resilience
{
    public interface ICancellationConfiguration
    {
        CancellationBoundary CancellationBoundary { get; }
    }
}
