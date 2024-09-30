using System;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
    [SerializeField] private BuildingType _buildingType;

    private CollectorBot _transferBot;
    private Flag _flag;
    private float _startPositionY;
    private float _endPositionY;

    public bool IsBuildingInProgress { get; private set; }
    public Vector3Int RoundPosition { get; private set; }

    public event Action<BuildingUnderConstruction> BuildingStarted;

    protected override void Start()
    {
        const float Height = 4f;
        const float Scale = 0.05f;
        base.Start();
        _flag = GetComponentInChildren<Flag>();
        DurationOfAction = 5f;
        Vector3 progressBarPosition = new Vector3(0, Height, 0);
        Vector3 progressBarScale = new Vector3(Scale, Scale, Scale);
        ActionProgressViewer.SetProgressBarPosition(progressBarPosition);
        ActionProgressViewer.SetProgressBarScale(progressBarScale);
    }

    public void SetStartPosition(Vector3 position) => _startPositionY = position.y;
    public void SetEndPosition(Vector3 position) => _endPositionY = position.y;
    public void SetRoundPosition(Vector3Int roundPos) => RoundPosition = roundPos;
    public void SetTransferBot(CollectorBot bot) => _transferBot = bot;

    public void StartConstruction(ChangingObject builder)
    {
        BuildingStarted?.Invoke(this);
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
        Building building;
        StopConstruction(builder);

        switch (_buildingType)
        {
            case BuildingType.MainBase:
                building = transform.AddComponent<MainBase>();
                building.GetComponent<MainBase>().ReceiveCollectorBot(_transferBot);
                break;

            case BuildingType.Barack:
                building = transform.AddComponent<Barack>();
                break;
        }

        ActionFinish();
        Destroy(_flag.gameObject);
        Destroy(this);
    }

    private void Constructing(float num)
    {
        ActionProgress.Value = num;
        float newPosY = Mathf.Lerp(_startPositionY, _endPositionY, ActionProgress.Value);
        transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
    }
}
