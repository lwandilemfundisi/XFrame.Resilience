using XFrame.Common;

namespace XFrame.Resilience
{
    public interface ITransientFaultHandler<out TResilientStrategy>
        where TResilientStrategy : IResilientStrategy
    {
        void ConfigureResilientStrategy(Action<TResilientStrategy> configureStrategy);

        Task TryAsync(
            Func<CancellationToken, Task> action,
            Label label,
            CancellationToken cancellationToken);

        Task<T> TryAsync<T>(
            Func<CancellationToken, Task<T>> action,
            Label label,
            CancellationToken cancellationToken);
    }
}
