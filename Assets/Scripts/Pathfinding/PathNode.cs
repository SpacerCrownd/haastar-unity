using PathfindingConstants;

public class PathNode
{
    private GridMap grid;
    private int x;
    private int y;

    private int parentCluster;
    private int[] clearances;
    private TerrainType terrain;

    private float gCost;
    private float hCost;
    private float fCost;

    private PathNode parent;

    private PathNode entranceDestination;

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

    public TerrainType Terrain
    {
        get => terrain;
        set
        {
            terrain = value;
            grid.TriggerGridObjectChanged(x, y);
        }
    }

    public float GCost
    {
        get => gCost;
        set => gCost = value;
    }

    public float HCost
    {
        get => hCost;
        set => hCost = value;
    }

    public float FCost
    {
        get => fCost;
    }

    public PathNode Parent
    {
        get => parent;
        set => parent = value;
    }

    public int ParentCluster
    {
        get => parentCluster;
        set => parentCluster = value;
    }

    public PathNode EntranceDestination
    {
        get => entranceDestination;
        set => entranceDestination = value;
    }

    public PathNode(GridMap grid, int x, int y, TerrainType terrain = TerrainType.Ground)
    {
        this.terrain = terrain;
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.clearances = new int[PathfindingConstantValues.capabilities.Length];
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return "(" + x.ToString() + "," + y.ToString() + ")";
    }

    public int GetClearance(TerrainType terrainType)
    {
        switch (terrainType)
        {
            case TerrainType.Ground:
                return clearances[0];
            case TerrainType.Water:
                return clearances[1];
            case TerrainType.Ground | TerrainType.Water:
                return clearances[2];
            default:
                return -1;
        }
    }

    public void SetClearance(TerrainType terrainType, int clearance)
    {
        switch (terrainType)
        {
            case TerrainType.Ground:
                clearances[0] = clearance;
                break;
            case TerrainType.Water:
                clearances[1] = clearance;
                break;
            case TerrainType.Ground | TerrainType.Water:
                clearances[2] = clearance;
                break;
        }
    }
}
