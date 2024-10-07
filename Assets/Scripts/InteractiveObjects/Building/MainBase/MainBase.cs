using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainBase : Building
{
    [SerializeField] private Transform _map;
    [SerializeField] private int _startingNumberOfBots;

    private Store _store = new();
    private DownPanelUI _buildingPanelUI;
    private GatheringPoint _gatheringPoint;
    private CollectorBot _prefabCollectorBot;
    private SingleReactiveProperty<int> _numberOfBots = new();
    private List<CollectorBot> _poolCollectorBots = new List<CollectorBot>();
    private Dictionary<Price, AmountOfResources> _priceList = new();

    public ITaskManager TaskManager { get; private set; }
    public int NumberOfBots => _numberOfBots.Value;
    public IStore Store => _store;

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

    public void SubscribeChangesNumberBots(Action<int> func)
    {
        _numberOfBots.Change += func;
    }

    public void UnSubscribeChangesNumberBots(Action<int> func)
    {
        _numberOfBots.Change -= func;
    }

    public AmountOfResources GetPriceOf(Price price)
    {
        return _priceList[price];
    }

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
        _buildingPanelUI = FindFirstObjectByType<DownPanelUI>();
        SelectionIndicator.localScale = Vector3.one * Half;
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        CreateStartingPriceList();
        StartCoroutine(StartInitialization());
    }

    private void CreateStartingPriceList()
    {
        AmountOfResources mainBasePrice = new AmountOfResources();
        mainBasePrice.Food = 5;
        mainBasePrice.Timber = 5;
        mainBasePrice.Marble = 5;
        _priceList.Add(Price.MainBase, mainBasePrice);
        AmountOfResources botPrice = new AmountOfResources();
        botPrice.Food = 3;
        botPrice.Timber = 3;
        botPrice.Marble = 3;
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