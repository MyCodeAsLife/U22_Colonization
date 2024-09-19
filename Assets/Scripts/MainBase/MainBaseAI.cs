using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseAI : Building
{
    [SerializeField] private Transform _gatheringPoint;
    [SerializeField] private int _maxCountCollectorBots;                         // +++++

    private Transform _map;
    private Store _store = new();
    private Coroutine _resourceScaning;
    private ResourceScaner _resourceScaner;
    private DownPanelUI _buildingPanelUI;                                                   //+++++
    private CollectorBotAI _prefabCollectorBot;

    private List<Task> _taskPool = new();
    private List<Task> _issueTasks = new();
    private List<AmountOfResources> _priceList = new();
    private List<CollectorBotAI> _poolOfIdleCollectorBots = new();
    private List<CollectorBotAI> _poolOfWorkingCollectorBots = new();

    public IStore Store { get { return _store; } }

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

    public void SubtarctResources(AmountOfResources amount)                 // Перенести в Store?
    {
        _store.ReduceFood(amount.Food);
        _store.ReduceTimber(amount.Timber);
        _store.ReduceMarble(amount.Marble);
    }

    public void StoreResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                _store.AddFood(1);
                break;

            case ResourceType.Timber:
                _store.AddTimber(1);
                break;

            case ResourceType.Marble:
                _store.AddMarble(1);
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

    public AmountOfResources GetPriceOf(SelectableObject obj)                       // Вынести в отдельную сущность
    {
        if (obj is MainBaseAI)
            return _priceList[0];
        else if (obj is Barack)
            return _priceList[1];
        else
            return _priceList[2];
    }

    public void CreateCollectorBot()
    {
        var collectorBot = Instantiate<CollectorBotAI>(_prefabCollectorBot);
        collectorBot.TaskCompleted += OnCollectorBotTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.transform.SetParent(transform.parent);
        collectorBot.SetBaseAffiliation(this);
        collectorBot.GoTo(_gatheringPoint.position);
        _poolOfIdleCollectorBots.Add(collectorBot);
    }

    public void AddNewTask(Task newTask)
    {
        bool taskIsAdded = false;

        if (_taskPool.Count < 1)
        {
            _taskPool.Add(newTask);
            return;
        }

        for (int i = 0; i < _taskPool.Count; i++)
        {
            if (newTask.Priority < _taskPool[i].Priority)
            {
                _taskPool.Insert(i, newTask);
                taskIsAdded = true;
                break;
            }
        }

        if (taskIsAdded == false)
            _taskPool.Add(newTask);
    }

    private void TransferCollectorBot(Flag newBase, CollectorBotAI bot)                   // Передать бота
    {
        newBase.SetBot(bot);
        bot.TaskCompleted -= OnCollectorBotTaskCompleted;
        bot.SetBaseAffiliation(this);
        _poolOfIdleCollectorBots.Remove(bot);
        _poolOfWorkingCollectorBots.Remove(bot);
    }

    private void OnTaskCompleted(Task task)
    {
        if (_issueTasks.Contains(task))
        {
            _issueTasks.Remove(task);
            task.Completed -= OnTaskCompleted;
        }
    }

    private void StartInicialization()
    {
        _buildingPanelUI = FindFirstObjectByType<DownPanelUI>();
        _map = GameObject.FindGameObjectWithTag("Map").transform;                               // Magical ???
        _selectionIndicator.localScale = Vector3.one * 0.5f;                                    // Magical ???
        _resourceScaner = new ResourceScaner(_map);
        _prefabCollectorBot = Resources.Load<CollectorBotAI>("Prefabs/CollectorBot");
        StartPriceList();
        _resourceScaning = StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
    }

    private void StartPriceList()                                                       // Прайс лист вынести в отдельную сущность
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
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void FindFreeResources()                                // переделать для корректной работы с несколькими независимыми базами на карте
    {
        var allResources = _resourceScaner.MapScaning();

        for (int i = 0; i < allResources.Count; i++)
        {
            bool contains = false;

            for (int j = 0; j < _taskPool.Count; j++)
            {
                if (_taskPool[j].Target is IResource)
                {
                    if ((_taskPool[j].Target as IResource).GetId() == allResources[i].GetId())
                    {
                        contains = true;
                        break;
                    }
                }
            }

            if (contains == false)
            {
                for (int j = 0; j < _issueTasks.Count; j++)
                {
                    if (_issueTasks[j].Target is IResource)
                    {
                        if ((_issueTasks[j].Target as IResource).GetId() == allResources[i].GetId())
                        {
                            contains = true;
                            break;
                        }
                    }
                }

                if (contains)
                    continue;

                Task task = new Task(1, TypesOfTasks.Harvesting, allResources[i] as Resource);               // Magic
                task.Completed += OnTaskCompleted;
                AddNewTask(task);
            }
        }
    }

    private void DistributeTasks()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_taskPool.Count < 1 || _poolOfIdleCollectorBots.Count < 1)
                break;

            _poolOfIdleCollectorBots[0].SetTask(GetTask());
            _poolOfWorkingCollectorBots.Add(_poolOfIdleCollectorBots[0]);
            _poolOfIdleCollectorBots.RemoveAt(0);
        }
    }

    private Task GetTask()
    {
        Task task = _taskPool[0];
        _issueTasks.Add(task);
        _taskPool.RemoveAt(0);
        return task;
    }

    private IEnumerator ResourceScanning()
    {
        const float Delay = 2.0f;
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            FindFreeResources();
            DistributeTasks();
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