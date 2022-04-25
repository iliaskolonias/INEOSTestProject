using INEOSTestProject;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "INEOS Test .NET Service";
    })
     .ConfigureServices(services =>
    {
        services.AddSingleton<Services.IPowerService, Services.PowerService>();
        services.AddSingleton<INEOSTestService>();
        services.AddHostedService<INEOSTestBackgroundService>();
    })
    .Build();

await host.RunAsync();
