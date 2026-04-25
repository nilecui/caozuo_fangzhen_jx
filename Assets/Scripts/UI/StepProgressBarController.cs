using UnityEngine;
using UnityEngine.UIElements;

public class StepProgressBarController : MonoBehaviour
{
    private Label _counter;
    private ProgressBar _bar;
    private Label _hint;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _counter = root.Q<Label>("step-counter");
        _bar     = root.Q<ProgressBar>("step-progress");
        _hint    = root.Q<Label>("step-hint");
    }

    public void UpdateProgress(int current, int total, string hintText)
    {
        _counter.text = $"步骤 {current}/{total}";
        _bar.value    = total > 0 ? (float)current / total * 100f : 0f;
        _hint.text    = hintText ?? "";
    }
}
