namespace XFrame.Resilience
{
    public enum CancellationBoundary
    {
        BeforeUpdatingAggregate,
        BeforeCommittingEvents,
        BeforeNotifyingSubscribers,
        CancelAlways
    }
}
