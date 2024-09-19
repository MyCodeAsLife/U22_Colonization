using Unity.VisualScripting;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
    [SerializeField] private BuildingType _buildingType;
    private float _startPositionY;
    private float _endPositionY;

    protected override void Start()
    {
        base.Start();
        DurationOfAction = 5f;
        Vector3 progressBarPosition = new Vector3(0, 4, 0);                         //Magic ??
        Vector3 progressBarScale = new Vector3(0.05f, 0.05f, 0.05f);                //Magic ??
        ActionProgressViewer.SetProgressBarPosition(progressBarPosition);
        ActionProgressViewer.SetProgressBarScale(progressBarScale);
    }

    public void SetStartPosition(Vector3 position) => _startPositionY = position.y;
    public void SetEndPosition(Vector3 position) => _endPositionY = position.y;

    public void StartConstruction(ChangingObject builder)
    {
        builder.SubscribeToActionProgress(Constructing);
        ActionStart();
    }
    public void StopConstruction(ChangingObject builder)
    {
        builder.UnsubscribeToActionProgress(Constructing);
        ActionFinish();
    }

    public void CompleteConstruction(ChangingObject builder)
    {
        Building building;                                  // Нужно ли кэширование ??
        StopConstruction(builder);

        switch (_buildingType)
        {
            case BuildingType.MainBase:
                building = transform.AddComponent<MainBaseAI>();
                break;

            case BuildingType.Barack:
                building = transform.AddComponent<Barack>();
                break;
        }

        ActionFinish();
        // Вызвать активацию строения

        Destroy(this);
    }

    private void Constructing(float num)
    {
        ActionProgress.Value = num;
        float newPosY = Mathf.Lerp(_startPositionY, _endPositionY, ActionProgress.Value);
        transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
    }
}
