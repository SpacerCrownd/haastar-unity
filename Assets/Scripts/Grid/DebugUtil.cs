using UnityEngine;

public class DebugUtil
{
    public static TextMesh CreateDebugText(string text, Transform parentTransform = null, Vector3 position = default(Vector3), int fontSize = 42, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft)
    {
        if (color == null) color = Color.white;
        return CreateDebugText(parentTransform, text, position, fontSize, (Color)color, textAnchor);
    }

    public static TextMesh CreateDebugText(Transform parentTransform, string text, Vector3 position, int fontSize, Color color, TextAnchor textAnchor)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));

        Transform transform = gameObject.transform;
        transform.SetParent(parentTransform, false);
        transform.localPosition = position;

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = TextAlignment.Left;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 1000;

        return textMesh;
    }
}
