using UnityEngine;

public class BuildingOrder : OrderButton
{
    [SerializeField] private BuildingPlacer _placer;
    [SerializeField] private BuildingUnderConstruction _prefab;

    public void CreateFlyingBuilding()
    {
        _placer.CreateFlyingBuilding(_prefab);
    }
}
