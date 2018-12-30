using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game2048.Core;

namespace Game2048.ConsoleApp
{
    internal class Program
    {
        private static readonly Random Random = new Random();
        private static int _score;
        private static int _maxNumber;

        private static void Main()
        {
            var game2048Matrix = GetGame2048Matrix();

            var backup = new int[game2048Matrix.Storage.Length];

            while (true)
            {
                PrintStatistics();
                PrintMatrix(game2048Matrix);

                game2048Matrix.Storage.CopyTo(backup, 0);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        game2048Matrix.MoveTo(MoveOrientation.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        game2048Matrix.MoveTo(MoveOrientation.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        game2048Matrix.MoveTo(MoveOrientation.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        game2048Matrix.MoveTo(MoveOrientation.Right);
                        break;
                    case ConsoleKey.Spacebar:
                        game2048Matrix = GetGame2048Matrix();
                        game2048Matrix.Storage.CopyTo(backup, 0);
                        break;
                }

                if (!Equals(backup, game2048Matrix.Storage))
                    game2048Matrix.Storage[GetExclusiveIndexes(game2048Matrix)] = GetTwoOrFour();

                Console.Clear();
            }
        }

        public static Game2048Matrix GetGame2048Matrix()
        {
            _score = 0;
            _maxNumber = 0;

            var game2048Matrix = new Game2048Matrix(6);

            game2048Matrix.Storage[GetExclusiveIndexes(game2048Matrix)] = GetTwoOrFour();
            game2048Matrix.Storage[GetExclusiveIndexes(game2048Matrix)] = GetTwoOrFour();

            game2048Matrix.Merged += (sender, args) =>
            {
                _score += args.MergedValue;
                if (args.MergedValue > _maxNumber) _maxNumber = args.MergedValue;
            };

            game2048Matrix.Merged += (sender, args) => Trace.WriteLine(
                $"[GAME2048 MERGED] " +
                $"{args.Orientation}: " +
                $"({args.FromCell.X}, {args.FromCell.Y}) --> " +
                $"({args.ToCell.X}, {args.ToCell.Y}) = " +
                $"{args.MergedValue}");

            game2048Matrix.Moved += (sender, args) => Trace.WriteLine(
                $"[GAME2048 MOVED] " +
                $"{args.Orientation}: " +
                $"({args.FromCell.X}, {args.FromCell.Y}) --> " +
                $"({args.ToCell.X}, {args.ToCell.Y})");

            return game2048Matrix;
        }

        private static int GetExclusiveIndexes(Game2048Matrix matrix)
        {
            int result;
            do
            {
                result = Random.Next(0, matrix.Storage.Length);
            } while (matrix.Storage[result] != 0);

            return result;
        }

        private static int GetTwoOrFour() => Random.NextDouble() > 0.5 ? 2 : 4;

        private static bool Equals(IReadOnlyCollection<int> left, IReadOnlyList<int> right)
        {
            if (left.Count != right.Count) return false;

            return !left.Where((item, i) => item != right[i]).Any();
        }

        private static void PrintStatistics()
        {
            Console.WriteLine($"┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.WriteLine($"┃ Score: {_score,-8} Max Number: {_maxNumber,-6} ┃");
            Console.WriteLine($"┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.WriteLine();
        }

        public static void PrintMatrix(Game2048Matrix game2048Matrix)
        {
            var order = game2048Matrix.MatrixOrder;

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    var value = game2048Matrix[j, i];
                    Console.Write($"{(value == 0 ? "." : value.ToString()),-5} ");
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
