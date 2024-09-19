using UnityEngine;

public class BuildingOrder : OrderButton
{
    public override void TryBuy()
    {
        AmountOfResources price = _currentBuilding.GetPriceOf(Prefab);

        if (CheckPriceAvailability(_currentBuilding.Store.GetAmountOfResources(), price))
        {
            Debug.Log("����� �� ������ ����������.");                                 // ++++++++++++++++++++
            _currentBuilding.SubtarctResources(price);
            Placer.CreateBuilding(Prefab.GetComponent<BuildingUnderConstruction>());
        }
        else
            Debug.Log("����� �� ������ ���.");                                 // ++++++++++++++++++++
    }
}
