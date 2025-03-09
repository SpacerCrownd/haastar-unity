using UnityEngine;
using UnityEngine.InputSystem;
using PathfindingConstants;

public class GridMapEditor : MonoBehaviour
{
    private GridMap map;
    private EditorInputActions editorInputActions;
    private TerrainType selectedTerrain = TerrainType.Rock;

    private Vector2 mousePosition;
    private bool holdingActive;

    private void Awake()
    {
        editorInputActions = new EditorInputActions();
    }

    private void Start()
    {
        map = LevelPathfinding.Instance.Grid;
    }

    private void OnEnable()
    {
        editorInputActions.Editor.Enable();
        editorInputActions.Editor.MousePosition.performed += OnMouseMove;
        editorInputActions.Editor.PlaceTile.performed += EditorInputActions_OnPlaceTile;
        editorInputActions.Editor.PlaceTile.started += EditorInputActions_OnPlaceTile;
        editorInputActions.Editor.PlaceTile.canceled += EditorInputActions_OnPlaceTile;
        editorInputActions.Editor.SelectTileGround.performed += EditorInputActions_OnSelectTileGround;
        editorInputActions.Editor.SelectTileWater.performed += EditorInputActions_OnSelectTileWater;
        editorInputActions.Editor.SelectTileRock.performed += EditorInputActions_OnSelectTileRock;
    }

    private void OnDisable()
    {
        editorInputActions.Editor.MousePosition.performed -= OnMouseMove;
        editorInputActions.Editor.PlaceTile.performed -= EditorInputActions_OnPlaceTile;
        editorInputActions.Editor.PlaceTile.started -= EditorInputActions_OnPlaceTile;
        editorInputActions.Editor.PlaceTile.canceled -= EditorInputActions_OnPlaceTile;
        editorInputActions.Editor.SelectTileGround.performed -= EditorInputActions_OnSelectTileGround;
        editorInputActions.Editor.SelectTileWater.performed -= EditorInputActions_OnSelectTileWater;
        editorInputActions.Editor.SelectTileRock.performed -= EditorInputActions_OnSelectTileRock;
        editorInputActions.Editor.Disable();
    }

    private void Update()
    {
        if(holdingActive)
        {
            PlaceTerrain();
        }
    }

    public void PlaceTerrain()
    {
        Vector2 clickedPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (map.GetGridPosition(clickedPosition, out int x, out int y))
        {
            map.GetGridObject(x, y).Terrain = selectedTerrain;
        }
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    private void EditorInputActions_OnPlaceTile(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            PlaceTerrain();
        }
        else if(context.phase == InputActionPhase.Performed)
        {
            holdingActive = true;
        }
        else
        {
            holdingActive = false;
        }
    }

    private void EditorInputActions_OnSelectTileGround(InputAction.CallbackContext context)
    {
        selectedTerrain = TerrainType.Ground;
    }

    private void EditorInputActions_OnSelectTileWater(InputAction.CallbackContext context)
    {
        selectedTerrain = TerrainType.Water;
    }

    private void EditorInputActions_OnSelectTileRock(InputAction.CallbackContext context)
    {
        selectedTerrain = TerrainType.Rock;
    }
}
