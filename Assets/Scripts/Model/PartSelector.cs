using System;
using System.Collections.Generic;
using UnityEngine;

public class PartSelector : MonoBehaviour
{
    public Camera mainCamera;
    public List<PartInfo> partInfoList;
    public event Action<PartInfo> OnPartSelected;
    public Material highlightMat;

    private GameObject _currentHighlight;
    private readonly Dictionary<GameObject, Material[]> _originalMaterials = new Dictionary<GameObject, Material[]>();

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        SelectPart(hit.collider.gameObject);
    }

    public void SelectPart(GameObject go)
    {
        ClearHighlight();
        _currentHighlight = go;

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            _originalMaterials[go] = renderer.sharedMaterials;
            var mats = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++) mats[i] = highlightMat;
            renderer.materials = mats;
        }

        var info = partInfoList.Find(p => p.ObjectName == go.name);
        OnPartSelected?.Invoke(info);
    }

    private void ClearHighlight()
    {
        if (_currentHighlight == null) return;
        var renderer = _currentHighlight.GetComponent<Renderer>();
        if (renderer != null && _originalMaterials.TryGetValue(_currentHighlight, out var mats))
            renderer.materials = mats;
        _currentHighlight = null;
    }
}
