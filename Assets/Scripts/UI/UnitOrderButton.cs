using UnityEngine;

public class UnitOrder : OrderButton
{
    public override void TryBuy()                                                   // Описать что делать при заказе юнита
    {
        AmountOfResources price = _currentBuilding.GetPriceOf(Prefab);

        if (CheckPriceAvailability(_currentBuilding.Store.GetAmountOfResources(), price))
        {
            Debug.Log("Денег на юнит достаточно.");                                 // ++++++++++++++++++++
            _currentBuilding.SubtarctResources(price);
            //Placer.CreateBuilding(Prefab.GetComponent<Building>());
        }
        else
            Debug.Log("Денег на юнит нет.");                                        // ++++++++++++++++
    }
}
