using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseAI : Building
{
    [SerializeField] private Transform _gatheringPoint;
    [SerializeField] private int _maxCountCollectorBots;                         // +++++

    private BuildingPanelUI _buildingPanelUI;                                                   //+++++
    private Transform _map;
    private Coroutine _resourceScaning;
    private ResourceScaner _resourceScaner;
    private CollectorBotAI _prefabCollectorBot;

    private IList<AmountOfResources> _priceList;
    private AmountOfResources _amountOfResources;
    //private int _numberOfFood;
    //private int _numberOfTimber;
    //private int _numberOfMarble;

    private IList<Resource> _freeResources;
    private IList<Resource> _resourcesPlannedForCollection;
    private IList<CollectorBotAI> _poolOfWorkingCollectorBots;
    private IList<CollectorBotAI> _poolOfIdleCollectorBots;

    public event Action<int> FoodQuantityChanged;
    public event Action<int> TimberQuantityChanged;
    public event Action<int> MarbleQuantityChanged;

    public AmountOfResources AmountOfResources { get { return _amountOfResources; } }
    //public int NumberOfFood { get { return _numberOfFood; } }
    //public int NumberOfTimber { get { return _numberOfTimber; } }
    //public int NumberOfMarble { get { return _numberOfMarble; } }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_resourceScaning != null)
            StopCoroutine(_resourceScaning);
    }

    protected override void Start()
    {
        base.Start();
        StartInicialization();
    }

    public void SubtarctResources(AmountOfResources amount)      // �������� �� ���-��, ����� �� ���� � �����
    {
        _amountOfResources.Food -= amount.Food;
        _amountOfResources.Timber -= amount.Timber;
        _amountOfResources.Marble -= amount.Marble;
    }

    public void StoreResource(ResourceType resourceType)
    {
        for (int i = 0; i < _resourcesPlannedForCollection.Count; ++i)
            if (_resourcesPlannedForCollection[i].ResourceType == resourceType)
            {
                _resourcesPlannedForCollection.RemoveAt(i);
                break;
            }

        switch (resourceType)
        {
            case ResourceType.Food:
                //_numberOfFood++;
                _amountOfResources.Food++;
                FoodQuantityChanged?.Invoke(_amountOfResources.Food);
                break;

            case ResourceType.Timber:
                //_numberOfTimber++;
                _amountOfResources.Timber++;
                TimberQuantityChanged?.Invoke(_amountOfResources.Timber);
                break;

            case ResourceType.Marble:
                //_numberOfMarble++;
                _amountOfResources.Marble++;
                MarbleQuantityChanged?.Invoke(_amountOfResources.Marble);
                break;
        }
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

    public AmountOfResources GetPriceOf(SelectableObject obj)
    {
        if (obj is MainBaseAI)
            return _priceList[0];
        else if (obj is Barack)
            return _priceList[1];
        else
            return _priceList[0];
    }

    //private bool Implements<T>(this object Object) => Object is T;                              // ��� Price �����

    private void StartInicialization()
    {
        _priceList = new List<AmountOfResources>();
        _buildingPanelUI = FindFirstObjectByType<BuildingPanelUI>();
        _map = GameObject.FindGameObjectWithTag("Map").transform;                               // Magical ???
        _selectionIndicator.localScale = Vector3.one * 0.5f;                                    // Magical ???
        _resourceScaner = new ResourceScaner(_map);
        _prefabCollectorBot = Resources.Load<CollectorBotAI>("Prefabs/CollectorBot");
        //_maxCountCollectorBots = 0;
        _freeResources = new List<Resource>();
        _resourcesPlannedForCollection = new List<Resource>();
        _poolOfWorkingCollectorBots = new List<CollectorBotAI>();
        _poolOfIdleCollectorBots = new List<CollectorBotAI>();

        StartPriceList();
        _resourceScaning = StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
    }

    private void StartPriceList()
    {
        AmountOfResources mainbase = new AmountOfResources();
        mainbase.Food = 1;
        mainbase.Timber = 1;
        mainbase.Marble = 1;
        _priceList.Add(mainbase);
        AmountOfResources baracks = new AmountOfResources();
        baracks.Food = 1;
        baracks.Timber = 1;
        baracks.Marble = 1;
        _priceList.Add(baracks);
        AmountOfResources bot = new AmountOfResources();
        bot.Food = 1;
        bot.Timber = 1;
        bot.Marble = 0;
        _priceList.Add(bot);
    }

    private void OnCollectorBotTaskCompleted(CollectorBotAI bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);

        //if (_poolOfIdleCollectorBots.Contains(bot) == false)
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void FindFreeResources()
    {
        IList<Resource> allResources = _resourceScaner.MapScaning();

        foreach (var resource in allResources)
            if (_freeResources.Contains(resource) == false)
                if (_resourcesPlannedForCollection.Contains(resource) == false)
                    _freeResources.Add(resource);
    }

    private void DistributeCollectionTasks()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_freeResources.Count < 1 || _poolOfIdleCollectorBots.Count < 1)
                break;

            _poolOfIdleCollectorBots[0].SetCollectionTask(_freeResources[0]);
            _resourcesPlannedForCollection.Add(_freeResources[0]);
            _freeResources.RemoveAt(0);
            _poolOfWorkingCollectorBots.Add(_poolOfIdleCollectorBots[0]);
            _poolOfIdleCollectorBots.RemoveAt(0);
        }
    }

    private void CreateCollectorBot()
    {
        var collectorBot = Instantiate<CollectorBotAI>(_prefabCollectorBot);
        collectorBot.TaskCompleted += OnCollectorBotTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.transform.SetParent(transform.parent);
        collectorBot.SetBaseAffiliation(this);
        collectorBot.GoTo(_gatheringPoint.position);
        _poolOfIdleCollectorBots.Add(collectorBot);
    }

    private IEnumerator ResourceScanning()
    {
        const float Delay = 2.0f;
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            FindFreeResources();

            if (_freeResources.Count > 0 && _poolOfIdleCollectorBots.Count > 0)
                DistributeCollectionTasks();
        }

        _resourceScaning = null;
    }

    private IEnumerator StartInitialization()
    {
        var delay = new WaitForSeconds(1f);

        for (int i = 0; i < _maxCountCollectorBots; i++)
        {
            yield return delay;
            CreateCollectorBot();
        }
    }
}