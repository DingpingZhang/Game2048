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

        public int MatrixOrder { get; }

        public int[] Matrix { get; }

        public Game2048Matrix(int matrixOrder = 4)
        {
            MatrixOrder = matrixOrder;
            Matrix = new int[matrixOrder * matrixOrder];
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
                MoveAndMergeArray(getter, setter, (mergedCells, mergedValue) =>
                {
                    if (mergedValue.HasValue)
                        RaiseMerged(orientation, index, mergedCells, mergedValue.Value);
                    else
                        RaiseMoved(orientation, index, mergedCells);
                });
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

        protected virtual void RaiseMerged(MoveOrientation orientation, int index, (int FromIndex, int ToIndex) movedCells, int mergedValue)
        {
            Merged?.Invoke(this, new MergedEventArgs(orientation, index, movedCells, MatrixOrder, mergedValue));
        }

        protected virtual void RaiseMoved(MoveOrientation orientation, int index, (int FromIndex, int ToIndex) movedCells)
        {
            Moved?.Invoke(this, new MovedEventArgs(orientation, index, movedCells, MatrixOrder));
        }
    }
}
