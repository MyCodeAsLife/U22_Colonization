using System.Collections.Generic;
using UnityEngine;

public class ResourceScaner
{
    private int _selectMask;
    private Vector3 _scanningArea;

    public ResourceScaner(Transform map)
    {
        const float PlaneScale = 10;
        const string InteractiveLayerName = "Interactable";
        _selectMask = LayerMask.NameToLayer(InteractiveLayerName);
        _scanningArea = new Vector3(map.localScale.x * PlaneScale, map.localScale.y * PlaneScale, map.localScale.z * PlaneScale);
    }

    public List<IResource> MapScaning()
    {
        List<IResource> list = new List<IResource>();
        Collider[] hits = Physics.OverlapBox(Vector3.zero, _scanningArea, Quaternion.identity, int.MaxValue ^ _selectMask);

        foreach (Collider hit in hits)
            if (hit.gameObject.layer == _selectMask && hit.TryGetComponent<IResource>(out IResource resource))
                list.Add(resource);

        return list;
    }
}