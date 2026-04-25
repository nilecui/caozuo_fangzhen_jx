using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeViewController : MonoBehaviour
{
    [System.Serializable]
    public class SubsystemGroup
    {
        public string name;
        public List<Transform> parts;
        public Vector3 explodeDirection;
        public float explodeDistance = 2f;
    }

    public List<SubsystemGroup> subsystems;
    public float animDuration = 0.6f;

    private bool _isExploded;
    private readonly Dictionary<Transform, Vector3> _originalPositions = new Dictionary<Transform, Vector3>();

    private void Start()
    {
        foreach (var group in subsystems)
            foreach (var part in group.parts)
                if (part != null && !_originalPositions.ContainsKey(part))
                    _originalPositions[part] = part.localPosition;
    }

    public void ToggleExplode()
    {
        _isExploded = !_isExploded;
        StopAllCoroutines();
        StartCoroutine(_isExploded ? AnimateExplode() : AnimateCollapse());
    }

    public void ExplodeSubsystem(string subsystemName)
    {
        var group = subsystems.Find(g => g.name == subsystemName);
        if (group == null) return;
        StartCoroutine(AnimateGroup(group, explode: true));
    }

    private IEnumerator AnimateExplode()
    {
        foreach (var group in subsystems)
            StartCoroutine(AnimateGroup(group, explode: true));
        yield return new WaitForSeconds(animDuration);
    }

    private IEnumerator AnimateCollapse()
    {
        foreach (var group in subsystems)
            StartCoroutine(AnimateGroup(group, explode: false));
        yield return new WaitForSeconds(animDuration);
    }

    private IEnumerator AnimateGroup(SubsystemGroup group, bool explode)
    {
        float elapsed = 0f;
        var targets = new Dictionary<Transform, Vector3>();

        foreach (var part in group.parts)
        {
            if (part == null) continue;
            Vector3 origin = _originalPositions.TryGetValue(part, out var o) ? o : part.localPosition;
            Vector3 dest = explode
                ? origin + group.explodeDirection.normalized * group.explodeDistance
                : origin;
            targets[part] = dest;
        }

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            foreach (var kv in targets)
            {
                if (kv.Key == null) continue;
                Vector3 from = _originalPositions.TryGetValue(kv.Key, out var orig)
                    ? (explode ? orig : kv.Key.localPosition)
                    : kv.Key.localPosition;
                kv.Key.localPosition = Vector3.Lerp(from, kv.Value, t);
            }
            yield return null;
        }

        foreach (var kv in targets)
            if (kv.Key != null)
                kv.Key.localPosition = kv.Value;
    }
}
