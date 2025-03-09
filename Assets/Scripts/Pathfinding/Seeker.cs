using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PathfindingConstants;
using UnityEngine.EventSystems;

public class Seeker : MonoBehaviour
{
    private bool isPointerOverUI;

    private PlayerInputActions playerInputActions;

    [SerializeField] 
    private GameObject targetPrefab;
    private GameObject target = null;

    private Vector2 mousePosition;

    private GridMap map;
    private LinkedList<Vector3> pathList;
    private LinkedListNode<Vector3> pathListIterator;
    private const float speed = 100f;

    [SerializeField]
    private CapabilitySO capabilitySO;

    [SerializeField]
    private IntValueSO agentSizeSO;

    [SerializeField]
    private CapabilitySO agentCapabilitySO;

    private TerrainType capability;

    private int size = 1;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // get gridmap
        map = LevelPathfinding.Instance.Grid;
        Vector3 transition = Vector3.one;
        transition = transition * map.CellSize * .5f;
        transition.y += (size - 1) * map.CellSize;
        transition.z = 0;
        transform.position = map.GetWorldPosition(0, 0) + transition;

        agentSizeSO.valueChangeEvent += OnSizeChange;
    }

    private void OnEnable()
    {
        agentCapabilitySO.capabilityChangeEvent += OnCapabilityChange;
        capability = capabilitySO.GetCapability();
        size = agentSizeSO.GetValue();

        // Enable input events
        playerInputActions.Pathfinding.Enable();
        playerInputActions.Pathfinding.MousePosition.performed += OnMouseMove;
        playerInputActions.Pathfinding.PlaceDestination.performed += OnPlaceDestination;
        playerInputActions.Pathfinding.InitializeMap.performed += OnInitializeMap;
    }

    private void OnDisable()
    {
        agentCapabilitySO.capabilityChangeEvent -= OnCapabilityChange;

        // Disable input events
        playerInputActions.Pathfinding.MousePosition.performed -= OnMouseMove;
        playerInputActions.Pathfinding.PlaceDestination.performed -= OnPlaceDestination;
        playerInputActions.Pathfinding.InitializeMap.performed -= OnInitializeMap;
        playerInputActions.Pathfinding.Disable();
    }

    void Update()
    {
        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        HandleMovement();
    }

    private void OnCapabilityChange(object sender, TerrainType e)
    {
        this.capability = e;
    }

    private void OnInitializeMap(InputAction.CallbackContext context)
    {
        LevelPathfinding.Instance.InitializeMap();
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    private void OnPlaceDestination(InputAction.CallbackContext context)
    {
        Vector2 clickedPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        SetTargetPosition(clickedPosition);

        if (pathList?.First is not null)
        {
            for(LinkedListNode<Vector3> pathIterator = pathList.First; pathIterator.Next != null; pathIterator = pathIterator.Next)
            {
                Debug.DrawLine(pathIterator.Value, pathIterator.Next.Value, Color.white, 10f);
            }
        }
    }

    private void HandleMovement()
    {
        if (pathList != null)
        {
            Vector3 targetPosition = pathListIterator.Value;

            // approximate destination position
            if (Vector3.Distance(transform.position, targetPosition) > .5f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else // go to next path node
            {
                pathListIterator = pathListIterator.Next;
                if (pathListIterator == null)
                {
                    pathList = null;
                }
            }
        }
    }

    private void SetTargetPosition(Vector3 targetPosition)
    {
        //if not clicked on UI
        if (!isPointerOverUI)
        {
            // if clicked position is within map bounds
            if (map.GetGridPosition(targetPosition, out int x, out int y))
            {
                Vector3 newPosition = map.GetWorldPosition(x, y) + map.CellSize * Vector3.one / 2;
                newPosition.z = 1;

                // position target sprite
                if (target == null)
                    target = Instantiate(targetPrefab, newPosition, Quaternion.identity); // instatiate if not instantiated yet
                else
                    target.transform.position = newPosition;

                // get path from pathfinding
                pathList = LevelPathfinding.Instance.GetPath(transform.position, targetPosition, capability, size);
                pathListIterator = pathList?.First;
            }
        }
    }

    private void OnSizeChange(object sender, int value)
    {
        size = value;
    }
}
