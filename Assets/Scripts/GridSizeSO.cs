using System;
using UnityEngine;

[CreateAssetMenu]
public class GridSizeSO : ScriptableObject
{
    [SerializeField] 
    private int width;

    [SerializeField] 
    private int height;

    public EventHandler<SizeChangeEventArgs> sizeChangeEvent;

    public class SizeChangeEventArgs : EventArgs
    {
        public int width;
        public int height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void ChangeSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        sizeChangeEvent?.Invoke(this, new SizeChangeEventArgs { width = width, height = height});
    }
}
