namespace XFrame.Resilience
{
    public class RepeatDelay
    {
        private static readonly Random Random = new Random();

        public static RepeatDelay Between(TimeSpan min, TimeSpan max)
        {
            return new RepeatDelay(min, max);
        }

        private RepeatDelay(
            TimeSpan min,
            TimeSpan max)
        {
            if (min.Ticks < 0) throw new ArgumentOutOfRangeException(nameof(min), "Minimum cannot be negative");
            if (max.Ticks < 0) throw new ArgumentOutOfRangeException(nameof(max), "Maximum cannot be negative");

            Min = min;
            Max = max;
        }

        public TimeSpan Max { get; }
        public TimeSpan Min { get; }

        public TimeSpan PickDelay()
        {
            return Min.Add(TimeSpan.FromMilliseconds((Max.TotalMilliseconds - Min.TotalMilliseconds) * Random.NextDouble()));
        }
    }
}
