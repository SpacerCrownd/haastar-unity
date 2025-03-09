using PathfindingConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HAAStarUI : MonoBehaviour
{
    [SerializeField]
    private Slider agentSizeSlider;
    [SerializeField]
    private TextMeshProUGUI agentSizeSliderValue;
    [SerializeField]
    private TMP_InputField gridWidthInput;
    [SerializeField]
    private TMP_InputField gridHeightInput;
    [SerializeField]
    private TMP_InputField clusterSizeInput;
    [SerializeField]
    private Toggle groundToggle;
    [SerializeField] 
    private Toggle waterToggle;
    [SerializeField]
    private TMP_Dropdown searchAlgDropdown;
    [SerializeField]
    private TMP_Text expandedNodes;

    [SerializeField]
    private IntValueSO agentSizeSO;
    [SerializeField]
    private IntValueSO clusterSizeSO;
    [SerializeField]
    private GridSizeSO gridSizeSO;
    [SerializeField]
    private CapabilitySO agentCapabilitySO;
    [SerializeField]
    private IntValueSO expandedNodesSO;


    public void OnEnable()
    {
        agentSizeSlider.value = agentSizeSO.GetValue();
        agentSizeSliderValue.text = agentSizeSO.GetValue().ToString();
        gridWidthInput.text = gridSizeSO.GetWidth().ToString();
        gridHeightInput.text = gridSizeSO.GetHeight().ToString();
        clusterSizeInput.text = clusterSizeSO.GetValue().ToString();
        groundToggle.isOn = (agentCapabilitySO.GetCapability() & TerrainType.Ground) == TerrainType.Ground;
        waterToggle.isOn = (agentCapabilitySO.GetCapability() & TerrainType.Water) == TerrainType.Water;

        agentSizeSlider.onValueChanged.AddListener(OnAgentSizeSliderChange);
        searchAlgDropdown.onValueChanged.AddListener(OnSearchAlgorithmChange);

        expandedNodesSO.valueChangeEvent += OnSearchFinished;
    }

    private void OnSearchFinished(object sender, int value)
    {
        expandedNodes.SetText(value.ToString());
    }

    private void OnDisable()
    {
        agentSizeSlider.onValueChanged.RemoveListener(OnAgentSizeSliderChange);
        searchAlgDropdown.onValueChanged.RemoveListener(OnSearchAlgorithmChange);
        expandedNodesSO.valueChangeEvent -= OnSearchFinished;
    }

    private void OnAgentSizeSliderChange(float value)
    {
        agentSizeSO.ChangeValue((int)value);
        agentSizeSliderValue.SetText(value.ToString());
    }

    public void OnBuildMap()
    {
        gridSizeSO.ChangeSize(int.Parse(gridWidthInput.text), int.Parse(gridHeightInput.text));
        clusterSizeSO.ChangeValue(int.Parse(clusterSizeInput.text));
    }

    public void OnGroundToggle()
    {
        if (groundToggle.isOn) 
        {
            agentCapabilitySO.AddCapability(PathfindingConstants.TerrainType.Ground);
        }
        else
        {
            agentCapabilitySO.RemoveCapability(PathfindingConstants.TerrainType.Ground);
        }
    }

    public void OnWaterToggle()
    {
        if (waterToggle.isOn)
        {
            agentCapabilitySO.AddCapability(PathfindingConstants.TerrainType.Water);
        }
        else
        {
            agentCapabilitySO.RemoveCapability(PathfindingConstants.TerrainType.Water);
        }
    }

    public void OnSearchAlgorithmChange(int option)
    {
        switch (option)
        {
            case 0:
                LevelPathfinding.Instance.UseHAAStar();
                break;
            case 1:
                LevelPathfinding.Instance.UseAAStar();
                break;
        }
    }
}
