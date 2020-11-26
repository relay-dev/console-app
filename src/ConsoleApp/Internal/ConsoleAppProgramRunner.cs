using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Internal
{
    internal class ConsoleAppProgramRunner
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsoleAppProgramRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            bool isExit = false;

            List<ConsoleAppMenu> consoleAppMenus = DiscoverConsoleAppMenus();

            try
            {
                while (!isExit)
                {
                    DisplayHomeMenu(consoleAppMenus);

                    string selection = Console.ReadLine();

                    if (selection?.ToUpper() == "X")
                    {
                        isExit = true;
                    }
                    else
                    {
                        ConsoleAppMenu consoleAppMenu =
                            consoleAppMenus.SingleOrDefault(s => string.Equals(s.Key, selection, StringComparison.OrdinalIgnoreCase));

                        if (consoleAppMenu == null)
                        {
                            Console.Clear();
                            Console.WriteLine("Invalid selection! Please try again (press any key to continue)");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Clear();

                            var consoleAppMenuToRun = _serviceProvider.GetRequiredService(consoleAppMenu.Type);

                            if (!consoleAppMenuToRun.GetType().GetInterfaces().Contains(typeof(IConsoleAppMenuRunner)) && !consoleAppMenuToRun.GetType().GetInterfaces().Contains(typeof(IConsoleAppMenuRunnerAsync)))
                            {
                                Console.WriteLine("Error! That selection is not a ConsolaterApp (press any key to continue)");
                                Console.ReadKey();
                                Console.Clear();
                            }
                            else if (consoleAppMenuToRun.GetType().GetInterfaces().Contains(typeof(IConsoleAppMenuRunnerAsync)))
                            {
                                await ((IConsoleAppMenuRunnerAsync)consoleAppMenuToRun).RunAsync(cancellationToken);
                            }
                            else
                            {
                                ((IConsoleAppMenuRunner)consoleAppMenuToRun).Run();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine($"Encountered unhandled exception{Environment.NewLine}{Environment.NewLine}'{e.Message}'");
            }
        }

        private List<ConsoleAppMenu> DiscoverConsoleAppMenus()
        {
            List<Type> consoleAppMenuTypes = new AssemblyScanner()
                .GetTypesWithAttribute<ConsoleAppMenuAttribute>()
                .OrderBy(t => t.Name)
                .ToList();

            var consoleAppMenus = new List<ConsoleAppMenu>();

            for (int i = 0; i < consoleAppMenuTypes.Count; i++)
            {
                Type t = consoleAppMenuTypes[i];

                ConsoleAppMenuAttribute consoleAppMenuAttribute =
                    (ConsoleAppMenuAttribute)t.GetCustomAttributes(typeof(ConsoleAppMenuAttribute), true).Single();

                consoleAppMenus.Add(new ConsoleAppMenu(consoleAppMenuAttribute.Key ?? (i + 1).ToString(), consoleAppMenuAttribute.Name ?? t.Name, t));
            }

            return consoleAppMenus;
        }

        private static void DisplayHomeMenu(List<ConsoleAppMenu> consoleAppMenus)
        {
            Console.WriteLine("Please make a selection:{0}", Environment.NewLine);

            foreach (ConsoleAppMenu consoleAppMenu in consoleAppMenus)
            {
                Console.WriteLine(" ({0}) {1}", consoleAppMenu.Key, consoleAppMenu.Name);
            }

            Console.WriteLine("{0}Select {1} - {2} and press Enter (enter ( X ) or ( x ) and press Enter to exit)", Environment.NewLine, consoleAppMenus.Min(s => s.Key), consoleAppMenus.Max(s => s.Key));
        }
    }
}
