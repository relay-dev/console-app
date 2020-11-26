using System;

namespace ConsoleApp.Internal
{
    internal class ConsoleAppMenu
    {
        public string Key { get; }
        public string Name { get; }
        public Type Type { get; }

        public ConsoleAppMenu(string key, string name, Type type)
        {
            Key = key;
            Name = name;
            Type = type;
        }
    }
}
