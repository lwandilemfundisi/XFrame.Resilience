using XFrame.ValueObjects;

namespace XFrame.Resilience
{
    public class Repeat : ValueObject
    {
        public static Repeat Yes { get; } = new Repeat(true, TimeSpan.Zero);

        public static Repeat YesAfter(TimeSpan repeatAfter) => new Repeat(true, repeatAfter);

        public static Repeat No { get; } = new Repeat(false, TimeSpan.Zero);

        public bool CanRepeat { get; }

        public TimeSpan RepeatAfter { get; }

        private Repeat(bool canRepeat, TimeSpan repeatAfter)
        {
            if (repeatAfter != TimeSpan.Zero && repeatAfter != repeatAfter.Duration())
                throw new ArgumentOutOfRangeException(nameof(repeatAfter));
            if (!canRepeat && repeatAfter != TimeSpan.Zero)
                throw new ArgumentException("Invalid combination. Should not be repeated and repeated after set");

            CanRepeat = canRepeat;
            RepeatAfter = repeatAfter;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CanRepeat;
            yield return RepeatAfter;
        }
    }
}
