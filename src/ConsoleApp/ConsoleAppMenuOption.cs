using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class ConsoleAppMenuOption
    {
        public string Key { get; }
        public string Name { get; }
        public Func<Task> Method { get; }

        public ConsoleAppMenuOption(string key, string name, Func<Task> method)
        {
            Key = key;
            Name = name;
            Method = method;
        }
    }
}
