using System;

namespace ConsoleApp
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConsoleAppMenuAttribute : Attribute
    {
        /// <summary>
        /// The key of the menu item that will display on the output window
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The key of the menu item that will display on the output window
        /// </summary>
        public string Name { get; set; }
    }
}
