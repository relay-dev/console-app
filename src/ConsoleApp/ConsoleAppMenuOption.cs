using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class ConsoleAppMenuOption
    {
        public string Key { get; }
        public string Name { get; }
        public Func<Task> ValueFactory { get; }

        public ConsoleAppMenuOption(string key, string name, Func<Task> valueFactory)
        {
            Key = key;
            Name = name;
            ValueFactory = valueFactory;
        }
    }
}
