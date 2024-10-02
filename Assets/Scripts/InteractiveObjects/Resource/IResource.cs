using System;

public interface IResource
{
    public event Action<Resource> Harvest;

    public bool IsOccupied { get; }

    public ResourceType GetResourceType();

    public int GetId();

    public void Delete();

    public void Occupy();
}
