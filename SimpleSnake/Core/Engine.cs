using System;
using System.Collections.Generic;
using System.Threading;
using SimpleSnake.Enums;
using SimpleSnake.GameObjects;
using SimpleSnake.Interfaces;

namespace SimpleSnake.Core
{
    internal class Engine : IEngine
    {
        private const char WallSymbol = '\u25A0';
        private const char SnakeSymbol = '\u25CF';
        private const int FoodElementsCount = 5;

        private static readonly Dictionary<Direction, Point> _directions = new()
        {
            [Direction.Up] = new Point(0, -1),
            [Direction.Down] = new Point(0, 1),
            [Direction.Left] = new Point(-1, 0),
            [Direction.Right] = new Point(1, 0),
        };

        private static readonly Func<Point, Food>[] _foodFactories =
        {
            p => new Food(p, 1, '*'),
            p => new Food(p, 2, '$'),
            p => new Food(p, 3, '#')
        };

        private readonly Playground _playground;
        private readonly IWriter _writer;
        private readonly IReader _reader;

        public Engine(Playground playground, IWriter writer, IReader reader)
        {
            this._playground = playground ?? throw new ArgumentNullException(nameof(playground));
            this._writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this._reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Run()
        {
            this.WritePlayground();
            var shouldPlay = true;
            while (shouldPlay)
                shouldPlay = this.Play();
            Environment.Exit(0);
        }

        private bool Play()
        {
            var snake = this.PrepareSnake();
            var foodMap = this.PrepareFood(snake);

            var score = 0;
            this.WriteScoreLabel(score);

            while (true)
            {
                this.TryChangeDirection(snake);
                if (!this.MoveSnake(snake))
                {
                    this.WriteRestartLabel();

                    var shouldRestart = this._reader.ReadConfirmation('y')
                                        || this._reader.ReadConfirmation('Y');

                    this.ClearRestartLabel();
                    if (shouldRestart)
                    {
                        this.ClearPlayground(snake, foodMap);
                    }
                    else
                    {
                        this.WriteEndGameMessage();
                    }

                    return shouldRestart;
                }
                if (foodMap.Remove(snake.Head, out var consumedFood))
                {
                    score += consumedFood.Score;
                    this.WriteScoreLabel(score);
                    this.AddNewFood(snake, foodMap);
                }
                else
                {
                    this._writer.Write(snake.Shorten(), ' ');
                }

                Thread.Sleep(100);
            }
        }

        private bool IsWithinActivePlayground(Point point)
            => point.X >= 1 && point.X <= this._playground.Width 
                            && point.Y >= 1 && point.Y <= this._playground.Height;

        private void TryChangeDirection(Snake snake)
        {
            if (!this._reader.TryReadDirection(out var direction) ||
                !_directions.TryGetValue(direction.Value, out var vector))
                return;

            if (vector != -snake.Direction) snake.Direction = vector;
        }

        private bool MoveSnake(Snake snake)
        {
            if (!snake.Grow() || !IsWithinActivePlayground(snake.Head))
                return false;
            this._writer.Write(snake.Head, SnakeSymbol);
            return true;
        }

        private Snake PrepareSnake()
        {
            var defaultDirection = _directions[Direction.Right];
            var snake = new Snake(new Point(1, 1), defaultDirection);
            this._writer.Write(snake.Head, SnakeSymbol);

            for (var i = 0; i < 5; i++)
            {
                snake.Grow();
                this._writer.Write(snake.Head, SnakeSymbol);
            }

            return snake;
        }

        private Dictionary<Point, Food> PrepareFood(Snake snake)
        {
            Dictionary<Point, Food> foodMap = new();
            for (var i = 0; i < FoodElementsCount; i++)
                this.AddNewFood(snake, foodMap);
            return foodMap;
        }

        private void AddNewFood(Snake snake, Dictionary<Point, Food> foodMap)
        {
            var food = GenerateRandomFood(snake, foodMap);
            foodMap[food.Position] = food;
            this._writer.Write(food.Position, food.Symbol);
        }

        private Food GenerateRandomFood(Snake snake, Dictionary<Point, Food> foodMap)
        {
            var factoryIndex = Random.Shared.Next(_foodFactories.Length);

            Point randomPoint;
            do
            {
                randomPoint = GenerateRandomPoint();
            } while (snake.HasBodyElementAt(randomPoint) || foodMap.ContainsKey(randomPoint));

            return _foodFactories[factoryIndex](randomPoint);
        }

        private Point GenerateRandomPoint()
        {
            var randomX = Random.Shared.Next(this._playground.Width) + 1;
            var randomY = Random.Shared.Next(this._playground.Height) + 1;
            return new Point(randomX, randomY);
        }

        private void WriteScoreLabel(int score)
        {
            this._writer.Write(new Point(this._playground.Width + 5, 1),
                $"Score: {score}{new string(' ', 10)}");
        }

        private void WriteEndGameMessage()
        {
            this._writer.Write(new Point(0, this._playground.Height + 3), "Thank you for playing!");
        }

        private void WriteRestartLabel()
        {
            this._writer.Write(new Point(this._playground.Width + 5, 4),
                $"Game over. If you want to restart, press Y.");
        }

        private void ClearRestartLabel()
        {
            this._writer.Write(new Point(this._playground.Width + 5, 4), new string(' ', 50));
        }

        private void WritePlayground()
        {
            // TOP WALL + BOTTOM WALL
            for (var i = 0; i <= this._playground.Width + 1; i++)
            {
                this._writer.Write(new Point(i, 0), WallSymbol);
                this._writer.Write(new Point(i, this._playground.Height + 1), WallSymbol);
            }

            // LEFT WALL + RIGHT WALL
            for (var i = 1; i <= this._playground.Height; i++)
            {
                this._writer.Write(new Point(0, i), WallSymbol);
                this._writer.Write(new Point(this._playground.Width + 1, i), WallSymbol);
            }
        }

        private void ClearPlayground(Snake snake, Dictionary<Point, Food> foodMap)
        {
            while (snake.BodyLength > 1)
                this._writer.Write(snake.Shorten(), ' ');

            if(this.IsWithinActivePlayground(snake.Head))
                this._writer.Write(snake.Head, ' ');

            foreach (var point in foodMap.Keys)
                this._writer.Write(point, ' ');
        }
    }
}
