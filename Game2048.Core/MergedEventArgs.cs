namespace Game2048.Core
{
    public class MergedEventArgs : MovedEventArgs
    {
        internal MergedEventArgs(MoveOrientation orientation, int index, (int FromIndex, int ToIndex) movedCellIndexes, int matrixOrder, int mergedValue)
            : base(orientation, index, movedCellIndexes, matrixOrder)
        {
            MergedValue = mergedValue;
        }

        public int MergedValue { get; }
    }
}
