using NecroCloud.Services;

// Service and Singletons Initialization Order Matters!

ServiceLocator.Current.RegisterSingleton<ILogger, Logger>();

ServiceLocator.Current.RegisterSingleton<IFileHelper, FileHelper>();

ServiceLocator.Current.RegisterSingleton<IFileWatcher, FileWatcher>();
ServiceLocator.Current.GetSingleton<IFileWatcher>().StartSubRoutine();

ServiceLocator.Current.RegisterService<FileMonitor>();

Console.WriteLine("Press Any Key to Exit");

Console.ReadKey(true);