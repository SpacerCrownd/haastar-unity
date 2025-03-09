using PathfindingConstants;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cluster
{
    private int clusterId;
    private int width;
    private int height;
    private int posX;
    private int posY;
    private List<PathNode> nodes = new List<PathNode>(); // Consider removing
    private HashSet<PathNode> entrances = new HashSet<PathNode>();

    private GridGraphAbstraction abstraction;

    public int Width
    {
        get => width;
    }

    public int Height
    {
        get => height;
    }

    public int PosX
    {
        get => posX;
    }

    public int PosY
    {
        get => posY;
    }

    public Cluster(int x, int y, int width, int height, int id, GridGraphAbstraction abstraction)
    {
        this.posX = x;
        this.posY = y;
        this.width = width;
        this.height = height;
        this.clusterId = id;
        this.abstraction = abstraction;
        //Debug.Log($"Cluster {clusterId} x :{posX} y :{posY} width :{width} height :{height}");
    }

    public void AddNodes(GridMap map)
    {
        for(int x = posX; x < posX + width; x++)
        {
            for (int y = posY; y < posY + height; y++)
            {
                PathNode node = map.GetGridObject(x, y);
                node.ParentCluster = clusterId;
                nodes.Add(node);
            }
        }
    }

    public void AddEntrance(PathNode node)
    {
        Graph abstractGraph = abstraction.GetGraph(1);

        // add nodes to abstract graph
        abstractGraph.AddNode(node);
        // if entrance transition point has already been added to cluster, don't rebuild intra edges (reuse existing node)
        if (entrances.Add(node))
        {
            //Debug.Log("Entrance " + node);
            CreateIntraEdges(node);
        }
    }

    private void CreateEntrance(PathNode insideNode, PathNode outsideNode, TerrainType capability, int clearance)
    {
        //Debug.Log($"Cluster {insideNode.ParentCluster} Entrance: ({insideNode}, {outsideNode}) Created");
        Graph abstractGraph = abstraction.GetGraph(1);

        // optimize clearance size for future intra-edge calculations
        if (clearance > PathfindingConstantValues.maxAgentSize)
            clearance = PathfindingConstantValues.maxAgentSize;

        // add entrance transition point's node to its corresponding parent cluster
        AddEntrance(insideNode);
        insideNode.EntranceDestination = outsideNode;
        
        Cluster neighbourCluster = abstraction.GetCluster(outsideNode.ParentCluster);
        neighbourCluster.AddEntrance(outsideNode);

        // add annotated inter-edge to abstract graph
        abstractGraph.AddAnnotatedEdge(insideNode, outsideNode, 10f, capability, clearance);
        abstractGraph.AddAnnotatedEdge(outsideNode, insideNode, 10f, capability, clearance);
    }

    private void CreateIntraEdges(PathNode node)
    {
        Graph abstractGraph = abstraction.GetGraph(1);
        AAStar aastar = new AAStar();
        aastar.SetLimitSearchToClusters(true);

        foreach (PathNode entrance in entrances)
        {
            if(entrance != node)
            {
                foreach (TerrainType capability in PathfindingConstantValues.capabilities)
                {
                    // bigger sizes first
                    foreach(int size in PathfindingConstantValues.agentSizes)
                    {
                        LinkedList<PathNode> path = aastar.GetPath(abstraction, 0, node, entrance, capability, size);
                        if (path != null)
                        {
                            // check strong dominance
                            if (abstractGraph.FindDominantEdge(node, entrance, capability, size, (int) path.Last.Value.GCost) != null)
                                continue;

                            Edge edgeFromNode = abstractGraph.AddAnnotatedEdge(node, entrance, path.Last.Value.GCost, capability, size);
                            Edge edgeToNode = abstractGraph.AddAnnotatedEdge(entrance, node, path.Last.Value.GCost, capability, size);

                            //Debug.Log($"Cluster {clusterId} ({capability}, {size}): Entrance {node} Intra-edge, dest: {entrance}, path length: {path.Last.Value.GCost}");
                        }
                    }
                }
            }
        }
    }

    public void BuildEntrances()
    {
        foreach (TerrainType capability in PathfindingConstantValues.capabilities)
        {
            BuildEastEntrances(capability);
            BuildSouthEntrances(capability);
        }
    }

    private void BuildEastEntrances(TerrainType capability)
    {
        // Build entrances on east border
        int xLimit = posX + width - 1;
        int yLimit = posY;

        GridMap map = abstraction.GetGridMap();

        // if on the edge then no neighbour cluster
        if (xLimit == map.Width - 1)
            return;

        int y = yLimit + height - 1;

        while (y >= yLimit)
        {
            int entranceStartY = y;
            int entranceEndY = FindEastEntranceEnd(xLimit, entranceStartY, capability);
            //Debug.Log($"Cluster {clusterId} East Entrances, Terrain: {capability}, xLimit:{xLimit}, yLimit:{yLimit}, EntranceStartY: {entranceStartY}, EntranceEndY: {entranceEndY}");
            PathNode insideNode = map.GetGridObject(xLimit, entranceStartY);
            PathNode outsideNode = map.GetGridObject(xLimit + 1, entranceStartY);

            int clearance = Mathf.Min(insideNode.GetClearance(capability), outsideNode.GetClearance(capability));

            if (clearance > 0)
                CreateEntrance(insideNode, outsideNode, capability, clearance);

            y = entranceEndY - 1;
        }
    }

    private int FindEastEntranceEnd(int xLimit, int entranceStartY, TerrainType capability)
    {
        GridMap map = abstraction.GetGridMap();
        int minClearance = int.MaxValue;
        int entranceEndY = entranceStartY;

        /* 
         * extend entrance until one of the conditions is met: 
         *   1.end of the border area is reached
         *   2.obstacle detected
         *   3.clearance value of nodes along the border area in either cluster begins to increase
         */
        for (int y = entranceStartY; y >= posY; y--)
        {
            PathNode node1 = map.GetGridObject(xLimit, y);
            PathNode node2 = map.GetGridObject(xLimit + 1, y);
            int clearance = Mathf.Min(node1.GetClearance(capability), node2.GetClearance(capability));

            // obstacle encountered
            if (clearance == 0)
                return y;

            if (clearance <= minClearance) // update minimum and entranceEndY
            {
                minClearance = clearance;
                entranceEndY = y;
            }
            else // clearance value started increasing
            {
                return entranceEndY;
            }
        }

        return entranceEndY;
    }

    private void BuildSouthEntrances(TerrainType capability)
    {
        // Build entrances on south border
        int xLimit = posX + width - 1;
        int yLimit = posY;
        GridMap map = abstraction.GetGridMap();

        // if on the edge then no neighbour cluster
        if (yLimit == 0)
            return;

        int x = posX;

        while (x <= xLimit)
        {
            int entranceStartX = x;
            int entranceEndX = FindSouthEntranceEnd(entranceStartX, yLimit, capability);

            //Debug.Log($"Cluster {clusterId} South Entrances, Terrain: {capability}, xLimit:{xLimit}, yLimit:{yLimit}, EntranceStartX: {entranceStartX}, EntranceEndX: {entranceEndX}");

            PathNode insideNode = map.GetGridObject(entranceStartX, yLimit);
            PathNode outsideNode = map.GetGridObject(entranceStartX, yLimit - 1);

            int clearance = Mathf.Min(outsideNode.GetClearance(capability), insideNode.GetClearance(capability));

            if (clearance > 0)
                CreateEntrance(insideNode, outsideNode, capability, clearance);

            x = entranceEndX + 1;
        }
    }

    private int FindSouthEntranceEnd(int entranceStartX, int yLimit, TerrainType capability)
    {
        GridMap map = abstraction.GetGridMap();
        int minClearance = int.MaxValue;
        int entranceEndX = entranceStartX;

        /* extend entrance until one of the conditions is met: 
         * 1.end of the border area is reached
         * 2.obstacle detected
         * 3.clearance value of nodes along the border area in either cluster begins to increase
         */
        for (int x = entranceStartX; x < posX + width; x++)
        {
            PathNode node1 = map.GetGridObject(x, yLimit);
            PathNode node2 = map.GetGridObject(x, yLimit - 1);
            int clearance = Mathf.Min(node1.GetClearance(capability), node2.GetClearance(capability));

            // obstacle encountered
            if (clearance == 0)
                return x;

            if (clearance <= minClearance) // update minimum and entranceEndY
            {
                minClearance = clearance;
                entranceEndX = x;
            }
            else // clearance value started increasing
            {
                return entranceEndX;
            }
        }

        return entranceEndX;
    }

    public bool ContainsEntrance(PathNode node)
    {
        return entrances.Contains(node);
    }

    public void RemoveEntrance(PathNode node)
    {
        if (entrances.Remove(node))
        {
            //Debug.Log(node + " removed");
            Graph graph = abstraction.GetGraph(1);
            graph.Remove(node);
        }

    }

    public void ClearEntrances()
    {
        entrances.Clear();
    }

    public PathNode[] GetEntrances()
    {
        return entrances.ToArray();
    }
}
