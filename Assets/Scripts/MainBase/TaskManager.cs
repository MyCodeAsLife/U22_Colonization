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
    private List<IResource> _resourcePool = new();
    private List<CollectorBotAI> _poolOfIdleCollectorBots = new();                // Создать главный пулл, и при старте этот пулл синхронизировать с главным
    private List<CollectorBotAI> _poolOfWorkingCollectorBots = new();             // При передачи ботов, удалять их отсюда и из главного

    private void OnDisable()
    {
        if (_resourceScaning != null)
        {
            StopCoroutine(_resourceScaning);
            _resourceScaning = null;
        }
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
        collectorBot.TaskCompleted += OnTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.transform.SetParent(transform.parent);
        collectorBot.SetBaseAffiliation(_mainBase);
        _poolOfIdleCollectorBots.Add(collectorBot);
    }

    public void RemoveCollectorBot(CollectorBotAI collectorBot)
    {
        _poolOfIdleCollectorBots.Remove(collectorBot);
        _poolOfWorkingCollectorBots.Remove(collectorBot);
        collectorBot.TaskCompleted -= OnTaskCompleted;
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

    private void OnTaskCompleted(CollectorBotAI bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);
        _poolOfIdleCollectorBots.Add(bot);
    }

    private Task GetHarvestTask()
    {
        for (int i = 0; i < _resourcePool.Count; i++)
        {
            if (_resourcePool[i].IsOccupied == false)
            {
                _resourcePool[i].Occupy();
                return new Task(1, TypesOfTasks.Harvesting, _resourcePool[i] as Resource);               // Magic
            }
        }

        return null;
    }

    private void AddTask(Task newTask)
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

    //private Task GetTask()          // Переместить в DistributeTasks ???
    //{
    //    Task task = null;

    //    if (_taskPool.Count != 0)
    //    {
    //        task = _taskPool[0];
    //        _taskPool.RemoveAt(0);
    //    }
    //    else
    //    {
    //        task = GetHarvestTask();
    //    }

    //    return task;
    //}

    private void DistributeTasks()   // Проверить наличие задач в пуле и свободных ботов, если задач нет то сформировать задачу на сбор и выдать боту
    {
        bool isWork = true;

        while (isWork)
        {
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

            Debug.Log(_poolOfIdleCollectorBots.Count);                  //++++++++++++++++++++++++
            Debug.Log(task == null);                                  //++++++++++++++++++++++++

            if (task != null)
            {
                _poolOfIdleCollectorBots[0].SetTask(task);
                // Если задача на строительство базы, то передать ей свободного бота
                if (task.TypeOfTask == TypesOfTasks.Constructing)
                {
                    _mainBase.TransferCollectorBot(task.Target as BuildingUnderConstruction, _poolOfIdleCollectorBots[0]);
                }
                else
                {
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
            _resourcePool = _resourceScaner.MapScaning();
            DistributeTasks();
        }

        _resourceScaning = null;
    }
}
