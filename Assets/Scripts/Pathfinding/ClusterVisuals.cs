using System.Collections.Generic;
using UnityEngine;

public class ClusterVisuals : MonoBehaviour
{
    [SerializeField]
    private GameObject entrancePrefab;
    [SerializeField]
    private Line linePrefab;
    [SerializeField]
    private Line entranceConnectionPrefab;

    private GridMap map;
    private List<Line> clusterLines = new List<Line>();
    private List<GameObject> entrances = new List<GameObject>();
    private List<Line> entranceConnections = new List<Line>();

    public void SetMap(GridMap map)
    {
        this.map = map;
    }

    public void ClearClusters()
    {
        foreach (Line line in clusterLines)
        {
            Destroy(line.gameObject);
        }
        clusterLines.Clear();
    }

    public void ShowCluster(Cluster cluster)
    {
        Line newLine = Instantiate(linePrefab);
        clusterLines.Add(newLine);

        Vector2 width = new Vector2(cluster.Width, 0);
        Vector2 height = new Vector2(0, cluster.Height);
        Vector2 origin = new Vector2(cluster.PosX, cluster.PosY);

        newLine.SetPosition(origin * map.CellSize);
        newLine.SetPosition((origin + width) * map.CellSize);
        newLine.SetPosition((origin + width + height) * map.CellSize);
        newLine.SetPosition((origin + height) * map.CellSize);
    }

    public void ClearEntrances()
    {
        foreach (GameObject entrance in entrances)
        {
            Destroy(entrance.gameObject);
        }
        entrances.Clear();

        foreach(Line connection in entranceConnections)
        {
            Destroy(connection.gameObject);
        }
        entranceConnections.Clear();
    }

    public void ShowEntrance(PathNode node)
    {
        Vector3 entrancePosition = map.GetWorldPosition(node.X, node.Y);
        entrancePosition += map.CellSize/2 * Vector3.right;
        entrancePosition += map.CellSize/2 * Vector3.up;
        GameObject newEntrance = Instantiate(entrancePrefab, entrancePosition, Quaternion.identity);
        entrances.Add(newEntrance);

        if(node.EntranceDestination is not null)
        {
            Vector3 destinationPosition = map.GetWorldPosition(node.EntranceDestination.X, node.EntranceDestination.Y) + (map.CellSize/2 * Vector3.one);

            Line newLine = Instantiate(entranceConnectionPrefab);
            newLine.SetPosition(entrancePosition);
            newLine.SetPosition(destinationPosition);
            entranceConnections.Add(newLine);
        }
    }
}
