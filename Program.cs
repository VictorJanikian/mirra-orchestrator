using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextToImage;
using Mirra_Orchestrator.Integration;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Repository;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Repository.Repositories;
using Mirra_Orchestrator.Service;
using Mirra_Orchestrator.Service.Interfaces;
using System.Diagnostics;
using System.Net.Http.Headers;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {

        config.SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddUserSecrets<Program>()
              .AddEnvironmentVariables();

    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();


        services.AddDbContext<DatabaseContext>(options =>
        {

            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

#if DEBUG
            options.EnableSensitiveDataLogging()  // Enable detailed logging, including SQL parameters (only in Debug mode)
                   .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)  // Log SQL queries to the console
                   .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information);  // Log to the debug window as well
#endif
        });

        services.AddScoped<ISchedulingRepository, SchedulingRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();

        services.AddScoped<ISchedulingService, SchedulingService>();
        services.AddScoped<IOrchestrationService, OrchestrationService>();
        services.AddScoped<IContentGenerationService, ContentGenerationService>();
        services.AddScoped<IPromptFormatterService, PromptFormatterService>();
        services.AddScoped<IModelCommunicationService, ModelCommunicationService>();
        services.AddScoped<IPreviousContentRecoveryService, PreviousContentRecoveryService>();
        services.AddScoped<IModelResponseFormatter, ModelResponseFormatter>();

        services.AddScoped<IWordpressIntegration, WordpressIntegration>();
        services.AddScoped<IOpenAIIntegration, OpenAIIntegration>();
        services.AddScoped<IRestClient, RestClient>();

        services.AddSingleton<Kernel>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();

            return Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    "gpt-4o",
                    configuration["AI:AzureOpenAI:Endpoint"],
                    configuration["AI:AzureOpenAI:ApiKey"]
                )
                .AddOpenAITextToImage(
                    configuration["AI:OpenAI:ApiKey"],
                    modelId: "gpt-image-1-mini"
                )
                .Build();
        });

        services.AddSingleton<IChatCompletionService>(provider =>
        {
            var kernel = provider.GetRequiredService<Kernel>();

            return kernel.GetRequiredService<IChatCompletionService>();
        });

        services.AddSingleton<ITextToImageService>(provider =>
        {
            var kernel = provider.GetRequiredService<Kernel>();

            return kernel.GetRequiredService<ITextToImageService>();
        });

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddHttpClient("wordpress", client =>
        {
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

    })
   .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })


    .Build();

host.Run();
