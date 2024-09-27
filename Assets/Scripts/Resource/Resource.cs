using System;
using UnityEngine;

public class Resource : SelectableObject, IResource
{
    [SerializeField] private ResourceType _resourceType;

    private int _id;

    public bool IsOccupied { get; private set; }

    public event Action<Resource> Harvest;

    public int GetId() => _id;
    public void SetID(int id) => _id = id;
    public ResourceType GetResourceType() => _resourceType;

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
