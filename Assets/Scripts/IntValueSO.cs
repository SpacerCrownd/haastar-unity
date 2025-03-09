using System;
using UnityEngine;

[CreateAssetMenu]
public class IntValueSO : ScriptableObject
{
    [SerializeField]
    private int value;
    public EventHandler<int> valueChangeEvent;

    public int GetValue()
    {
        return value;
    }

    public void ChangeValue(int value)
    {
        this.value = value;
        valueChangeEvent?.Invoke(this, value);
    }
}
