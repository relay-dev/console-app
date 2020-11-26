using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Internal
{
    internal abstract class ConsoleAppMenuRunnerAsync : IConsoleAppMenuRunnerAsync
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            bool isExit = false;

            List<ConsoleAppMenuOption> consoleAppMenuOptions = DiscoverConsoleAppMenuOptions();

            while (!isExit)
            {
                DisplayOptionsMenu(consoleAppMenuOptions);

                string selection = Console.ReadLine();

                if (selection?.ToUpper() == "H")
                {
                    Console.Clear();
                    isExit = true;
                }
                else
                {
                    ConsoleAppMenuOption consoleAppMenuOption =
                        consoleAppMenuOptions.SingleOrDefault(s => string.Equals(s.Key, selection, StringComparison.OrdinalIgnoreCase));

                    if (consoleAppMenuOption == null)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid selection! Please try again (press any key to continue)");
                    }
                    else
                    {
                        Console.Clear();

                        await consoleAppMenuOption.ValueFactory.Invoke();

                        Console.WriteLine("{0}Press any key to continue...", Environment.NewLine);
                    }

                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private List<ConsoleAppMenuOption> DiscoverConsoleAppMenuOptions()
        {
            List<MethodInfo> methodInfos = this.GetType().GetMethods()
                .Where(m => m.GetCustomAttribute<ConsoleAppMenuOptionAttribute>() != null)
                .OrderBy(mi => mi.Name)
                .ToList();

            var consoleAppMenuOptions = new List<ConsoleAppMenuOption>();

            for (int i = 0; i < methodInfos.Count; i++)
            {
                MethodInfo m = methodInfos[i];

                ConsoleAppMenuOptionAttribute consoleAppMenuOptionAttribute =
                    (ConsoleAppMenuOptionAttribute)m.GetCustomAttributes(typeof(ConsoleAppMenuOptionAttribute), true).Single();

                consoleAppMenuOptions.Add(new ConsoleAppMenuOption(consoleAppMenuOptionAttribute.Key ?? (i + 1).ToString(), consoleAppMenuOptionAttribute.Name ?? m.Name + "()", async () => await (Task)m.Invoke(this, null)));
            }

            return consoleAppMenuOptions.OrderBy(sm => sm.Key).ToList();
        }

        protected void DisplayOptionsMenu(List<ConsoleAppMenuOption> consoleAppMenuOptions)
        {
            Console.WriteLine("Please make a selection: {0}", Environment.NewLine);

            foreach (ConsoleAppMenuOption consoleAppMenuOption in consoleAppMenuOptions)
            {
                Console.WriteLine(" ({0}) {1}", consoleAppMenuOption.Key, consoleAppMenuOption.Name);
            }

            Console.WriteLine("{0}Select 1{1} and press Enter (enter ( H ) or ( h ) to return to the home menu)", Environment.NewLine, consoleAppMenuOptions.Count == 1 ? string.Empty : " - " + consoleAppMenuOptions.Max(sm => sm.Key));
        }
    }
}
