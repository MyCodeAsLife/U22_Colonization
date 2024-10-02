using System;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    protected Transform SelectionIndicator;

    private Transform _prefabSelectionIndicator;
    private float _shiftSize;

    public event Action Destroyed;

    protected virtual void Awake()
    {
        _shiftSize = 1.1f;
        _prefabSelectionIndicator = Resources.Load<Transform>("Prefabs/SelectionIndicator");
        SelectionIndicator = Instantiate(_prefabSelectionIndicator, transform);
        SelectionIndicator.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroyed?.Invoke();
    }

    public virtual void OnHover()
    {
        transform.localScale = Vector3.one * _shiftSize;
    }

    public virtual void OnUnhover()
    {
        transform.localScale = Vector3.one;
    }

    public virtual void Selected()
    {
        SelectionIndicator.gameObject.SetActive(true);
    }

    public virtual void UnSelect()
    {
        SelectionIndicator.gameObject.SetActive(false);
    }
}
