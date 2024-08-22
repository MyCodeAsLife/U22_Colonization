using System;

public interface IStore
{
    public event Action<int> FoodQuantityChanged;
    public event Action<int> TimberQuantityChanged;
    public event Action<int> MarbleQuantityChanged;

    public AmountOfResources GetAmountOfResources();
}
