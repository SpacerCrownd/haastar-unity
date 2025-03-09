using UnityEngine;
using PathfindingConstants;
using System.Collections.Generic;

public class GridMapVisuals : MonoBehaviour
{
    [SerializeField] 
    private Tile tilePrefab;
    private Tile[,] tileGrid;
    private HashSet<Tile> modifiedTiles;
    private GridMap map;

    private Color waterColor = new Color(.0f, .0f, 1f);
    private Color groundColor = new Color(.929f, .655f, .384f);
    private Color rockColor = new Color(0.529f, 0.549f, 0.659f);
    private Color visitedColor = new Color(0.82f, 0.82f, 0.82f);
    private Color expandedColor = new Color(0.52f, 0.52f, 0.52f);
    private Color pathColor = new Color(1f, 1f, 0f);

    private void Start()
    {
        CreateGridMapTiles();
        map.OnGridObjectChanged += Grid_OnGridObjectChanged;
    }

    public void CreateGridMapTiles()
    {
        Clear();
        tileGrid = new Tile[map.Width, map.Height];
        modifiedTiles = new HashSet<Tile>();

        foreach (PathNode node in map.Grid)
        {
            Vector3 position = map.GetWorldPosition(node.X, node.Y) + Vector3.one * map.CellSize / 2;
            tileGrid[node.X, node.Y] = Instantiate(tilePrefab, position, Quaternion.identity);

            switch (node.Terrain)
            {
                case TerrainType.Ground:
                    tileGrid[node.X, node.Y].SetColor(groundColor);
                    tileGrid[node.X, node.Y].SetDefaultColor(groundColor);
                    break;
                case TerrainType.Water:
                    tileGrid[node.X, node.Y].SetColor(waterColor);
                    tileGrid[node.X, node.Y].SetDefaultColor(waterColor);
                    break;
                case TerrainType.Rock:
                    tileGrid[node.X, node.Y].SetColor(rockColor);
                    tileGrid[node.X, node.Y].SetDefaultColor(rockColor);
                    break;
            }
        }
    }

    public void Clear()
    {
        if(tileGrid != null)
        {
            foreach (Tile tile in tileGrid)
            {
                Destroy(tile.gameObject);
            }
            tileGrid = null;
        }
    }

    public void SetMap(GridMap map)
    {
        this.map = map;
    }

    public void ResetTiles()
    {
        foreach(Tile tile in modifiedTiles)
        {
            tile.ResetTile();
            tile.DisableText();
        }

        modifiedTiles.Clear();
    }

    public void SetTileVisited(int x, int y)
    {
        if(tileGrid[x, y].GetColor() != pathColor) // if tile is part of path then don't change color
            tileGrid[x, y].SetColor(visitedColor);
    }

    public void SetTileExpanded(int x, int y)
    {
        tileGrid[x, y].SetColor(expandedColor);
    }

    public void SetTilePath(int x, int y)
    {
        tileGrid[x, y].SetColor(pathColor);
    }

    private void Grid_OnGridObjectChanged(object sender, GridMap.OnGridObjectChangedEventArgs e)
    {
        switch (map.GetGridObject(e.x, e.y).Terrain)
        {
            case TerrainType.Ground:
                tileGrid[e.x, e.y].SetColor(groundColor);
                tileGrid[e.x, e.y].SetDefaultColor(groundColor);
                break;
            case TerrainType.Water:
                tileGrid[e.x, e.y].SetColor(waterColor);
                tileGrid[e.x, e.y].SetDefaultColor(waterColor);
                break;
            case TerrainType.Rock:
                tileGrid[e.x, e.y].SetColor(rockColor);
                tileGrid[e.x, e.y].SetDefaultColor(rockColor);
                break;
        }
    }

    public void SetCostText(PathNode node)
    {
        tileGrid[node.X, node.Y].EnableText();
        tileGrid[node.X, node.Y].SetCostText(node.GCost, node.HCost, node.FCost);

        modifiedTiles.Add(tileGrid[node.X, node.Y]);
    }

    public void SetCleranceText()
    {
        foreach(PathNode node in map.Grid)
        {
            tileGrid[node.X, node.Y].EnableText();
            tileGrid[node.X, node.Y].SetClearanceText(node.GetClearance(TerrainType.Ground), node.GetClearance(TerrainType.Water), node.GetClearance(TerrainType.Ground | TerrainType.Water));
        }
    }
}