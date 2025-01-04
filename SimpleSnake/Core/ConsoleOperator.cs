using System;
using System.Drawing;
using SimpleSnake.Enums;
using SimpleSnake.GameObjects;
using SimpleSnake.Interfaces;
using Point = SimpleSnake.GameObjects.Point;

namespace SimpleSnake.Core
{
    public class ConsoleOperator : IWriter, IReader
    {
        public void Write(Point point, char symbol)
        {
            SetCursorPosition(point);
            Console.Write(symbol);
        }

        public void Write(Point point, string text)
        {
            SetCursorPosition(point);
            Console.Write(text);
        }

        private static void SetCursorPosition(Point point)
        {
            Console.CursorLeft = point.X;
            Console.CursorTop = point.Y;
        }

        public bool TryReadDirection(out Direction? direction)
        {
            direction = null;

            if (!Console.KeyAvailable) return false;
            var keyInfo = Console.ReadKey(intercept: true);

            direction = keyInfo.Key switch
            {
                ConsoleKey.W or ConsoleKey.UpArrow => Direction.Up,
                ConsoleKey.A or ConsoleKey.LeftArrow => Direction.Left,
                ConsoleKey.D or ConsoleKey.RightArrow => Direction.Right,
                ConsoleKey.S or ConsoleKey.DownArrow => Direction.Down,
                _ => null
            };

            return direction is not null;
        }

        public bool ReadConfirmation(char expectedKeyChar)
        {
            var keyInfo = Console.ReadKey(intercept: true);
            return keyInfo.KeyChar == expectedKeyChar;
        }
    }
}
