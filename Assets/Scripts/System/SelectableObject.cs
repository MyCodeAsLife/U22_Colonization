using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private Transform _prefabSelectionIndicator;
    private Transform _selectionIndicator;

    protected virtual void Awake()
    {
        _prefabSelectionIndicator = Resources.Load<Transform>("Prefabs/SelectionIndicator");
        //_selectionIndicator = Instantiate(_prefabSelectionIndicator, transform);
        //_selectionIndicator.gameObject.SetActive(false);
    }

    public virtual void OnHover()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public virtual void OnUnhover()
    {
        transform.localScale = Vector3.one;
    }

    public virtual void Select()
    {
        _selectionIndicator = Instantiate(_prefabSelectionIndicator, transform);
        //_selectionIndicator.gameObject.SetActive(true);
    }

    public virtual void UnSelect()
    {
        Destroy(_selectionIndicator.gameObject);
        //_selectionIndicator.gameObject.SetActive(false);
    }
}
