using System;
using System.Collections.Generic;
using System.Linq;
using PathfindingConstants;

public class Graph
{
    private struct Coordinates : IEquatable<Coordinates>
    {
        public int x;
        public int y;

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Coordinates other)
        {
            return other.x == x && other.y == y;
        }
    }

    private Dictionary<Coordinates, PathNode> nodes = new Dictionary<Coordinates, PathNode>();
    private Dictionary<PathNode, List<Edge>> edges = new Dictionary<PathNode, List<Edge>>();

    public void AddEdge(PathNode from, PathNode to, float cost)
    {
        if(edges.TryGetValue(from, out List<Edge> edgesList))
        {
            edgesList.Add(new Edge(cost, from, to));
        }
        else
        {
            throw new Exception("AddEdge Exception : Invalid Node");
        }
    }

    public Edge AddAnnotatedEdge(PathNode from, PathNode to, float cost, TerrainType capability, int clearance)
    {
        if (edges.TryGetValue(from, out List<Edge> edgesList))
        {
            Edge edge = new Edge(cost, from, to, capability, clearance);
            edgesList.Add(edge);
            return edge;
        }
        else
        {
            throw new Exception("AddAnnotatedEdge Exception : Invalid Node");
        }
    }

    public void AddNode(PathNode node)
    {
        if(!nodes.TryGetValue(new Coordinates(node.X, node.Y), out PathNode val))
        {
            nodes[new Coordinates(node.X, node.Y)] = node;
            edges.TryAdd(node, new List<Edge>());
        }
    }

    public IEnumerable<Edge> GetEdges(PathNode currentNode)
    {
        List<Edge> edgesList = new List<Edge>();
        if(edges.TryGetValue(currentNode, out edgesList))
        {
            foreach (Edge edge in edgesList)
            {
                yield return edge;
            }
        }
    }

    public Edge GetAnnotatedEdge(PathNode from, PathNode to, TerrainType capability, int clearance)
    {
        foreach(Edge edge in GetEdges(from))
        {
            if(edge.ToNode == to && edge.Capability == capability && edge.GetClearance(capability) >= clearance)
            {
                return edge;
            }
        }

        return null;
    }

    public Edge FindDominantEdge(PathNode from, PathNode to, TerrainType capability, int clearance, int cost)
    {
        foreach (Edge edge in GetEdges(from))
        {
            if (edge.ToNode == to && edge.Capability == capability && edge.GetClearance(capability) >= clearance && edge.Label <= cost)
            {
                return edge;
            }
        }

        return null;
    }

    public PathNode GetNode(int x, int y) 
    {
        if(nodes.TryGetValue(new Coordinates(x, y), out PathNode result))
            return result;
        else
            return null;
    }

    public List<PathNode> GetNodes()
    {
        return nodes.Values.ToList();
    }

    public void Remove(PathNode node)
    {
        Dictionary<PathNode, List<Edge>> edgesToRemove = new Dictionary<PathNode, List<Edge>>();

        edges.Remove(node);
        nodes.Remove(new Coordinates(node.X, node.Y));

        foreach(KeyValuePair<PathNode, List<Edge>> edgesEntry in edges)
        {
            edgesToRemove.Add(edgesEntry.Key, new List<Edge>());
            foreach (Edge edge in edgesEntry.Value)
            {
                if(edge.ToNode == node)
                {
                    edgesToRemove[edge.FromNode].Add(edge);
                }
            }
        }

        foreach(KeyValuePair<PathNode, List<Edge>> edgesEntry in edgesToRemove)
        {
            foreach (Edge edge in edgesEntry.Value)
            {
                edges[edgesEntry.Key].Remove(edge);
            }
        }
    }
}

public class Edge
{
    private float label;
    private TerrainType capability;
    private int clearance;
    private PathNode fromNode;
    private PathNode toNode;

    public float Label 
    { 
        get => label; 
        set => label = value;
    }

    public byte Direction
    {
        get;
    }

    public PathNode FromNode
    {
        get => fromNode;
        set => fromNode = value;
    }

    public PathNode ToNode
    {
        get => toNode;
        set => toNode = value;
    }

    public TerrainType Capability
    {
        get => capability;
        set => capability = value;
    }

    public Edge(float label, PathNode from, PathNode to)
    {
        this.label = label;
        this.fromNode = from;
        this.toNode = to;
        this.clearance = 0;
        this.capability = TerrainType.None;
    }

    public Edge(float label, PathNode from, PathNode to, TerrainType capability, int clearance)
    {
        this.label = label;
        this.fromNode = from;
        this.toNode = to;
        this.clearance = clearance;
        this.capability = capability;
    }

    public int GetClearance(TerrainType capability)
    {
        if ((this.capability & capability) == this.capability)
            return this.clearance;
        else
            return 0;
    }

    public override string ToString()
    {
        return $"Edge <Cost: {label}, From: {FromNode}, To: {ToNode}, Capability: {capability}, Clearance: {clearance}>";
    }
}
