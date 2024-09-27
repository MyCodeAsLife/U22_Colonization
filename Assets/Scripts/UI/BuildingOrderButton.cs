using UnityEngine;

public class BuildingOrder : OrderButton
{
    [SerializeField] protected BuildingPlacer Placer;
    [SerializeField] private BuildingUnderConstruction _prefab;

    public void CreateFlyingBuilding()
    {
        Placer.CreateFlyingBuilding(_prefab);
    }
}
