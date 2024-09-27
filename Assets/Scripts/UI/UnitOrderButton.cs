public class UnitOrder : OrderButton
{
    public void TryBuy()
    {
        AmountOfResources price = _currentBuilding.GetPriceOf(objectPrice);

        if (CheckPriceAvailability(_currentBuilding.Store.AmountOfResources, price))
        {
            _currentBuilding.Store.SubtractResources(price);
            _currentBuilding.CreateCollectorBot();
        }
    }
}
