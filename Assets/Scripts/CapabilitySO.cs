using PathfindingConstants;
using System;
using UnityEngine;

[CreateAssetMenu]
public class CapabilitySO : ScriptableObject
{
    [SerializeField]
    private TerrainType capability = TerrainType.None;

    public EventHandler<TerrainType> capabilityChangeEvent;

    public TerrainType GetCapability()
    {
        return capability;
    }

    public void AddCapability(TerrainType capability)
    {
        this.capability = this.capability | capability;
        capabilityChangeEvent?.Invoke(this, this.capability);
    }

    public void RemoveCapability(TerrainType capability)
    {
        this.capability = this.capability & ~capability;
        capabilityChangeEvent?.Invoke(this, this.capability);
    }
}
