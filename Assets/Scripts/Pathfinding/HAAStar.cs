using PathfindingConstants;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HAAStar : AAStar
{
    private GridMapVisuals gridMapVisualsAAStar;

    public LinkedList<PathNode> GetPath(GridGraphAbstraction graphAbstraction, PathNode start, PathNode goal, TerrainType capability, int size, out int expandedNodes)
    {
        LinkedList<PathNode> path = null;
        expandedNodes = 0;

        AAStar aaStar = new AAStar();
        aaStar.SetLimitSearchToClusters(true);

        if (base.gridMapVisuals is not null)
        {
            gridMapVisualsAAStar = base.gridMapVisuals;
            base.gridMapVisuals = null;
        }

        aaStar.SetGridMapVisuals(gridMapVisualsAAStar);

        if (start.ParentCluster == goal.ParentCluster)
        {
            path = aaStar.GetPath(graphAbstraction, 0, start, goal, capability, size, out expandedNodes);
        }

        if (path is null)
        {
            // there are no built clusters or start and goal were in the same cluster but goal was unreachable
            if(start.ParentCluster < 0 || start.ParentCluster == goal.ParentCluster)
            {
                return null;
            }

            if (graphAbstraction.InsertStartAndGoal(start, goal))
            {
                LinkedList<PathNode> abstractPath = base.GetPath(graphAbstraction, 1, start, goal, capability, size, out int abstractExpandedNodes); // get path from abstract graph
                expandedNodes += abstractExpandedNodes;

                if (abstractPath != null)
                {
                    /*
                    Debug.Log("start abs path");
                    foreach (PathNode node in abstractPath)
                    {
                        Debug.log(node);
                    }
                    Debug.Log("end abs path");
                    */

                    path = new LinkedList<PathNode>();

                    LinkedListNode<PathNode> tmpNode = abstractPath.First;
                    path.AddFirst(tmpNode.Value);

                    while (tmpNode.Next != null) // while not at last node (goal)
                    {
                        LinkedList<PathNode> tmpPath = aaStar.GetPath(graphAbstraction, 0, tmpNode.Value, tmpNode.Next.Value, capability, size, out int partialExpandedNodes);
                        expandedNodes += partialExpandedNodes;
                        // Avoid overlapping nodes
                        if (tmpPath == null)
                        {
                            break;
                        }

                        tmpPath.RemoveFirst();
                        path.AddRange(tmpPath);

                        tmpNode = tmpNode.Next;
                    }
                }
                else
                {
                    Debug.Log("Abstract Path Empty");
                }

                graphAbstraction.RemoveStartAndGoal();
            }
        }

        return path;
    }

    public override LinkedList<PathNode> GetPath(GridGraphAbstraction graphAbstraction, Vector3 start, Vector3 goal, TerrainType capability, int size, out int expandedNodes)
    {
        GridMap map = graphAbstraction.GetGridMap();

        return GetPath(graphAbstraction, map.GetGridObject(start), map.GetGridObject(goal), capability, size, out expandedNodes);
    }

    protected override bool isTraversable(GridMap map, PathNode start, PathNode end, TerrainType capability, int size)
    {
        Edge edge = currentEdge;
        //Debug.Log($"Evaluating {start} {end}");
        if (edge.GetClearance(capability) >= size)
        {
            //Debug.Log("True");
            return true;
        }

        return false;
    }
}
