using UnityEngine;

public class UnitOrder : OrderButton
{
    public void TryBuy()
    {
        AmountOfResources price = _currentBuilding.GetPriceOf(objectPrice);

        if (CheckPriceAvailability(_currentBuilding.Store.GetAmountOfResources(), price))
        {
            Debug.Log("����� �� ���� ����������.");                                 // ++++++++++++++++++++
            _currentBuilding.SubtractResources(price);
            _currentBuilding.CreateCollectorBot();
        }
        else
            Debug.Log("����� �� ���� ���.");                                        // ++++++++++++++++
    }
}
