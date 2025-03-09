using System;
using UnityEngine;
using PathfindingConstants;
using System.Collections.Generic;

public class GridGraphAbstraction
{
    private GridMap map;
    private Graph[] graphs = new Graph[2];

    private PathNode start;
    private PathNode goal;

    private List<Cluster> clusters = new List<Cluster>();

    private static readonly Vector3[] DIRECTIONS =
    {
        // left
        new Vector3(-1, 0, 10),
        // top-left
        new Vector3(-1, 1, 14), 
        // top
        new Vector3(0, 1, 10),
        // top-right
        new Vector3(1, 1, 14),
        // right
        new Vector3(1, 0, 10),
        // bottom-right
        new Vector3(1, -1, 14),
        // bottom
        new Vector3(0, -1, 10),
        // bottom-left
        new Vector3(-1, -1, 14)
    };

    private ClusterVisuals clusterVisuals;

    public GridGraphAbstraction(GridMap map) 
    {
        this.map = map;
        InitializeGraph();
    }

    public GridMap GetGridMap() 
    { 
        return map; 
    }

    public Graph GetGraph(int level)
    {
        return graphs[level];
    }

    public void InitializeGraph()
    {
        graphs[0] = new Graph();

        foreach (PathNode node in map.Grid)
        {
            graphs[0].AddNode(node);
        }

        foreach (PathNode node in map.Grid)
        {
            foreach (Vector3 direction in DIRECTIONS)
            {
                PathNode to = map.GetGridObject(node.X + (int)direction.x, node.Y + (int)direction.y);

                if (to != null)
                    graphs[0].AddEdge(node, to, direction.z);
            }
        }

        AnnotateMap();
    }

    public void InitializeCosts()
    {
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                PathNode node = map.GetGridObject(x, y);
                node.GCost = float.MaxValue;
                node.CalculateFCost();
                node.Parent = null;
            }
        }
    }

    public float CalculateHeuristic(PathNode a, PathNode b)
    {
        Vector2Int dist = new Vector2Int(Mathf.Abs(a.X - b.X), Mathf.Abs(a.Y - b.Y));

        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);

        return 10 * (dx + dy) + (14 - 10 * 2) * Math.Min(dx, dy);
    }

    public void AnnotateMap()
    {
        // start from bottom right corner
        for (int x = map.Width - 1; x >= 0 ; x--)
        {
            for(int y = 0; y < map.Height; y++)
            {
                PathNode node = map.GetGridObject(x, y);

                TerrainType nodeTerrain = node.Terrain;

                if (nodeTerrain != TerrainType.None)
                {
                    // neighbours
                    PathNode right, rightDown, down; 
                    right = map.GetGridObject(node.X + 1, node.Y); ;
                    rightDown = map.GetGridObject(node.X + 1, node.Y - 1);
                    down = map.GetGridObject(node.X, node.Y - 1);

                    // if not on border
                    if (right != null && rightDown != null && down != null)
                    {
                        foreach (TerrainType capability in PathfindingConstantValues.capabilities) 
                        {
                            // calculate clearance only for capabilities that include the node's terrain type
                            if ((capability & nodeTerrain) == nodeTerrain)
                            {
                                // get minimum clearance of neighbours
                                int min = Mathf.Min(right.GetClearance(capability), down.GetClearance(capability), rightDown.GetClearance(capability));
                                node.SetClearance(capability, min + 1);
                            }
                            else
                                node.SetClearance(capability, 0);
                        }
                    }
                    else
                    {
                        foreach (TerrainType capability in PathfindingConstantValues.capabilities)
                        {
                            if ((capability & nodeTerrain) == nodeTerrain)
                                node.SetClearance(capability, 1);
                            else
                                node.SetClearance(capability, 0);
                        }
                    }
                }
            }
        }
    }

    public void BuildClusters(int clusterSize)
    {
        clusters.Clear();
        clusterVisuals?.ClearEntrances();
        clusterVisuals?.ClearClusters();

        if(clusterSize > 0 && clusterSize <= Mathf.Max(map.Width, map.Height))
        {
            // Split map into clusters of size clusterSize if possible
            for (int x = 0; x < map.Width; x += clusterSize)
            {
                for (int y = 0; y < map.Height; y += clusterSize)
                {
                    int clusterWidth = clusterSize;
                    int clusterHeight = clusterSize;

                    if (clusterWidth + x > map.Width) // if cluster width is bigger than map bounds then get remaining width
                        clusterWidth = map.Width - x;

                    if (clusterHeight + y > map.Height) // if cluster height is bigger than map bounds then get remaining height
                        clusterHeight = map.Height - y;

                    Cluster cluster = new Cluster(x, y, clusterWidth, clusterHeight, clusters.Count, this);
                    cluster.AddNodes(map);
                    clusters.Add(cluster);

                    clusterVisuals?.ShowCluster(cluster);
                }
            }
        }
    }

    public void BuildEntrances()
    {
        graphs[1] = new Graph();

        foreach (Cluster cluster in clusters)
        {
            cluster.ClearEntrances();
        }
        
        foreach (Cluster cluster in clusters)
        {
            cluster.BuildEntrances();
        }

        clusterVisuals?.ClearEntrances();
        foreach (Cluster cluster in clusters)
        {
            foreach (PathNode node in cluster.GetEntrances())
            {
                clusterVisuals?.ShowEntrance(node);
            }
        }

        /*
        for(int i = 0;i < clusters.Count; i++)
        {
            Debug.Log($"Start Entrances Cluster {i}");
            foreach (PathNode node in clusters[i].GetEntrances())
            {
                Debug.Log(node);
            }
            Debug.Log($"End Entrances Cluster {i}");
        }
        */
    }

    public Cluster GetCluster(int clusterId)
    {
        return clusters[clusterId];
    }

    public bool InsertStartAndGoal(PathNode start, PathNode goal)
    {
        if(start.Terrain == TerrainType.Rock || goal.Terrain == TerrainType.Rock)
        {
            return false;
        }

        if (!clusters[start.ParentCluster].ContainsEntrance(start))
        {
            //Debug.Log("Inserting Start");
            clusters[start.ParentCluster].AddEntrance(start);
            this.start = start;
        }

        if (!clusters[goal.ParentCluster].ContainsEntrance(goal))
        {
            //Debug.Log("Inserting Goal");
            clusters[goal.ParentCluster].AddEntrance(goal);
            this.goal = goal;
        }

        return true;
    }

    public void RemoveStartAndGoal()
    {
        if(start != null)
            clusters[start.ParentCluster].RemoveEntrance(start);

        if(goal != null)
            clusters[goal.ParentCluster].RemoveEntrance(goal);
    }
    
    public void SetClusterVisuals(ClusterVisuals visuals)
    {
        clusterVisuals = visuals;
    }
}
