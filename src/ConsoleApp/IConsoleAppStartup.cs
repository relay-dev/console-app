using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    public interface IConsoleAppStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}