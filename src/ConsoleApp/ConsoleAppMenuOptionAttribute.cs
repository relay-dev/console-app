using System;

namespace ConsoleApp
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ConsoleAppMenuOptionAttribute : Attribute
    {
        /// <summary>
        /// The key of the menu option that will display on the output window
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The key of the menu option that will display on the output window
        /// </summary>
        public string Name { get; set; }
    }
}
