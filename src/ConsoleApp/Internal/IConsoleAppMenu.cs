using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Internal
{
    internal interface IConsoleAppMenu
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
