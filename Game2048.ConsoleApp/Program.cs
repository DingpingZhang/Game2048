﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.Core;

namespace Game2048.ConsoleApp
{
    internal class Program
    {
        private static readonly Random Random = new Random();

        private static void Main()
        {
            var game2048Matrix = GetGame2048Matrix();

            var backup = new int[game2048Matrix.Matrix.Length];

            while (true)
            {
                PrintMatrix(game2048Matrix);

                game2048Matrix.Matrix.CopyTo(backup, 0);

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
                        break;
                }

                if (!Equals(backup, game2048Matrix.Matrix))
                    game2048Matrix.Matrix[GetExclusiveIndexes(game2048Matrix)] = GetTwoOrFour();

                Console.Clear();
            }
        }

        public static Game2048Matrix GetGame2048Matrix()
        {
            var game2048Matrix = new Game2048Matrix(5);

            game2048Matrix.Matrix[GetExclusiveIndexes(game2048Matrix)] = GetTwoOrFour();
            game2048Matrix.Matrix[GetExclusiveIndexes(game2048Matrix)] = GetTwoOrFour();

            return game2048Matrix;
        }

        private static int GetExclusiveIndexes(Game2048Matrix matrix)
        {
            int result;
            do
            {
                result = Random.Next(0, matrix.Matrix.Length);
            } while (matrix.Matrix[result] != 0);

            return result;
        }

        private static int GetTwoOrFour() => Random.NextDouble() > 0.5 ? 2 : 4;

        private static bool Equals(IReadOnlyCollection<int> left, IReadOnlyList<int> right)
        {
            if (left.Count != right.Count) return false;

            return !left.Where((item, i) => item != right[i]).Any();
        }

        public static void PrintMatrix(Game2048Matrix game2048Matrix)
        {
            var matrix = game2048Matrix.Matrix;
            var order = game2048Matrix.MatrixOrder;

            if (matrix.Length != order * order) throw new InvalidOperationException();

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    Console.Write($"\t{matrix[i * order + j]} ");
                }
                Console.WriteLine();
            }
        }
    }
}