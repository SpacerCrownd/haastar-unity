using PathfindingConstants;
using System.Collections.Generic;
using UnityEngine;

public class LevelPathfinding : MonoBehaviour
{
    public static LevelPathfinding Instance { get; private set; }

    [SerializeField]
    private GridSizeSO gridSizeSO;

    [SerializeField]
    private IntValueSO clusterSizeSO;

    [SerializeField]
    private IntValueSO expandedNodesSO;

    [SerializeField]
    private GridMapVisuals gridMapVisuals;

    [SerializeField]
    private ClusterVisuals clusterVisuals;
    
    [SerializeField]
    private bool debug = false;

    private GridMap map;
    private GridGraphAbstraction gridGraphAbstraction;
    public GridMap Grid { get { return map; } }

    private float gridCellSize = 10f;

    private AAStar searchAlgorithm;

    private void Awake()
    {
        Instance = this;
        map = new GridMap(gridSizeSO.GetWidth(), gridSizeSO.GetHeight(), gridCellSize, debug);
        gridGraphAbstraction = new GridGraphAbstraction(map);

        gridMapVisuals.SetMap(map);
        clusterVisuals.SetMap(map);

        //searchAlgorithm = new AAStar();
        searchAlgorithm = new HAAStar();
        searchAlgorithm.SetGridMapVisuals(gridMapVisuals);
    }

    private void Start()
    {
        gridGraphAbstraction.SetClusterVisuals(clusterVisuals);

        gridGraphAbstraction.BuildClusters(clusterSizeSO.GetValue());
        
        InitializeMap();
    }

    private void OnEnable()
    {
        gridSizeSO.sizeChangeEvent += OnGridSizeChangeEvent;
        clusterSizeSO.valueChangeEvent += OnClusterSizeChange;
    }

    private void OnDisable()
    {
        gridSizeSO.sizeChangeEvent -= OnGridSizeChangeEvent;
        clusterSizeSO.valueChangeEvent -= OnClusterSizeChange;
    }

    private void OnClusterSizeChange(object sender, int e)
    {
        gridGraphAbstraction.BuildClusters(e);
        InitializeMap();
    }

    private void OnGridSizeChangeEvent(object sender, GridSizeSO.SizeChangeEventArgs e)
    {
        map.Width = e.width;
        map.Height = e.height;
        map.CreateGrid();
        
        clusterVisuals.ClearClusters();
        clusterVisuals.ClearEntrances();

        gridGraphAbstraction.InitializeGraph();
        gridMapVisuals.CreateGridMapTiles();
    }

    public void InitializeMap()
    {
        gridGraphAbstraction.AnnotateMap();
        gridGraphAbstraction.BuildEntrances();
        
        if(debug)
            gridMapVisuals.SetCleranceText();
    }

    public LinkedList<Vector3> GetPath(Vector3 start, Vector3 goal, TerrainType capability, int size)
    {
        gridMapVisuals?.ResetTiles();
        LinkedList<PathNode> path = searchAlgorithm.GetPath(gridGraphAbstraction, start, goal, capability, size, out int expandedNodes);
        LinkedList<Vector3> vectorPath = null;

        expandedNodesSO.ChangeValue(expandedNodes);

        if (path != null)
        {
            vectorPath = new LinkedList<Vector3>();

            foreach (PathNode node in path)
            {
                Vector3 vector = map.GetWorldPosition(node.X, node.Y) + Vector3.one * map.CellSize/2;
                vector.z = start.z;
                vectorPath.AddLast(vector);
            }
        }

        return vectorPath;
    }

    public void UseHAAStar()
    {
        searchAlgorithm = new HAAStar();
        searchAlgorithm.SetGridMapVisuals(gridMapVisuals);
    }

    public void UseAAStar()
    {
        searchAlgorithm = new AAStar();
        searchAlgorithm.SetGridMapVisuals(gridMapVisuals);
    }
}

