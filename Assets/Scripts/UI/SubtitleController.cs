using UnityEngine;
using UnityEngine.UIElements;

public class SubtitleController : MonoBehaviour
{
    private Label _subtitleText;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _subtitleText = root.Q<Label>("subtitle-text");
    }

    public void ShowSubtitle(string text)
    {
        if (_subtitleText != null)
            _subtitleText.text = text;
    }

    public void ClearSubtitle()
    {
        if (_subtitleText != null)
            _subtitleText.text = "";
    }
}
