using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private IStore _store;
    private MainBaseAI _mainBase;
    private Coroutine _resourceScaning;
    private ResourceScaner _resourceScaner;
    private BuildingUnderConstruction _scheduleBuilding;

    private List<Task> _taskPool = new();
    //private List<Task> _issueTasks = new();
    //private Dictionary<Price, AmountOfResources> _priceList = new();              // Нужна здесь или запрашивать из MainBase ??
    private List<IResource> _resourcePool = new();
    private List<CollectorBotAI> _poolOfIdleCollectorBots = new();                // Создать главный пулл, и при старте этот пулл синхронизировать с главным
    private List<CollectorBotAI> _poolOfWorkingCollectorBots = new();             // При передачи ботов, удалять их отсюда и из главного

    protected void OnDisable()
    {
        if (_resourceScaning != null)
            StopCoroutine(_resourceScaning);
    }

    private void Start()
    {
        _mainBase = GetComponent<MainBaseAI>();
    }

    public void SetStore(IStore store)
    {
        _store = store;
    }

    public void AddResourceScaner(ResourceScaner resourceScaner)
    {
        _resourceScaner = resourceScaner;
        _resourceScaning = StartCoroutine(ResourceScanning());
    }

    public void AddCollectorBot(CollectorBotAI collectorBot)
    {
        collectorBot.TaskCompleted += OnCollectorBotTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.transform.SetParent(transform.parent);
        collectorBot.SetBaseAffiliation(_mainBase);
        //collectorBot.GoTo(_gatheringPoint.transform.position);
        _poolOfIdleCollectorBots.Add(collectorBot);
        //_numberOfBots.Value++;
    }

    public void RemoveCollectorBot(CollectorBotAI collectorBot)
    {
        _poolOfIdleCollectorBots.Remove(collectorBot);
        _poolOfWorkingCollectorBots.Remove(collectorBot);
        //_numberOfBots.Value--;
        //newBase.SetTransferBot(bot);
        collectorBot.TaskCompleted -= OnCollectorBotTaskCompleted;
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
            //_numberOfBots.Change += WaitingForConditionsForConstruction;
            _mainBase.SubscribeChangesNumberBots(WaitingForConditionsForConstruction);
            _store.FoodQuantityChanged += WaitingForConditionsForConstruction;
            _store.TimberQuantityChanged += WaitingForConditionsForConstruction;
            _store.MarbleQuantityChanged += WaitingForConditionsForConstruction;
        }
    }

    private void WaitingForConditionsForConstruction(int number)            // Перенести в CheckingForConditionsForConstruction ??
    {
        if (CheckingForConditionsForConstruction())
        {
            //_numberOfBots.Change -= WaitingForConditionsForConstruction;
            _mainBase.UnSubscribeChangesNumberBots(WaitingForConditionsForConstruction);
            _store.FoodQuantityChanged -= WaitingForConditionsForConstruction;
            _store.TimberQuantityChanged -= WaitingForConditionsForConstruction;
            _store.MarbleQuantityChanged -= WaitingForConditionsForConstruction;
        }
    }

    private bool CheckingForConditionsForConstruction()
    {
        var amountOfResources = _store.AmountOfResources;
        var price = _mainBase.GetPriceOf(Price.MainBase);

        if (amountOfResources.Food >= price.Food &&
            amountOfResources.Timber >= price.Timber &&
            amountOfResources.Marble >= price.Marble &&
            _mainBase.NumberOfBots > 0)                                                               // Magic
        {
            AddTask(new Task(0, TypesOfTasks.Constructing, _scheduleBuilding));                    // Magic
            _scheduleBuilding = null;
            _mainBase.Store.SubtractResources(price);
            return true;
        }
        else
        {
            return false;
        }
    }

    //private void OnTaskCompleted(Task task)
    //{
    //    if (_issueTasks.Contains(task))
    //    {
    //        _issueTasks.Remove(task);
    //        task.Completed -= OnTaskCompleted;
    //    }
    //}

    //private void OnCollectorBotTaskCompleted(CollectorBotAI bot)    // Объеденитьб с OnTaskCompleted
    //{
    //    _poolOfWorkingCollectorBots.Remove(bot);
    //    _poolOfIdleCollectorBots.Add(bot);
    //}

    private bool TryGetHarvestTask(out Task task)
    {
        //_resourcePool = _resourceScaner.MapScaning();
        task = null;

        for (int i = 0; i < _resourcePool.Count; i++)
        {
            //bool contains = false;

            //for (int j = 0; j < _taskPool.Count; j++)
            //{
            //    if (_taskPool[j].Target is IResource)
            //    {
            //        if ((_taskPool[j].Target as IResource).GetId() == _resourcePool[i].GetId())             // ID - упразднить?
            //        {
            //            contains = true;
            //            break;
            //        }
            //    }
            //}

            //if (contains == false)
            //{
            //    for (int j = 0; j < _issueTasks.Count; j++)
            //    {
            //        if (_issueTasks[j].Target is IResource)
            //        {
            //            if ((_issueTasks[j].Target as IResource).GetId() == allResources[i].GetId())
            //            {
            //                contains = true;
            //                break;
            //            }
            //        }
            //    }

            //    if (contains)
            //        continue;

            //    //Task task = new Task(1, TypesOfTasks.Harvesting, allResources[i] as Resource);               // Magic
            //    //task.Completed += OnTaskCompleted;                                                          // Дублирование

            //    if (allResources[i].IsOccupied == false)
            //        AddTask(new Task(1, TypesOfTasks.Harvesting, allResources[i] as Resource));               // Magic
            //}

            if (_resourcePool[i].IsOccupied == false)
            {
                _resourcePool[i].Occupy();
                task = new Task(1, TypesOfTasks.Harvesting, _resourcePool[i] as Resource);               // Magic
                task.Completed += OnTaskCompleted;
                return true;
            }
        }

        return false;
    }

    private void AddTask(Task newTask)
    {
        bool taskIsAdded = false;
        newTask.Completed += OnTaskCompleted;

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

    //private Task GetTask()
    //{
    //    Task task = _taskPool[0];
    //    _issueTasks.Add(task);
    //    _taskPool.RemoveAt(0);
    //    return task;
    //}

    private void DistributeTasks() // Проверить наличие задач в пуле и свободных ботов, если задач нет то сформировать задачу на сбор и выдать боту
    {
        bool isWork = true;

        while (isWork)
        {
            //if (_taskPool.Count < 1 || _poolOfIdleCollectorBots.Count < 1)
            if (_poolOfIdleCollectorBots.Count < 1)
                break;

            var task = GetTask();

            if (task.TypeOfTask == TypesOfTasks.Harvesting)
            {
                if ((task.Target as IResource).IsOccupied == false)
                {
                    (task.Target as IResource).Occupy();
                }
                else
                {
                    OnTaskCompleted(task);
                    continue;
                }
            }

            _poolOfIdleCollectorBots[0].SetTask(task);
            // Если задача на строительство базы, то передать ей свободного бота
            if (task.TypeOfTask == TypesOfTasks.Constructing)
            {
                _mainBase.TransferCollectorBot(task.Target as BuildingUnderConstruction, _poolOfIdleCollectorBots[0]);
            }
            else
            {
                //_poolOfIdleCollectorBots[0].SetTask(GetTask());
                _poolOfWorkingCollectorBots.Add(_poolOfIdleCollectorBots[0]);
                _poolOfIdleCollectorBots.RemoveAt(0);
            }
        }
    }

    //private void FindFreeResources()
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
    //                AddTask(new Task(1, TypesOfTasks.Harvesting, allResources[i] as Resource));               // Magic
    //        }
    //    }
    //}

    private IEnumerator ResourceScanning()
    {
        const float Delay = 2.0f;
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            //FindFreeResources();
            _resourcePool = _resourceScaner.MapScaning();
            DistributeTasks();
        }

        _resourceScaning = null;
    }
}
