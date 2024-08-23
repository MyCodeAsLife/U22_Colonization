using UnityEngine;

public class UnitOrder : OrderButton
{
    public override void TryBuy()
    {
        AmountOfResources price = _currentBuilding.GetPriceOf(Prefab);

        if (CheckPriceAvailability(_currentBuilding.Store.GetAmountOfResources(), price))
        {
            Debug.Log("����� �� ���� ����������.");                                 // ++++++++++++++++++++
            _currentBuilding.SubtarctResources(price);
            _currentBuilding.CreateCollectorBot();
        }
        else
            Debug.Log("����� �� ���� ���.");                                        // ++++++++++++++++
    }
}
