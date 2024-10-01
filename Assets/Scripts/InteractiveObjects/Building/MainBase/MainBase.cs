using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainBase : Building
{
    [SerializeField] private int _startingNumberOfBots;

    private Transform _map;
    private Store _store = new();
    private DownPanelUI _buildingPanelUI;
    private GatheringPoint _gatheringPoint;
    private CollectorBot _prefabCollectorBot;
    private SingleReactiveProperty<int> _numberOfBots = new();
    private List<CollectorBot> _poolCollectorBots = new List<CollectorBot>();
    private Dictionary<Price, AmountOfResources> _priceList = new();

    public IStore Store { get { return _store; } }
    public TaskManager TaskManager { get; private set; }
    public int NumberOfBots => _numberOfBots.Value;

    protected override void Awake()
    {
        base.Awake();
        TaskManager = transform.AddComponent<TaskManager>();
        _gatheringPoint = GetComponentInChildren<GatheringPoint>();
    }

    protected override void Start()
    {
        base.Start();
        StartInicialization();
    }

    public void SubscribeChangesNumberBots(Action<int> func) => _numberOfBots.Change += func;
    public void UnSubscribeChangesNumberBots(Action<int> func) => _numberOfBots.Change -= func;
    public AmountOfResources GetPriceOf(Price price) => _priceList[price];

    public override void Selected()
    {
        base.Selected();
        _buildingPanelUI.LinkBase(this);
    }

    public override void UnSelect()
    {
        base.UnSelect();
        _buildingPanelUI.UnLinkBase(this);
    }

    public void CreateCollectorBot()
    {
        var collectorBot = Instantiate<CollectorBot>(_prefabCollectorBot);
        TaskManager.AddCollectorBot(collectorBot);
        collectorBot.GoTo(_gatheringPoint.transform.position);
        _numberOfBots.Value++;
    }

    public void ReceiveCollectorBot(CollectorBot bot)
    {
        TaskManager.AddCollectorBot(bot);
        _poolCollectorBots.Add(bot);
        _numberOfBots.Value++;
    }

    public void TransferCollectorBot(BuildingUnderConstruction newBase, CollectorBot bot)
    {
        TaskManager.RemoveCollectorBot(bot);
        _poolCollectorBots.Remove(bot);
        newBase.SetTransferBot(bot);
        _numberOfBots.Value--;
    }

    private void StartInicialization()
    {
        const float Half = 0.5f;
        const string MapTag = "Map";
        _buildingPanelUI = FindFirstObjectByType<DownPanelUI>();
        _map = GameObject.FindGameObjectWithTag(MapTag).transform;
        _selectionIndicator.localScale = Vector3.one * Half;
        TaskManager.AddResourceScaner(new ResourceScaner(_map));
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        CreateStartingPriceList();
        StartCoroutine(StartInitialization());
    }

    private void CreateStartingPriceList()
    {
        AmountOfResources mainBasePrice = new AmountOfResources();
        mainBasePrice.Food = 2;
        mainBasePrice.Timber = 2;
        mainBasePrice.Marble = 2;
        _priceList.Add(Price.MainBase, mainBasePrice);
        AmountOfResources botPrice = new AmountOfResources();
        botPrice.Food = 1;
        botPrice.Timber = 1;
        botPrice.Marble = 0;
        _priceList.Add(Price.CollectorBot, botPrice);
    }

    private IEnumerator StartInitialization()
    {
        var delay = new WaitForSeconds(1f);

        for (int i = 0; i < _startingNumberOfBots; i++)
        {
            yield return delay;
            CreateCollectorBot();
        }
    }
}