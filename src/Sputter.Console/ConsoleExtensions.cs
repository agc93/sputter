using Spectre.Console;

namespace Sputter.Console;
internal static class ConsoleExtensions {
    internal static Table AddIfSet(this Table table, string key, string? value) {
        if (value != null) {
            table.AddRow(key, value);
        }
        return table;
    }
}
