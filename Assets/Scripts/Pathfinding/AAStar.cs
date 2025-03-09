using PathfindingConstants;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class AAStar
{
    protected GridMapVisuals gridMapVisuals;
    private bool limitSearchToClusters = false;
    private int startClusterId;
    private int goalClusterId;
    protected Edge currentEdge;

    public void SetGridMapVisuals(GridMapVisuals gridMapVisuals)
    {
        this.gridMapVisuals = gridMapVisuals;
    }

    public GridMapVisuals GetGridMapVisuals()
    {
        return gridMapVisuals;
    }

    public void SetLimitSearchToClusters(bool limitSearchToClusters)
    {
        this.limitSearchToClusters = limitSearchToClusters;
    }

    public virtual LinkedList<PathNode> GetPath(GridGraphAbstraction graphAbstraction, int level, PathNode start, PathNode goal, TerrainType capability, int size, out int expandedNodes)
    {
        expandedNodes = 0;

        if (start == null || goal == null || graphAbstraction == null)
        {
            return null;
        }

        //Debug.Log($"Start {start}, Goal {goal}");

        //gridMapVisuals?.ResetColors();

        if (limitSearchToClusters)
        {
            startClusterId = start.ParentCluster;
            goalClusterId = goal.ParentCluster;
        }

        if (start.GetClearance(capability) < size || goal.GetClearance(capability) < size) // if one side is unreachable
            return null;

        HashSet<PathNode> openList = new HashSet<PathNode>();
        HashSet<PathNode> closedList = new HashSet<PathNode>();
        PriorityQueue<PathNode, float> openListQueue = new PriorityQueue<PathNode, float>();

        Graph graph = graphAbstraction.GetGraph(level);
        GridMap map = graphAbstraction.GetGridMap();

        graphAbstraction.InitializeCosts();

        start.GCost = 0;
        start.HCost = graphAbstraction.CalculateHeuristic(start, goal);
        start.CalculateFCost();

        openListQueue.Enqueue(start, start.FCost);
        openList.Add(start);

        gridMapVisuals?.SetCostText(start);
        gridMapVisuals?.SetTileVisited(start.X, start.Y);
        expandedNodes++;

        while (openListQueue.Count > 0)
        {
            PathNode currentNode = openListQueue.Dequeue();
            //Debug.Log($"Current node : {currentNode.X} {currentNode.Y}");
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Debug.Log($"Openlist current node {currentNode}");

            if (currentNode.Equals(goal))
            {
                //Debug.Log("Goal Found");
                return CalculatePath(goal);
            }

            foreach (Edge edge in graph.GetEdges(currentNode))
            {
                currentEdge = edge;
                //Debug.Log(edge);
                PathNode neighbour = edge.ToNode;

                //Debug.Log("Cycling neighbours, current : " + neighbour);

                if (closedList.Contains(neighbour))
                    continue;

                //Debug.Log(isTraversable(map, currentNode, neighbour, capability, size) + " " + currentNode + " " + neighbour);

                if (isTraversable(map, currentNode, neighbour, capability, size))
                {
                    float newGCost = currentNode.GCost + edge.Label;
                    //Debug.Log($"neighbour {neighbour.X} {neighbour.Y} cost {edge.Label}");
                    if ((newGCost < neighbour.GCost || !openList.Contains(neighbour)))
                    {
                        neighbour.Parent = currentNode;
                        neighbour.GCost = newGCost;
                        neighbour.HCost = graphAbstraction.CalculateHeuristic(neighbour, goal);
                        neighbour.CalculateFCost();
                        //Debug.Log($"neighbour {neighbour.X} {neighbour.Y} newGCost {newGCost} HCost {neighbour.HCost} Fcost {neighbour.FCost}");

                        gridMapVisuals?.SetCostText(neighbour);

                        if (!openList.Contains(neighbour))
                        {
                            gridMapVisuals?.SetTileVisited(neighbour.X, neighbour.Y);

                            openListQueue.Enqueue(neighbour, neighbour.FCost);
                            openList.Add(neighbour);
                        }
                    }
                }
            }
            gridMapVisuals?.SetTileExpanded(currentNode.X, currentNode.Y);
            expandedNodes++;
        }

        return null;
    }

    public virtual LinkedList<PathNode> GetPath(GridGraphAbstraction graphAbstraction, Vector3 start, Vector3 goal, TerrainType capability, int size, out int expandedNodes)
    {
        GridMap map = graphAbstraction.GetGridMap();

        return GetPath(graphAbstraction, 0, map.GetGridObject(start), map.GetGridObject(goal), capability, size, out expandedNodes);
    }

    public LinkedList<PathNode> GetPath(GridGraphAbstraction graphAbstraction, int level, PathNode start, PathNode goal, TerrainType capability, int size)
    {
        GridMap map = graphAbstraction.GetGridMap();
        return GetPath(graphAbstraction, 0, start, goal, capability, size, out int expandedNodes);
    }

    public LinkedList<PathNode> GetPath(GridGraphAbstraction graphAbstraction, Vector3 start, Vector3 goal, TerrainType capability, int size)
    {
        GridMap map = graphAbstraction.GetGridMap();

        return GetPath(graphAbstraction, 0, map.GetGridObject(start), map.GetGridObject(goal), capability, size, out int expandedNodes);
    }

    protected virtual bool isTraversable(GridMap map, PathNode start, PathNode end, TerrainType capability, int size)
    {
        if (start == null || end == null)
        {
            return false;
        }

        if (end.GetClearance(capability) < size)
        {
            return false;
        }

        if (limitSearchToClusters && !isInCluster(end))
        {
            //Debug.Log("not in cluster");
            return false;
        }

        Direction direction = 0;

        /*     1 0 -1
         *     _ _ _
         * -1 |o o o
         *  0 |o x o
         *  1 |o o o
         */
        int diffX = start.X - end.X;
        int diffY = start.Y - end.Y;

        switch (diffX)
        {
            // add eastern direction
            case 1:
                direction = Direction.West;
                break;
            // add western direction
            case -1:
                direction = Direction.East;
                break;
            case 0:
                break;
        };

        switch (diffY)
        {
            // add northen direction
            case 1:
                direction = direction | Direction.South;
                break;
            // add southern direction
            case -1:
                direction = direction | Direction.North;
                break;
            case 0:
                break;
        }

        // if not diagonal, no need to check other directions
        if (direction == Direction.North || direction == Direction.East || direction == Direction.South || direction == Direction.West)
            return true;

        switch (direction)
        {
            // if diagonal check if the other 2 cardinal directions are traversable
            case Direction.NorthEast:
                {
                    return isTraversable(map, start, map.GetGridObject(start.X, start.Y + 1), capability, size) && isTraversable(map, start, map.GetGridObject(start.X + 1, start.Y), capability, size);
                }
            case Direction.SouthEast:
                {
                    return isTraversable(map, start, map.GetGridObject(start.X, start.Y - 1), capability, size) && isTraversable(map, start, map.GetGridObject(start.X + 1, start.Y), capability, size);
                }
            case Direction.SouthWest:
                {
                    return isTraversable(map, start, map.GetGridObject(start.X, start.Y - 1), capability, size) && isTraversable(map, start, map.GetGridObject(start.X - 1, start.Y), capability, size);
                }
            case Direction.NorthWest:
                {
                    return isTraversable(map, start, map.GetGridObject(start.X, start.Y + 1), capability, size) && isTraversable(map, start, map.GetGridObject(start.X - 1, start.Y), capability, size);
                }
        }

        return false;
    }

    private bool isInCluster(PathNode node)
    {
        return node.ParentCluster == startClusterId || node.ParentCluster == goalClusterId;
    }

    private LinkedList<PathNode> CalculatePath(PathNode goal)
    {
        LinkedList<PathNode> path = new LinkedList<PathNode>();

        PathNode currentNode = goal;
        path.AddFirst(currentNode);

        if (gridMapVisuals != null)
            gridMapVisuals.SetTilePath(currentNode.X, currentNode.Y);
        while (currentNode.Parent is not null)
        {
            path.AddFirst(currentNode.Parent);
            currentNode = currentNode.Parent;

            if (gridMapVisuals != null)
                gridMapVisuals.SetTilePath(currentNode.X, currentNode.Y);
        }

        return path;
    }
}
