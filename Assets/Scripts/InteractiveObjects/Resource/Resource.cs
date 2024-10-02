using System;
using UnityEngine;

public class Resource : SelectableObject, IResource
{
    [SerializeField] private ResourceType _resourceType;

    private int _id;

    public event Action<Resource> Harvest;

    public bool IsOccupied { get; private set; }

    public int GetId()
    {
        return _id;
    }

    public void SetID(int id)
    {
        _id = id;
    }

    public ResourceType GetResourceType()
    {
        return _resourceType;
    }

    public void Occupy()
    {
        if (IsOccupied == false)
            IsOccupied = true;
    }

    public void Delete()
    {
        IsOccupied = false;
        Harvest?.Invoke(this);
    }
}
