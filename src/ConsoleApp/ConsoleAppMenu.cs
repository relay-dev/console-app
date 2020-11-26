using ConsoleApp.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public abstract class ConsoleAppMenu : IConsoleAppMenu
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            bool isExit = false;

            List<ConsoleAppMenuOption> consoleAppMenuOptions = DiscoverConsoleAppMenuOptions();

            while (!isExit)
            {
                DisplayOptionsMenu(consoleAppMenuOptions);

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                string selection = consoleKeyInfo.KeyChar.ToString();

                if (selection.ToLower() == "h")
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

                        Execute(async () =>
                        {
                            await consoleAppMenuOption.ValueFactory.Invoke();
                        });

                        Console.WriteLine("{0}Press any key to continue...", Environment.NewLine);
                    }

                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static void Execute(Action action)
        {
            try
            {
                action.Invoke();

                Console.WriteLine();
                Console.WriteLine("***Complete***");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine($"ERROR: {e.Message}");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
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

            Console.WriteLine("{0}Enter {1}{2} (enter 'h' to return to the home menu)", Environment.NewLine, consoleAppMenuOptions.Min(s => s.Key), consoleAppMenuOptions.Count == 1 ? string.Empty : " - " + consoleAppMenuOptions.Max(sm => sm.Key));
        }
    }
}
