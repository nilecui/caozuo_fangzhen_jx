using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class PrincipleDemoController : MonoBehaviour
{
    [System.Serializable]
    public class SubsystemTimeline
    {
        public string name;
        public PlayableDirector director;
    }

    public SubsystemTimeline[] timelines;

    private PlayableDirector _current;
    private Label _subtitle;
    private Slider _progressSlider;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _subtitle       = root.Q<Label>("subtitle-text");
        _progressSlider = root.Q<Slider>("principle-progress");

        root.Q<Button>("btn-mechanical").RegisterCallback<ClickEvent>(_ => PlayTimeline("机械系统"));
        root.Q<Button>("btn-hydraulic").RegisterCallback<ClickEvent>(_ => PlayTimeline("液压系统"));
        root.Q<Button>("btn-electrical").RegisterCallback<ClickEvent>(_ => PlayTimeline("电气系统"));
        root.Q<Button>("btn-pause").RegisterCallback<ClickEvent>(_ => TogglePause());
        root.Q<Button>("btn-replay").RegisterCallback<ClickEvent>(_ => _current?.Play());
    }

    private void Update()
    {
        if (_current == null || _current.state != PlayState.Playing) return;
        if (_progressSlider != null)
            _progressSlider.value = (float)(_current.time / _current.duration) * 100f;
    }

    private void PlayTimeline(string subsystemName)
    {
        _current?.Stop();
        var entry = System.Array.Find(timelines, t => t.name == subsystemName);
        if (entry?.director == null)
        {
            Debug.LogWarning($"[PrincipleDemo] No timeline for: {subsystemName}");
            return;
        }
        _current = entry.director;
        _current.Play();
        if (_subtitle != null)
            _subtitle.text = $"正在演示：{subsystemName}原理";
    }

    private void TogglePause()
    {
        if (_current == null) return;
        if (_current.state == PlayState.Playing)
            _current.Pause();
        else
            _current.Resume();
    }
}
