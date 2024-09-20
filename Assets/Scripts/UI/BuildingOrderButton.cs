using UnityEngine;

public class BuildingOrder : OrderButton
{
    [SerializeField] protected BuildingPlacer Placer;

    //public override void TryBuy()                       // Упразднить в кнопке по строительству +++++++++++++++++++++++++++++++++++
    //{
    //    AmountOfResources price = _currentBuilding.GetPriceOf(Prefab);

    //    if (CheckPriceAvailability(_currentBuilding.Store.GetAmountOfResources(), price))
    //    {
    //        Debug.Log("Денег на здание достаточно.");                                 // ++++++++++++++++++++
    //        _currentBuilding.SubtarctResources(price);
    //        Placer.CreateBuilding(Prefab.GetComponent<BuildingUnderConstruction>());
    //    }
    //    else
    //        Debug.Log("Денег на здание нет.");                                 // ++++++++++++++++++++
    //}

    // При нажатии на кнопку создать "летающее строение"
    public void CreateFlyingBuilding()
    {
        Placer.CreateFlyingBuilding(Prefab.GetComponent<BuildingUnderConstruction>());
    }
}
