using System;

public interface IStore
{
    public AmountOfResources AmountOfResources { get; }

    public event Action<int> FoodQuantityChanged;
    public event Action<int> TimberQuantityChanged;
    public event Action<int> MarbleQuantityChanged;

    public void StoreResource(ResourceType resourceType);
    public void SubtractResources(AmountOfResources amount);
}
