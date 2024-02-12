using Spectre.Console;
using Spectre.Console.Cli;

namespace Sputter.Console;

public class SputterSettings : CommandSettings {

    public override ValidationResult Validate() {
        if (!string.IsNullOrWhiteSpace(ScrutinyApiAddress) && !Uri.TryCreate(ScrutinyApiAddress, UriKind.Absolute, out var _)) {
            return ValidationResult.Error("Failed to parse Scrutiny API address!");
        }
        return base.Validate();
    }

    [CommandArgument(0, "[filter]")]
    public string? DriveFilter { get; set; }

    [CommandOption("--scrutiny-api")]
    public string? ScrutinyApiAddress { get; set; }
}
