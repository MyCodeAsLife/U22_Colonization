using System;

public class Store : IStore
{
    private AmountOfResources _amountOfResources = new();               // Убрать данный класс и сделать на обычных int?

    public AmountOfResources AmountOfResources { get { return _amountOfResources; } }

    public event Action<int> FoodQuantityChanged;
    public event Action<int> TimberQuantityChanged;
    public event Action<int> MarbleQuantityChanged;

    public Store(int amountFood = 0, int amountTimber = 0, int amountMarble = 0)
    {
        _amountOfResources.Food = 0;
        _amountOfResources.Timber = 0;
        _amountOfResources.Marble = 0;
    }

    public void AddFood(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Food += amount;
            FoodQuantityChanged?.Invoke(_amountOfResources.Food);
        }
    }

    public void AddTimber(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Timber += amount;
            TimberQuantityChanged?.Invoke(_amountOfResources.Timber);
        }
    }

    public void AddMarble(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Marble += amount;
            MarbleQuantityChanged?.Invoke(_amountOfResources.Marble);
        }
    }

    public void ReduceFood(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Food -= amount;
            FoodQuantityChanged?.Invoke(_amountOfResources.Food);
        }
    }

    public void ReduceTimber(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Timber -= amount;
            TimberQuantityChanged?.Invoke(_amountOfResources.Timber);
        }
    }

    public void ReduceMarble(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Marble -= amount;
            MarbleQuantityChanged?.Invoke(_amountOfResources.Marble);
        }
    }

    AmountOfResources IStore.GetAmountOfResources()
    {
        return _amountOfResources;
    }
}
