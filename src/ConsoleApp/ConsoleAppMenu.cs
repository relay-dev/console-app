using System;

namespace ConsoleApp
{
    public class ConsoleAppMenu
    {
        public string Key { get; }
        public string Name { get; }
        public Type MenuItemType { get; }

        public ConsoleAppMenu(string key, string name, Type menuItemType)
        {
            Key = key;
            Name = name;
            MenuItemType = menuItemType;
        }
    }
}
