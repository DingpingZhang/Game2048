using System;

namespace Game2048.Core
{
    public class MovedEventArgs : EventArgs
    {
        public MoveOrientation Orientation { get; }

        public (int X, int Y) FromCell { get; }

        public (int X, int Y) ToCell { get; }

        internal MovedEventArgs(MoveOrientation orientation, int index, (int FromIndex, int ToIndex) movedCellIndexes, int matrixOrder)
        {
            Orientation = orientation;
            FromCell = CalculateCoordinate(orientation, index, movedCellIndexes.FromIndex, matrixOrder);
            ToCell = CalculateCoordinate(orientation, index, movedCellIndexes.ToIndex, matrixOrder);
        }

        private (int X, int Y) CalculateCoordinate(MoveOrientation orientation, int rowIndex, int cellIndex, int matrixOrder)
        {
            switch (orientation)
            {
                case MoveOrientation.Up:
                    return (rowIndex, cellIndex);
                case MoveOrientation.Down:
                    return (rowIndex, matrixOrder - 1 - cellIndex);
                case MoveOrientation.Left:
                    return (cellIndex, rowIndex);
                case MoveOrientation.Right:
                    return (matrixOrder - 1 - cellIndex, rowIndex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
}
