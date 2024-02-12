using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sputter.Core;

namespace Sputter.Messaging;

public static class SputterMediatRConfigurationExtensions {
    public static IServiceCollection AddSputterDefaults(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) {
        services.Add(new ServiceDescriptor(typeof(IMeasurementService), typeof(DriveMeasurementService), lifetime));
        return services;
    }

    public static IServiceCollection AddSputterDefaults(this IServiceCollection services, IEnumerable<Type> adapterTypes, ServiceLifetime lifetime = ServiceLifetime.Transient) {
        var adapters = adapterTypes.Where(t => t.IsAssignableTo(typeof(IDriveSensorAdapter)));
        foreach (var item in adapters) {
            services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), item, lifetime));
        }
        services.Add(new ServiceDescriptor(typeof(IMeasurementService), typeof(DriveMeasurementService), lifetime));
        return services;
    }

    public static IServiceCollection AddSputterAdapter<TAdapter>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) where TAdapter : IDriveSensorAdapter {
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter), lifetime));
        return services;
    }

    public static IServiceCollection AddSputter<TAdapter>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) where TAdapter : IDriveSensorAdapter {
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter), lifetime));
        return services;
    }

    public static IServiceCollection AddSputter<TAdapter, TAdapter2>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) where TAdapter : IDriveSensorAdapter {
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter), lifetime));
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter2), lifetime));
        return services;
    }

    public static IServiceCollection AddSputter<TAdapter, TAdapter2, TAdapter3>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) where TAdapter : IDriveSensorAdapter {
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter), lifetime));
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter2), lifetime));
        services.Add(new ServiceDescriptor(typeof(IDriveSensorAdapter), typeof(TAdapter3), lifetime));
        return services;
    }

    public static IServiceCollection AddSputterWithMediatR(this IServiceCollection services, Action<SputterMediatRConfiguration>? configureAction = null, Action<MediatRServiceConfiguration>? mediatRConfig = null) {
        var serviceConfig = ConfigureSputterServices(services, ref configureAction);
        mediatRConfig ??= m => { };
        return services.AddMediatR(m => {
            mediatRConfig.Invoke(m);
            m.AddSputterComponents(serviceConfig);
        });
    }

    private static SputterMediatRConfiguration ConfigureSputterServices(IServiceCollection services, ref Action<SputterMediatRConfiguration>? configureAction) {
        configureAction ??= conf => { };
        var serviceConfig = new SputterMediatRConfiguration();
        configureAction.Invoke(serviceConfig);
        if (serviceConfig.EnableDefaults) {
            if (serviceConfig.AsSingletons) {
                services.AddSingleton<IMeasurementService, DriveMeasurementService>();
            } else {
                services.AddScoped<IMeasurementService, DriveMeasurementService>();
            }
        }

        return serviceConfig;
    }

    internal static MediatRServiceConfiguration AddSputterComponents(this MediatRServiceConfiguration configuration, SputterMediatRConfiguration sputterConfig) {
        configuration.RegisterServicesFromAssemblyContaining<SputterMediatRConfiguration>();
        if (sputterConfig.AddAggregator) {
            configuration.AddBehavior<IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>>, MeasurementAggregatorBehaviour>();
        }
        return configuration;
    }

    public static MediatRServiceConfiguration AddSputterComponents(this MediatRServiceConfiguration configuration, Action<SputterMediatRConfiguration>? sputterConfig = null) {
        sputterConfig ??= conf => { };
        var serviceConfig = new SputterMediatRConfiguration();
        sputterConfig.Invoke(serviceConfig);
        configuration.AddSputterComponents(sputterConfig);
        return configuration;
    }
}
