using System;

public interface IResource
{
    public event Action<Resource> Harvest;

    public ResourceType GetResourceType();

    public int GetId();

    public void Delete();
}
