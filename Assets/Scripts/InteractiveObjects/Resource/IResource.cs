using System;

public interface IResource
{
    public bool IsOccupied { get; }

    public event Action<Resource> Harvest;

    public ResourceType GetResourceType();

    public int GetId();

    public void Delete();

    public void Occupy();
}
