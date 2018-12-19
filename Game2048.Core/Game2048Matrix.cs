using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Core
{
    public enum MoveOrientation { Up, Down, Left, Right }

    public class Game2048Matrix
    {
        private readonly IDictionary<MoveOrientation, (Func<int, int> Getter, Action<int, int> Setter)[]> _operatorsDictionary;

        public event Action<int> OnMerged;

        public int MatrixOrder { get; }

        public int[] Matrix { get; }

        public Game2048Matrix(int matrixOrder = 4)
        {
            MatrixOrder = matrixOrder;
            Matrix = new int[matrixOrder * matrixOrder];
            _operatorsDictionary = new Dictionary<MoveOrientation, (Func<int, int> Getter, Action<int, int> Setter)[]>
            {
                { MoveOrientation.Left, GetOperators(GetLinearFunction(MatrixOrder, 1)).ToArray() },
                { MoveOrientation.Right, GetOperators(GetLinearFunction(MatrixOrder, -1, MatrixOrder - 1)).ToArray() },
                { MoveOrientation.Up, GetOperators(GetLinearFunction(1, MatrixOrder)).ToArray() },
                { MoveOrientation.Down, GetOperators(GetLinearFunction(1, -MatrixOrder,MatrixOrder * (MatrixOrder - 1))).ToArray() },
            };
        }

        public void MoveTo(MoveOrientation orientation)
        {
            foreach (var (getter, setter) in _operatorsDictionary[orientation])
            {
                MoveAndMergeArray(getter, setter);
            }
        }

        private IEnumerable<(Func<int, int> Getter, Action<int, int> Setter)> GetOperators(Func<int, int, int> indexGetter)
        {
            return Enumerable.Range(0, MatrixOrder)
                .Select(i => new Func<int, int>(index => Matrix[indexGetter(i, index)]))
                .Zip(Enumerable.Range(0, MatrixOrder)
                        .Select(i => new Action<int, int>((index, value) => Matrix[indexGetter(i, index)] = value)),
                    (getter, setter) => (getter, setter));
        }

        private void MoveAndMergeArray(Func<int, int> getter, Action<int, int> setter)
        {
            for (int p = 0, i = 1; i < MatrixOrder; i++)
            {
                int next = getter(i);
                if (next == 0) continue;

                setter(i, 0);
                int current = getter(p);
                if (current == next)
                {
                    setter(p++, next * 2);
                    OnMerged?.Invoke(next * 2);
                }
                else
                {
                    setter(current == 0 ? p : ++p, next);
                }
            }
        }

        private static Func<int, int, int> GetLinearFunction(int a, int b, int c = 0) => (x, y) => a * x + b * y + c;
    }
}
