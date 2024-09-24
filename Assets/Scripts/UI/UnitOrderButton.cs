using UnityEngine;

public class UnitOrder : OrderButton
{
    public void TryBuy()
    {
        AmountOfResources price = _currentBuilding.GetPriceOf(objectPrice);

        if (CheckPriceAvailability(_currentBuilding.Store.AmountOfResources, price))
        {
            Debug.Log("Денег на юнит достаточно.");                                 // ++++++++++++++++++++
            _currentBuilding.Store.SubtractResources(price);
            _currentBuilding.CreateCollectorBot();
        }
        else
            Debug.Log("Денег на юнит нет.");                                        // ++++++++++++++++
    }
}
