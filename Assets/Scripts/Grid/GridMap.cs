using System;
using UnityEngine;

public class GridMap
{
    private int width;
    private int height;
    private float cellSize;
    private bool debug;
    private PathNode[,] grid;

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public PathNode[,] Grid
    {
        get => grid;
    }

    public float CellSize
    {
        get => cellSize;
    }

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;

    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public GridMap(int width, int height, float cellSize, bool debug)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.debug = debug;

        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new PathNode[width, height];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new PathNode(this, x, y);
            }
        }

        if (debug)
        {
            TextMesh[,] debugText = new TextMesh[width, height];
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    debugText[x, y] = DebugUtil.CreateDebugText(grid[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f + new Vector3(0, -1, 0), 20, Color.black, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugText[eventArgs.x, eventArgs.y].text = grid[eventArgs.x, eventArgs.y].ToString();
            };
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    public bool GetGridPosition(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public PathNode GetGridObject(int x, int y)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            return grid[x, y];
        }
        else
        {
            return null;
        }
    }

    public PathNode GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        if(GetGridPosition(worldPosition, out x, out y))
        {
            return grid[x, y];
        }
        else
        {
            return null;
        }
    }

    public void SetGridObject(Vector3 worldPosition, PathNode obj)
    {
        int x, y;
        GetGridPosition(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            grid[x, y] = obj;
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y});
    }
}
