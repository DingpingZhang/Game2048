using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Core
{
    public class Game2048Matrix
    {
        private readonly IDictionary<MoveOrientation, (Func<int, int> Getter, Action<int, int> Setter)[]> _operatorsDictionary;

        public event EventHandler<MergedEventArgs> Merged;
        public event EventHandler<MovedEventArgs> Moved;

        public int[] Storage { get; }

        public int MatrixOrder { get; }

        public int this[int x, int y]
        {
            get => Storage[x + y * MatrixOrder];
            set => Storage[x + y * MatrixOrder] = value;
        }

        public Game2048Matrix(int matrixOrder = 4)
        {
            MatrixOrder = matrixOrder;
            Storage = new int[matrixOrder * matrixOrder];
            _operatorsDictionary = new Dictionary<MoveOrientation, (Func<int, int> Getter, Action<int, int> Setter)[]>
            {
                { MoveOrientation.Left, GetOperators(GetLinearFunction(matrixOrder, 1)).ToArray() },
                { MoveOrientation.Right, GetOperators(GetLinearFunction(matrixOrder, -1, matrixOrder - 1)).ToArray() },
                { MoveOrientation.Up, GetOperators(GetLinearFunction(1, matrixOrder)).ToArray() },
                { MoveOrientation.Down, GetOperators(GetLinearFunction(1, -matrixOrder, matrixOrder * (matrixOrder - 1))).ToArray() },
            };
        }

        public void MoveTo(MoveOrientation orientation)
        {
            int i = 0;
            foreach (var (getter, setter) in _operatorsDictionary[orientation])
            {
                var index = i++;
                MoveAndMergeArray(getter, setter, (movedCells, mergedValue) =>
                {
                    if (mergedValue.HasValue)
                        Merged?.Invoke(this, new MergedEventArgs(orientation, index, movedCells, MatrixOrder, mergedValue.Value));
                    else
                        Moved?.Invoke(this, new MovedEventArgs(orientation, index, movedCells, MatrixOrder));
                });
            }
        }

        private IEnumerable<(Func<int, int> Getter, Action<int, int> Setter)> GetOperators(Func<int, int, int> indexGetter)
        {
            return Enumerable.Range(0, MatrixOrder)
                .Select(i => new Func<int, int>(index => Storage[indexGetter(i, index)]))
                .Zip(Enumerable.Range(0, MatrixOrder)
                        .Select(i => new Action<int, int>((index, value) => Storage[indexGetter(i, index)] = value)),
                    (getter, setter) => (getter, setter));
        }

        private void MoveAndMergeArray(Func<int, int> getter, Action<int, int> setter, Action<(int from, int to), int?> reporter)
        {
            for (int p = 0, i = 1; i < MatrixOrder; i++)
            {
                int next = getter(i);
                if (next == 0) continue;

                int current = getter(p);
                if (current == next)
                {
                    setter(p++, next * 2);
                    setter(i, 0);
                    reporter?.Invoke((i, p - 1), next * 2);
                }
                else
                {
                    int toIndex = current == 0 ? p : ++p;
                    if (toIndex != i)
                    {
                        setter(toIndex, next);
                        setter(i, 0);
                        reporter?.Invoke((i, toIndex), null);
                    }
                }
            }
        }

        private static Func<int, int, int> GetLinearFunction(int a, int b, int c = 0) => (x, y) => a * x + b * y + c;
    }
}
