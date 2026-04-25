using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FeedbackOverlayController : MonoBehaviour
{
    private VisualElement _viewport;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _viewport = root.Q<VisualElement>("viewport");
    }

    public void ShowCorrect(float duration = 1.5f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedback("feedback-correct", duration));
    }

    public void ShowError(float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedback("feedback-error", duration));
    }

    private IEnumerator ShowFeedback(string className, float duration)
    {
        _viewport.AddToClassList(className);
        yield return new WaitForSeconds(duration);
        _viewport.RemoveFromClassList(className);
    }
}
