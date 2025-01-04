using System;
using SimpleSnake.Core;
using SimpleSnake.GameObjects;
using SimpleSnake.Interfaces;

namespace SimpleSnake
{
    using Utilities;

    public class StartUp
    {
        public static void Main()
        {
            ConsoleWindow.CustomizeConsole();

            var consoleOperator = new ConsoleOperator();
            IEngine engine = new Engine(new Playground(40, 20), consoleOperator, consoleOperator);
            engine.Run();
        }
    }
}
