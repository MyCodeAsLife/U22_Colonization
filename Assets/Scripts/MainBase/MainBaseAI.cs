using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseAI : Building
{
    [SerializeField] private int _maxCountCollectorBots;                         // +++++

    private Transform _map;
    private Store _store = new();
    private Coroutine _resourceScaning;
    private DownPanelUI _buildingPanelUI;                                                   //+++++
    private GatheringPoint _gatheringPoint;
    private ResourceScaner _resourceScaner;
    private CollectorBotAI _prefabCollectorBot;
    private BuildingUnderConstruction _scheduleBuilding;
    private SingleReactiveProperty<int> _numberOfBots = new();

    private IList<IResource> _resourcePool;
    private List<Task> _taskPool = new();
    //private List<Task> _issueTasks = new();
    private Dictionary<Price, AmountOfResources> _priceList = new();
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
        _gatheringPoint = GetComponentInChildren<GatheringPoint>();
        StartInicialization();
    }

    public void SubtractResources(AmountOfResources amount)                 // Перенести в Store?
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

    public AmountOfResources GetPriceOf(Price price)                       // Вынести в отдельную сущность
    {
        return _priceList[price];
    }

    public void CreateCollectorBot()
    {
        var collectorBot = Instantiate<CollectorBotAI>(_prefabCollectorBot);
        collectorBot.TaskCompleted += OnCollectorBotTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.transform.SetParent(transform.parent);
        collectorBot.SetBaseAffiliation(this);
        collectorBot.GoTo(_gatheringPoint.transform.position);
        _poolOfIdleCollectorBots.Add(collectorBot);
        _numberOfBots.Value++;
    }

    public void AddNewTask(Task newTask)
    {
        bool taskIsAdded = false;
        //newTask.Completed += OnTaskCompleted;

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

    public void ScheduleConstruction(BuildingUnderConstruction building)
    {
        if (_scheduleBuilding == null)
        {
            _scheduleBuilding = building;
        }
        else if (_scheduleBuilding.IsBuildingInProgress)
        {
            Destroy(building.gameObject);
        }
        else
        {
            _scheduleBuilding.transform.position = building.transform.position;
            Destroy(building.gameObject);
        }

        if (CheckingForConditionsForConstruction() == false)
        {
            _numberOfBots.Change += WaitingForConditionsForConstruction;
            _store.FoodQuantityChanged += WaitingForConditionsForConstruction;
            _store.TimberQuantityChanged += WaitingForConditionsForConstruction;
            _store.MarbleQuantityChanged += WaitingForConditionsForConstruction;
        }
    }

    // Функция ожидания выполнения условий
    private void WaitingForConditionsForConstruction(int number)            // Перенести в CheckingForConditionsForConstruction ??
    {
        if (CheckingForConditionsForConstruction())
        {
            _numberOfBots.Change -= WaitingForConditionsForConstruction;
            _store.FoodQuantityChanged -= WaitingForConditionsForConstruction;
            _store.TimberQuantityChanged -= WaitingForConditionsForConstruction;
            _store.MarbleQuantityChanged -= WaitingForConditionsForConstruction;
        }
    }

    // Функция проверки необходимых условий
    private bool CheckingForConditionsForConstruction()
    {
        var amountOfResources = _store.AmountOfResources;               // Нужно кэшировать

        if (amountOfResources.Food >= _priceList[Price.MainBase].Food &&
            amountOfResources.Timber >= _priceList[Price.MainBase].Timber &&
            amountOfResources.Marble >= _priceList[Price.MainBase].Marble &&
            _numberOfBots.Value > 0)                                                        // Magic
        {
            AddNewTask(new Task(0, TypesOfTasks.Constructing, _scheduleBuilding));                       // Magic
            _scheduleBuilding = null;
            SubtractResources(_priceList[Price.MainBase]);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetTransferBot(CollectorBotAI bot)
    {
        //Debug.Log("Count bot before settransfer: " + _numberOfBots.Value);                     //+++++++++++++++++++++++++++
        bot.TaskCompleted += OnCollectorBotTaskCompleted;
        bot.SetBaseAffiliation(this);
        _poolOfIdleCollectorBots.Add(bot);
        _numberOfBots.Value++;
        //Debug.Log("Count bot after settransfer: " + _numberOfBots.Value);                     //+++++++++++++++++++++++++++
        //Debug.Log("_gatheringPoint " + _gatheringPoint == null);                            //+++++++++++++++++++++++++++
        //Debug.Log(" bot " + bot == null);                            //+++++++++++++++++++++++++++
        //bot.GoTo(_gatheringPoint.transform.position);
    }

    private void TransferCollectorBot(BuildingUnderConstruction newBase, CollectorBotAI bot)                   // Передать бота
    {
        _poolOfIdleCollectorBots.Remove(bot);
        _poolOfWorkingCollectorBots.Remove(bot);
        _numberOfBots.Value--;
        newBase.SetTransferBot(bot);
        bot.TaskCompleted -= OnCollectorBotTaskCompleted;
        //bot.SetBaseAffiliation(newBase);
    }

    //private void OnTaskCompleted(Task task)
    //{
    //    if (_issueTasks.Contains(task))
    //    {
    //        _issueTasks.Remove(task);
    //        //task.Completed -= OnTaskCompleted;
    //    }
    //}

    private void OnCollectorBotTaskCompleted(CollectorBotAI bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void StartInicialization()
    {
        _buildingPanelUI = FindFirstObjectByType<DownPanelUI>();
        _map = GameObject.FindGameObjectWithTag("Map").transform;                               // Magical ???
        _selectionIndicator.localScale = Vector3.one * 0.5f;                                    // Magical ???
        _resourceScaner = new ResourceScaner(_map);
        _prefabCollectorBot = Resources.Load<CollectorBotAI>("Prefabs/CollectorBot");
        CreateStartingPriceList();
        _resourceScaning = StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
    }

    private void CreateStartingPriceList()                                                       // Прайс лист вынести в отдельную сущность
    {
        AmountOfResources mainBasePrice = new AmountOfResources();
        mainBasePrice.Food = 3;
        mainBasePrice.Timber = 3;
        mainBasePrice.Marble = 3;
        _priceList.Add(Price.MainBase, mainBasePrice);
        AmountOfResources botPrice = new AmountOfResources();
        botPrice.Food = 1;
        botPrice.Timber = 1;
        botPrice.Marble = 0;
        _priceList.Add(Price.CollectorBot, botPrice);
    }

    //private void FindResources()                                // переделать для корректной работы с несколькими независимыми базами на карте
    //{
    //    var allResources = _resourceScaner.MapScaning();

    //    for (int i = 0; i < allResources.Count; i++)
    //    {
    //        bool contains = false;

    //        for (int j = 0; j < _taskPool.Count; j++)
    //        {
    //            if (_taskPool[j].Target is IResource)
    //            {
    //                if ((_taskPool[j].Target as IResource).GetId() == allResources[i].GetId())
    //                {
    //                    contains = true;
    //                    break;
    //                }
    //            }
    //        }

    //        if (contains == false)
    //        {
    //            for (int j = 0; j < _issueTasks.Count; j++)
    //            {
    //                if (_issueTasks[j].Target is IResource)
    //                {
    //                    if ((_issueTasks[j].Target as IResource).GetId() == allResources[i].GetId())
    //                    {
    //                        contains = true;
    //                        break;
    //                    }
    //                }
    //            }

    //            if (contains)
    //                continue;

    //            //Task task = new Task(1, TypesOfTasks.Harvesting, allResources[i] as Resource);               // Magic
    //            //task.Completed += OnTaskCompleted;                                                          // Дублирование

    //            if (allResources[i].IsOccupied == false)
    //                AddNewTask(new Task(1, TypesOfTasks.Harvesting, allResources[i] as Resource));               // Magic
    //        }
    //    }
    //}

    //private Task GetTask()
    //{
    //    Task task = _taskPool[0];
    //    _issueTasks.Add(task);
    //    _taskPool.RemoveAt(0);
    //    return task;
    //}

    private Task GetHarvestTask()
    {
        for (int i = 0; i < _resourcePool.Count; i++)
        {
            if (_resourcePool[i].IsOccupied == false && (_resourcePool[i] as Resource).gameObject.activeSelf)
            {
                _resourcePool[i].Occupy();
                return new Task(1, TypesOfTasks.Harvesting, _resourcePool[i] as Resource);               // Magic
            }
        }

        return null;
    }

    private void DistributeTasks()
    {
        bool isWork = true;

        while (isWork)
        {
            //if (_taskPool.Count < 1 || _poolOfIdleCollectorBots.Count < 1)
            //    break;

            //var task = GetTask();

            //if (task.TypeOfTask == TypesOfTasks.Harvesting)
            //{
            //    if ((task.Target as IResource).IsOccupied == false)
            //    {
            //        (task.Target as IResource).Occupy();
            //    }
            //    else
            //    {
            //        OnTaskCompleted(task);
            //        continue;
            //    }
            //}

            if (_poolOfIdleCollectorBots.Count < 1 || (_taskPool.Count < 1 && _resourcePool.Count < 1))
                break;

            Task task = null;

            if (_taskPool.Count != 0)
            {
                task = _taskPool[0];
                _taskPool.RemoveAt(0);
            }
            else
            {
                task = GetHarvestTask();
            }

            if (task != null /*&& task.Target.gameObject.activeSelf*/)                      // Теперь ошибка здесь...
            {
                Debug.Log(task.Target.name);                                            //++++++++++++++++++++++++++++++++++++++

                _poolOfIdleCollectorBots[0].SetTask(task);
                // Если задача на строительство базы, то передать ей свободного бота
                if (task.TypeOfTask == TypesOfTasks.Constructing)
                {
                    TransferCollectorBot(task.Target as BuildingUnderConstruction, _poolOfIdleCollectorBots[0]);
                }
                else
                {
                    //_poolOfIdleCollectorBots[0].SetTask(GetTask());
                    _poolOfWorkingCollectorBots.Add(_poolOfIdleCollectorBots[0]);
                    _poolOfIdleCollectorBots.RemoveAt(0);
                }
            }
        }
    }

    private IEnumerator ResourceScanning()
    {
        const float Delay = 2.0f;
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            //FindResources();
            _resourcePool = _resourceScaner.MapScaning();
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