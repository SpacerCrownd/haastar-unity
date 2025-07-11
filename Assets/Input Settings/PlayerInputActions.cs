//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input Settings/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Pathfinding"",
            ""id"": ""affdd737-ae04-4825-97bd-194162c2eb61"",
            ""actions"": [
                {
                    ""name"": ""PlaceDestination"",
                    ""type"": ""Button"",
                    ""id"": ""b988c877-5e8f-48c8-aa3e-819fa67ad0d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""31f17d67-cc5f-49f9-9e6b-fb9806413af6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""InitializeMap"",
                    ""type"": ""Button"",
                    ""id"": ""73198c15-0fbe-4f07-892c-465d15facfed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""30fb2a72-9e56-4616-87fd-7aae6c05836b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlaceDestination"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50030078-406d-4ca7-9e5d-448932c0013f"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a144c134-7788-4f9a-a780-1d9dafa92146"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InitializeMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Pathfinding
        m_Pathfinding = asset.FindActionMap("Pathfinding", throwIfNotFound: true);
        m_Pathfinding_PlaceDestination = m_Pathfinding.FindAction("PlaceDestination", throwIfNotFound: true);
        m_Pathfinding_MousePosition = m_Pathfinding.FindAction("MousePosition", throwIfNotFound: true);
        m_Pathfinding_InitializeMap = m_Pathfinding.FindAction("InitializeMap", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Pathfinding
    private readonly InputActionMap m_Pathfinding;
    private List<IPathfindingActions> m_PathfindingActionsCallbackInterfaces = new List<IPathfindingActions>();
    private readonly InputAction m_Pathfinding_PlaceDestination;
    private readonly InputAction m_Pathfinding_MousePosition;
    private readonly InputAction m_Pathfinding_InitializeMap;
    public struct PathfindingActions
    {
        private @PlayerInputActions m_Wrapper;
        public PathfindingActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @PlaceDestination => m_Wrapper.m_Pathfinding_PlaceDestination;
        public InputAction @MousePosition => m_Wrapper.m_Pathfinding_MousePosition;
        public InputAction @InitializeMap => m_Wrapper.m_Pathfinding_InitializeMap;
        public InputActionMap Get() { return m_Wrapper.m_Pathfinding; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PathfindingActions set) { return set.Get(); }
        public void AddCallbacks(IPathfindingActions instance)
        {
            if (instance == null || m_Wrapper.m_PathfindingActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PathfindingActionsCallbackInterfaces.Add(instance);
            @PlaceDestination.started += instance.OnPlaceDestination;
            @PlaceDestination.performed += instance.OnPlaceDestination;
            @PlaceDestination.canceled += instance.OnPlaceDestination;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
            @InitializeMap.started += instance.OnInitializeMap;
            @InitializeMap.performed += instance.OnInitializeMap;
            @InitializeMap.canceled += instance.OnInitializeMap;
        }

        private void UnregisterCallbacks(IPathfindingActions instance)
        {
            @PlaceDestination.started -= instance.OnPlaceDestination;
            @PlaceDestination.performed -= instance.OnPlaceDestination;
            @PlaceDestination.canceled -= instance.OnPlaceDestination;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
            @InitializeMap.started -= instance.OnInitializeMap;
            @InitializeMap.performed -= instance.OnInitializeMap;
            @InitializeMap.canceled -= instance.OnInitializeMap;
        }

        public void RemoveCallbacks(IPathfindingActions instance)
        {
            if (m_Wrapper.m_PathfindingActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPathfindingActions instance)
        {
            foreach (var item in m_Wrapper.m_PathfindingActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PathfindingActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PathfindingActions @Pathfinding => new PathfindingActions(this);
    public interface IPathfindingActions
    {
        void OnPlaceDestination(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnInitializeMap(InputAction.CallbackContext context);
    }
}
