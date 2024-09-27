using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainBaseAI : Building
{
    [SerializeField] private int _startCountCollectorBots;                         // Переименовать

    private Transform _map;
    private Store _store = new();
    private DownPanelUI _buildingPanelUI;
    private GatheringPoint _gatheringPoint;
    private CollectorBotAI _prefabCollectorBot;
    private SingleReactiveProperty<int> _numberOfBots = new();
    private List<CollectorBotAI> _poolCollectorBots = new List<CollectorBotAI>();
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
    public AmountOfResources GetPriceOf(Price price) => _priceList[price];       // Вынести в отдельную сущность

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
        var collectorBot = Instantiate<CollectorBotAI>(_prefabCollectorBot);
        TaskManager.AddCollectorBot(collectorBot);
        collectorBot.GoTo(_gatheringPoint.transform.position);
        _numberOfBots.Value++;
    }

    public void ReceiveCollectorBot(CollectorBotAI bot)
    {
        TaskManager.AddCollectorBot(bot);
        _poolCollectorBots.Add(bot);
        _numberOfBots.Value++;
    }

    public void TransferCollectorBot(BuildingUnderConstruction newBase, CollectorBotAI bot)
    {
        TaskManager.RemoveCollectorBot(bot);
        _poolCollectorBots.Remove(bot);
        newBase.SetTransferBot(bot);
        _numberOfBots.Value--;
    }

    private void StartInicialization()
    {
        _buildingPanelUI = FindFirstObjectByType<DownPanelUI>();
        _map = GameObject.FindGameObjectWithTag("Map").transform;                               // Magical ???
        _selectionIndicator.localScale = Vector3.one * 0.5f;                                    // Magical ???
        TaskManager.AddResourceScaner(new ResourceScaner(_map));
        _prefabCollectorBot = Resources.Load<CollectorBotAI>("Prefabs/CollectorBot");
        CreateStartingPriceList();
        StartCoroutine(StartInitialization());
    }

    private void CreateStartingPriceList()                                                       // Прайс лист вынести в отдельную сущность
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

        for (int i = 0; i < _startCountCollectorBots; i++)
        {
            yield return delay;
            CreateCollectorBot();
        }
    }
}