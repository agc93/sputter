using Microsoft.Extensions.Options;
using Sputter.Core;
using Sputter.MQTT;
using Sputter.Server.Configuration;

namespace Sputter.Server;

internal static class OptionsExtensions {
    private static readonly Debouncer _debouncer = new(TimeSpan.FromSeconds(2));
    public static IDisposable? OnChangeDedup<TOptions>(
            this IOptionsMonitor<TOptions> monitor,
            string name,
            Action<TOptions, string?> listener,
            Func<TOptions, byte[]> checksumFunc) {
        var originValueHashToken = checksumFunc(monitor.Get(name));
        return monitor.OnChange((newValue, key) =>
        {
            if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase)) {
                var newValueHashToken = checksumFunc(newValue);
                var oldValueHashToken = Interlocked.Exchange(
                    ref originValueHashToken,
                    newValueHashToken);

                if (!IsHashTokenEqual(oldValueHashToken, newValueHashToken)) {
                    listener(newValue, key);
                }
            }
        });
    }

    public static IDisposable? OnChangeDebounce<TOptions>(
            this IOptionsMonitor<TOptions> monitor,
            string name,
            Action<TOptions, string?> listener) {
        return monitor.OnChange((newValue, key) => {
            if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase)) {
                _debouncer.Debounce(() => {
                    listener(newValue, key);
                });
            }
        });
    }

    public static IDisposable? OnChangeDedup(
        this IOptionsMonitor<ServerConfiguration> monitor,
        Action<ServerConfiguration> listener) {
        return OnChangeDedup(
            monitor,
            Options.DefaultName,
            (options, _) => listener(options),
            CacheExtensions.GetChecksumBytes);
    }

    public static IDisposable? OnChangeDedup(
        this IOptionsMonitor<MQTTConfiguration> monitor,
        Action<MQTTConfiguration> listener){
        return OnChangeDedup(
            monitor,
            Options.DefaultName,
            (options, _) => listener(options),
            CacheExtensions.GetChecksumBytes);
    }

    private static bool IsHashTokenEqual(byte[] lhs, byte[] rhs) {
        return System.Linq.Enumerable.SequenceEqual(lhs, rhs);
    }
}