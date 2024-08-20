using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseAI : Building
{
    [SerializeField] private Transform _gatheringPoint;
    private BuildingPanelUI _buildingPanelUI;                                                   //+++++

    private Transform _map;
    private Coroutine _resourceScaning;
    private ResourceScaner _resourceScaner;
    private CollectorBotAI _prefabCollectorBot;

    private int _numberOfFood;
    private int _numberOfTimber;
    private int _numberOfMarble;
    [SerializeField] private int _maxCountCollectorBots;                         // +++++

    private IList<Resource> _freeResources;
    private IList<Resource> _resourcesPlannedForCollection;
    private IList<CollectorBotAI> _poolOfWorkingCollectorBots;
    private IList<CollectorBotAI> _poolOfIdleCollectorBots;

    public event Action<int> FoodQuantityChanged;
    public event Action<int> TimberQuantityChanged;
    public event Action<int> MarbleQuantityChanged;

    public int NumberOfFood { get { return _numberOfFood; } }
    public int NumberOfTimber { get { return _numberOfTimber; } }
    public int NumberOfMarble { get { return _numberOfMarble; } }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_resourceScaning != null)
            StopCoroutine(_resourceScaning);
    }

    protected override void Start()
    {
        base.Start();
        _buildingPanelUI = FindFirstObjectByType<BuildingPanelUI>();
        _map = GameObject.FindGameObjectWithTag("Map").transform;                                // Magical ???
        _selectionIndicator.localScale = Vector3.one * 0.5f;                                // Magical ???
        _resourceScaner = new ResourceScaner(_map);
        _prefabCollectorBot = Resources.Load<CollectorBotAI>("Prefabs/CollectorBot");
        //_maxCountCollectorBots = 0;
        _freeResources = new List<Resource>();
        _resourcesPlannedForCollection = new List<Resource>();
        _poolOfWorkingCollectorBots = new List<CollectorBotAI>();
        _poolOfIdleCollectorBots = new List<CollectorBotAI>();

        _resourceScaning = StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
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
                _numberOfFood++;
                FoodQuantityChanged?.Invoke(_numberOfFood);
                break;

            case ResourceType.Timber:
                _numberOfTimber++;
                TimberQuantityChanged?.Invoke(_numberOfTimber);
                break;

            case ResourceType.Marble:
                _numberOfMarble++;
                MarbleQuantityChanged?.Invoke(_numberOfMarble);
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