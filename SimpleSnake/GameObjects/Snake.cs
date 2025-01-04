using System;
using System.Collections.Generic;

namespace SimpleSnake.GameObjects
{
    public class Snake
    {
        private Point _direction;

        private readonly Queue<Point> _bodyQueue = new();
        private readonly HashSet<Point> _bodySet = new();

        public Snake(Point initialHead, Point direction)
        {
            this.SetHead(initialHead);
            this._direction = direction;
        }

        public Point Head { get; private set; }
        public Point Direction
        {
            get => this._direction;
            set
            {
                if (Math.Abs(value.X) + Math.Abs(value.Y) != 1)
                    throw new ArgumentException("Invalid direction");
                this._direction = value;
            }

        }
        public int BodyLength => this._bodyQueue.Count;

        public bool Grow()
            => this.SetHead(this.Head + this.Direction);

        public Point Shorten()
        {
            if (this._bodyQueue.Count == 1)
                throw new InvalidOperationException("Cannot shorten this snake anymore");
            
            var tail = this._bodyQueue.Dequeue();
            this._bodySet.Remove(tail);
            return tail;
        }

        public bool HasBodyElementAt(Point point) => this._bodySet.Contains(point);

        private bool SetHead(Point headPosition)
        {
            if (!this._bodySet.Add(headPosition)) return false;

            this.Head = headPosition;
            this._bodyQueue.Enqueue(headPosition);
            return true;
        }
    }
}
