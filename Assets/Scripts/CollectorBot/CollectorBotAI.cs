using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollectorBotAI : ChangingObject
{
    private BotMover _mover;
    private Task _task;
    private MainBaseAI _mainBase;
    private Vector3 _resourceAttachmentPoint;
    private bool _haveCollectedResource;
    private int _interactableObjectMask;

    public event Action<CollectorBotAI> TaskCompleted;

    protected override void Awake()
    {
        base.Awake();
        _mover = transform.AddComponent<BotMover>();
        _interactableObjectMask = LayerMask.NameToLayer("Interactable");
    }

    private void OnEnable()
    {
        CurrentAction = null;
        _mover.MoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        if (_mover.IsMoving)
            _mover.Stop();

        _mover.MoveCompleted -= OnMoveCompleted;

        if (CurrentAction != null)
            StopAction();
    }

    private void Start()
    {
        DurationOfAction = 5f;
        _resourceAttachmentPoint = new Vector3(0, transform.localScale.y + 1, 0);
    }

    public override void StopAction()
    {
        base.StopAction();

        if (_task.TypeOfTask == TypesOfTasks.Constructing)
            (_task.Target as BuildingUnderConstruction).StopConstruction(this);
    }

    public void GoTo(Vector3 point)
    {
        if (CurrentAction != null)
        {
            StopAction();
            ActionFinish();
        }
        _mover.Move(point);
    }

    public void SetTask(Task task)
    {
        if (_haveCollectedResource)
            StoreResource(task.Target as IResource);

        _task = task;
        GoTo(task.Target.transform.position);
    }

    public void SetBaseAffiliation(MainBaseAI mainBase)
    {
        _mainBase = mainBase;
    }

    private void OnTriggerEnter(Collider other)                 // Переделать под сравнения на тип текущей задачи
    {
        if (other.gameObject.layer == _interactableObjectMask)                                  // Приделать состояние задач, и в зависимости от задачи к Action подключать нужную функцию
        {
            var obj = other.GetComponent<SelectableObject>();

            if (obj is IResource && _haveCollectedResource == false && _task != null)              // Если задание на сбор
            {
                if ((_task.Target as IResource).GetId() == (obj as IResource).GetId())
                {
                    _mover.Stop();
                    CurrentAction = StartCoroutine(PerformingAnAction(Collecting));
                }
            }
            else if (obj is MainBaseAI && _haveCollectedResource)                                   // Если задание на перенос
            {
                _mover.Stop();
                StoreResource(_task.Target as IResource);
                OnMoveCompleted();
            }
            else if (obj is BuildingUnderConstruction && _task.TypeOfTask == TypesOfTasks.Constructing)       // Если задание на строительство
            {
                var building = obj as BuildingUnderConstruction;
                _mover.Stop();
                CurrentAction = StartCoroutine(PerformingAnAction(Constructing));
                ActionProgress.Value = building.GetActionProgress();
                building.StartConstruction(this);
            }
        }
    }

    private void StoreResource(IResource resource)
    {
        _haveCollectedResource = false;
        _mainBase.StoreResource(resource.GetResourceType());
        resource.Delete();
        TaskComplete();
    }

    private void TaskComplete()
    {
        _task.Complete();
        _task = null;
    }

    private void OnMoveCompleted()
    {
        if (_task == null)
            TaskCompleted?.Invoke(this);
        else if (_haveCollectedResource == false && _task != null)
            GoTo(_task.Target.transform.position);
        else
            GoTo(_mainBase.transform.position);
    }

    private void Collecting()
    {
        var resource = _task.Target;
        _haveCollectedResource = true;
        resource.transform.SetParent(transform);
        resource.transform.localPosition = _resourceAttachmentPoint;
        GoTo(_mainBase.transform.position);
    }

    private void Constructing()                         // Строительства нет в taskPool, добавить. Добавить возврат задачи в пулл при ручной выдачи задачи
    {
        var building = _task.Target as BuildingUnderConstruction;
        // По завершению строительства у построеного здания вызвать завершающий метод в  BuildingUnderConstruction
        building.CompleteConstruction();
    }
}