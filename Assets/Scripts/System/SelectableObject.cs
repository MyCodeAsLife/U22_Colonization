using System;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    protected Transform _prefabSelectionIndicator;
    protected Transform _selectionIndicator;

    public event Action Destroyed;

    protected virtual void Awake()
    {
        _prefabSelectionIndicator = Resources.Load<Transform>("Prefabs/SelectionIndicator");
        _selectionIndicator = Instantiate(_prefabSelectionIndicator, transform);
        _selectionIndicator.gameObject.SetActive(false);
    }

    public virtual void OnHover()
    {
        transform.localScale = Vector3.one * 1.1f;                              // Magic
    }

    public virtual void OnUnhover()
    {
        transform.localScale = Vector3.one;
    }

    public virtual void Selected()
    {
        _selectionIndicator.gameObject.SetActive(true);
    }

    public virtual void UnSelect()
    {
        _selectionIndicator.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroyed?.Invoke();
    }
}
