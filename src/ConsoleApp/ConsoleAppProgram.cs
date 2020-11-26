using ConsoleApp.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class ConsoleAppProgram<TStartup> where TStartup : IConsoleAppStartup
    {
        protected static async Task RunAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // Build configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", true)
                .AddJsonFile("C:\\Azure\\appsettings.KeyVault.json", true, true)
                .AddJsonFile("appsettings.Local.json", true, true)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            // Initialize a ServiceCollection
            var services = new ServiceCollection();

            // Add Configuration
            services.AddSingleton<IConfiguration>(config);

            // Add Logging
            services
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(config.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                });

            // Add Processors
            AddConsoleAppMenuTypes(services);

            // Create the Startup
            IConsoleAppStartup startup = (TStartup)Activator.CreateInstance(typeof(TStartup), new object[] { config });

            if (startup == null)
            {
                throw new InvalidOperationException($"Could not create an instance of startup class of type '{typeof(TStartup).FullName}'");
            }

            // Configure services
            startup.ConfigureServices(services);

            // Build the IServiceProvider
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Create the program
            var application = new ConsoleAppProgramRunner(serviceProvider);

            try
            {
                // Run the program
                await application.RunAsync(cancellationToken);

                Console.Clear();
                Console.WriteLine("The program has ended");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("Encountered unhandled exception:");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static IServiceCollection AddConsoleAppMenuTypes(IServiceCollection services)
        {
            var types = new AssemblyScanner().GetTypesWithAttribute<ConsoleAppMenuAttribute>();

            foreach (Type type in types)
            {
                services.AddTransient(type);
            }

            return services;
        }

        private static string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }
}
