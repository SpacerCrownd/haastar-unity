using System;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    public void SetPosition(Vector2 position)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }

    public static explicit operator Line(GameObject v)
    {
        throw new NotImplementedException();
    }
}