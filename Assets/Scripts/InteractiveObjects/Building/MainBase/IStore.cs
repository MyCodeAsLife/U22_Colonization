using System;

public interface IStore
{
    public event Action ResourcesQuantityChanged;

    public AmountOfResources AmountOfResources { get; }

    public void StoreResource(ResourceType resourceType);
    public void SubtractResources(AmountOfResources amount);
}
