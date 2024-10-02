using System;

public class Store : IStore
{
    private AmountOfResources _amountOfResources = new();

    public event Action ResourcesQuantityChanged;

    public Store(int amountFood = 0, int amountTimber = 0, int amountMarble = 0)
    {
        _amountOfResources.Food = 0;
        _amountOfResources.Timber = 0;
        _amountOfResources.Marble = 0;
    }

    public AmountOfResources AmountOfResources { get { return _amountOfResources; } }

    public void StoreResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                AddFood(1);
                break;

            case ResourceType.Timber:
                AddTimber(1);
                break;

            case ResourceType.Marble:
                AddMarble(1);
                break;
        }
    }

    public void SubtractResources(AmountOfResources amount)
    {
        ReduceFood(amount.Food);
        ReduceTimber(amount.Timber);
        ReduceMarble(amount.Marble);

        ResourcesQuantityChanged?.Invoke();
    }

    private void AddFood(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Food += amount;
            ResourcesQuantityChanged?.Invoke();
        }
    }

    private void AddTimber(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Timber += amount;
            ResourcesQuantityChanged?.Invoke();
        }
    }

    private void AddMarble(int amount)
    {
        if (amount > 0)
        {
            _amountOfResources.Marble += amount;
            ResourcesQuantityChanged?.Invoke();
        }
    }

    private void ReduceFood(int amount)
    {
        if (amount > 0)
            _amountOfResources.Food -= amount;
    }

    private void ReduceTimber(int amount)
    {
        if (amount > 0)
            _amountOfResources.Timber -= amount;
    }

    private void ReduceMarble(int amount)
    {
        if (amount > 0)
            _amountOfResources.Marble -= amount;
    }
}
