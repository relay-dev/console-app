using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Internal
{
    internal interface IConsoleAppMenuRunnerAsync
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
