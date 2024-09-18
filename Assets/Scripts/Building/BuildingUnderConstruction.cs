using Unity.VisualScripting;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
    [SerializeField] private BuildingType _buildingType;

    protected override void Start()
    {
        base.Start();
        DurationOfAction = 5f;
    }

    public void StartConstruction(ChangingObject builder)
    {
        builder.SubscribeToActionProgress(Constructing);
    }

    public void StopConstruction(ChangingObject builder)
    {
        builder.UnsubscribeToActionProgress(Constructing);
    }

    public void CompleteConstruction()
    {
        Building building;                                  // Нужно ли кэширование ??

        switch (_buildingType)
        {
            case BuildingType.MainBase:
                building = transform.AddComponent<MainBaseAI>();
                break;

            case BuildingType.Barack:
                building = transform.AddComponent<Barack>();
                break;
        }

        // Вызвать активацию строения

        Destroy(this);
    }

    private void Constructing(float num)
    {
        ActionProgress.Value = num;

        // Подъем здания из-под земли
        float newPosY = transform.position.y + transform.localScale.y * num;
        transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
    }
}
