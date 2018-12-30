# Game2048

## 1. `Game2048Matrix` API

```csharp
var matrix = new Game2048Matrix(matrixOrder: 6); // Creates an instance of the Game2048Matrix with the specified matrix order.

matrix.MoveTo(MoveOrientation.Left); // Slides all cells to the left.

matrix.Moved += (sender, e) => { }; // Subscribes the Moved event.
matrix.Merged += (sender, e) => { }; // Subscribes the Merged event.

var cellValue = matrix[1, 3]; // Gets the value on (1, 3).

```

## 2. Build and Play

- Open the `.sln` file with VS2017 (or later). Rebuild the `Game2048.ConsoleApp` project and run it;
- You will see a console program launched, just like the screenshot below;
- You can control these cells by using the arrow keys (`←`, `↑`, `→` and `↓`), or start over by pressing the `space bar`;

![console-ui](./Screenshots/cui.png)

- You can open the `output` dialog on Visual Studio, and view the action event record (`Moved` or `Merged`), just like below.

![operation-events-log](./Screenshots/events-log.png)

## 3. Core Code

```csharp
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
```
