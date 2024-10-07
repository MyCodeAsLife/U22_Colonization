using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScaner : MonoBehaviour
{
    private Transform _map;
    private int _selectMask;
    private Vector3 _scanningArea;
    private Coroutine _resourceScanning;

    public List<IResource> ResourcePool { get; private set; }

    private void OnEnable()
    {
        const float PlaneScale = 10;
        const string InteractiveLayerName = "Interactable";
        _map = GetComponentInParent<SceneObjectManager>().Map;
        _selectMask = LayerMask.NameToLayer(InteractiveLayerName);
        _scanningArea = new Vector3(_map.localScale.x * PlaneScale, _map.localScale.y * PlaneScale, _map.localScale.z * PlaneScale);

        if (_resourceScanning == null)
            _resourceScanning = StartCoroutine(ResourceScanning());
    }

    private void OnDisable()
    {
        StopCoroutine(_resourceScanning);
        _resourceScanning = null;
    }

    private void UpdateResourcePool()
    {
        List<IResource> list = new List<IResource>();
        Collider[] hits = Physics.OverlapBox(Vector3.zero, _scanningArea, Quaternion.identity, int.MaxValue ^ _selectMask);

        foreach (Collider hit in hits)
            if (hit.gameObject.layer == _selectMask && hit.TryGetComponent<IResource>(out IResource resource))
                list.Add(resource);

        ResourcePool = list;
    }

    private IEnumerator ResourceScanning()
    {
        const float Delay = 2.0f;
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            UpdateResourcePool();
        }
    }
}