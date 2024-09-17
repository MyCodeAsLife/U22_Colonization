using System;
using UnityEngine;

public class Resource : SelectableObject, IResource
{
    [SerializeField] private ResourceType _resourceType;
    private int _id;

    public event Action<Resource> Harvest;

    //public ResourceType ResourceType => _resourceType;
    //public int Id => _id;
    public int GetId() => _id;

    public void SetID(int id) => _id = id;

    public void Delete() => Harvest?.Invoke(this);

    public ResourceType GetResourceType() => _resourceType;
}
