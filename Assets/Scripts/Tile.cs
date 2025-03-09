using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    [SerializeField]
    private TextMeshPro fCostText, gCostText, hCostText;

    [SerializeField]
    private TextMeshPro groundClearance, waterClearance, rockClearance;

    [SerializeField]
    private GameObject textGameObject;

    public void ResetTile()
    {
        spriteRenderer.color = defaultColor;
        ClearCostText();
    }

    public void SetDefaultColor(Color color)
    {
        this.defaultColor = color;
    }

    public Color DefaultColor { get; }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public Color GetColor() 
    {
        return spriteRenderer.color;
    }

    public void SetCostText(float g, float h, float f)
    {
        gCostText.text = g.ToString();
        hCostText.text = h.ToString();
        fCostText.text = f.ToString();
    }

    public void ClearCostText()
    {
        gCostText.text = null;
        hCostText.text = null;
        fCostText.text = null;
    }

    public void SetClearanceText(int ground, int water, int rock)
    {
        groundClearance.text = ground.ToString();
        waterClearance.text = water.ToString();
        rockClearance.text = rock.ToString();
    }

    public void EnableText()
    {
        textGameObject.SetActive(true);
    }

    public void DisableText()
    {
        textGameObject.SetActive(false);
    }
}
