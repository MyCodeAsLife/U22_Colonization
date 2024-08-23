using Unity.VisualScripting;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
    [SerializeField] private BuildingType _buildingType;

    public void CompleteConstruction()
    {
        Building building;

        if (_buildingType == BuildingType.MainBase)
            building = transform.AddComponent<MainBaseAI>();
        else if (_buildingType == BuildingType.Barack)
            building = transform.AddComponent<Barack>();

        // Вызвать активацию

        // Удалить этот компонент
        Destroy(this);
    }
}
