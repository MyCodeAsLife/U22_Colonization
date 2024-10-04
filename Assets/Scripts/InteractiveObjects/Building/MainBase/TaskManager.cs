using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour, ITaskManager
{
    private IStore _store;
    private MainBase _mainBase;
    private Coroutine _resourceScaning;
    private ResourceScaner _resourceScaner;
    private BuildingUnderConstruction _scheduleBuilding;

    private List<Task> _taskPool = new();
    private List<IResource> _resourcePool = new();
    private List<CollectorBot> _poolOfIdleCollectorBots = new();
    private List<CollectorBot> _poolOfWorkingCollectorBots = new();

    public bool IsBuildingPlanned => _scheduleBuilding != null;

    private void Awake()
    {
        _mainBase = GetComponent<MainBase>();
        _store = _mainBase.Store;
    }

    private void OnEnable()
    {
        _store.ResourcesQuantityChanged += CheckShedule;
    }

    private void OnDisable()
    {
        if (_resourceScaning != null)
        {
            StopCoroutine(_resourceScaning);
            _resourceScaning = null;
        }

        _store.ResourcesQuantityChanged -= CheckShedule;
    }

    public void AddResourceScaner(ResourceScaner resourceScaner)
    {
        _resourceScaner = resourceScaner;
        _resourceScaning = StartCoroutine(ResourceScanning());
    }

    public void AddCollectorBot(CollectorBot collectorBot)
    {
        collectorBot.SetBaseAffiliation(_mainBase);
        collectorBot.TaskCompleted += OnTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.transform.SetParent(transform.parent);
        _poolOfIdleCollectorBots.Add(collectorBot);
    }

    public void RemoveCollectorBot(CollectorBot collectorBot)
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

            if (TryStartConstruction() == false)
                _mainBase.SubscribeChangesNumberBots(OnBotsQuantityChanged);
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
    }

    private void CheckShedule()
    {
        if (_scheduleBuilding != null)
        {
            if (TryStartConstruction())
                _mainBase.UnSubscribeChangesNumberBots(OnBotsQuantityChanged);
        }
        else
        {
            TryBuyCollectorBot();
        }
    }

    private void OnBotsQuantityChanged(int amount)
    {
        CheckShedule();
    }

    private bool TryBuyCollectorBot()
    {
        var amountOfResources = _store.AmountOfResources;
        var price = _mainBase.GetPriceOf(Price.CollectorBot);

        if (amountOfResources.Food >= price.Food &&
            amountOfResources.Timber >= price.Timber &&
            amountOfResources.Marble >= price.Marble)
        {
            _mainBase.Store.SubtractResources(price);
            _mainBase.CreateCollectorBot();
            return true;
        }

        return false;
    }

    private bool TryStartConstruction()
    {
        var amountOfResources = _store.AmountOfResources;
        var price = _mainBase.GetPriceOf(Price.MainBase);
        const int BuildingPriority = 0;
        const int MinimumRequiredNumberOfBots = 1;

        if (amountOfResources.Food >= price.Food &&
            amountOfResources.Timber >= price.Timber &&
            amountOfResources.Marble >= price.Marble &&
            _mainBase.NumberOfBots > MinimumRequiredNumberOfBots)
        {
            AddTask(new Task(BuildingPriority, TypeOfTask.Constructing, _scheduleBuilding));
            _scheduleBuilding = null;
            _mainBase.Store.SubtractResources(price);
            return true;
        }

        return false;
    }

    private void OnTaskCompleted(CollectorBot bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);

        if (_poolOfIdleCollectorBots.Contains(bot) == false)
            _poolOfIdleCollectorBots.Add(bot);
    }

    private Task GetHarvestTask()
    {
        const int HarvestPriority = 1;

        for (int i = 0; i < _resourcePool.Count; i++)
        {
            if (_resourcePool[i].IsOccupied == false)
            {
                _resourcePool[i].Occupy();
                return new Task(HarvestPriority, TypeOfTask.Harvesting, _resourcePool[i] as Resource);
            }
        }

        return null;
    }

    private void AddTask(Task newTask)
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

    private void DistributeTasks()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_poolOfIdleCollectorBots.Count < 1)
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

                if (task == null)
                    break;
            }

            _poolOfIdleCollectorBots[0].SetTask(task);

            if (task.TypeOfTask == TypeOfTask.Constructing)
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
