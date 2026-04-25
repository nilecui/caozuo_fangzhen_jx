using UnityEngine;
using UnityEngine.UIElements;

public class InfoPanelController : MonoBehaviour
{
    public PartSelector partSelector;

    private Label _partName, _partSpec, _partMaterial, _partDesc;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _partName     = root.Q<Label>("part-name");
        _partSpec     = root.Q<Label>("part-spec");
        _partMaterial = root.Q<Label>("part-material");
        _partDesc     = root.Q<Label>("part-desc");

        if (partSelector != null)
            partSelector.OnPartSelected += UpdatePanel;
    }

    private void OnDisable()
    {
        if (partSelector != null)
            partSelector.OnPartSelected -= UpdatePanel;
    }

    private void UpdatePanel(PartInfo info)
    {
        if (info == null)
        {
            _partName.text = "---";
            _partSpec.text = _partMaterial.text = _partDesc.text = "";
            return;
        }
        _partName.text     = info.DisplayName;
        _partSpec.text     = $"型号：{info.Model}";
        _partMaterial.text = $"材质：{info.Material}";
        _partDesc.text     = info.Description;
    }
}
