using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Sputter.Console;
using Sputter.Core;
using Sputter.DBus;
using Sputter.HWMon;
using Sputter.Messaging;
using Sputter.MQTT;
using System.Diagnostics.CodeAnalysis;

internal class Program {

    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicNestedTypes, typeof(MeasureCommand))]
    private static async Task<int> Main(string[] args) {
        var services = new ServiceCollection();
        services.AddSputterDefaults([typeof(DBusAdapter), typeof(HWMonAdapter)]);
        services.AddMediatR(m => m.AddSputterComponents(s => s.EnableAggregation()));
		services.AddSputterPlugins();
        //services.AddSputterWithMediatR(c => c.EnableAggregation());
        // add extra services to the container here
        using var registrar = new DependencyInjectionRegistrar(services);
        var app = new CommandApp<MeasureCommand>(registrar);
        return await app.RunAsync(args);
    }
}