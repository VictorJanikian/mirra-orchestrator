using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mirra_Orchestrator.Integration;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Repository;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service;
using Mirra_Orchestrator.Service.Interfaces;
using System.Diagnostics;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();


        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySQL(configuration.GetConnectionString("DefaultConnection"));

#if DEBUG
            options.EnableSensitiveDataLogging()  // Enable detailed logging, including SQL parameters (only in Debug mode)
                   .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)  // Log SQL queries to the console
                   .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information);  // Log to the debug window as well
#endif
        });

        services.AddSingleton<ISchedulingRepository, SchedulingRepository>();
        services.AddSingleton<ISchedulingService, SchedulingService>();
        services.AddSingleton<IWordpressIntegration, WordpressIntegration>();
        services.AddSingleton<IRestClient, RestClient>();

    })
   .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })


    .Build();

host.Run();
