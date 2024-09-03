using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(config =>
    {
        config.AddUserSecrets<Program>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService(options =>
        {
            options.ConnectionString = context.Configuration["ApplicationInsights:ConnectionString"];
        });
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton(s =>
        {
            var connectionString = context.Configuration["AzureStorage:ConnectionString"];

            return new QueueServiceClient(connectionString);
        });
    })
    .Build();


host.Run();
