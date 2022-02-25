using NecroCloud.Services;

ServiceLocator.Current.RegisterSingleton<ILogger, Logger>();
ServiceLocator.Current.RegisterService<FileMonitor>();