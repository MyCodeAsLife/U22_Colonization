using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollectorBotAI : ChangingObject
{
    private BotMover _mover;
    private MainBaseAI _mainBase;
    private bool _haveCollectedResource;
    private int _interactableObjectMask;
    private Vector3 _resourceAttachmentPoint;

    public Task CurrentTask { get; private set; }

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

    private void OnTriggerEnter(Collider other)
    {
        if (CurrentTask != null && other.gameObject.layer == _interactableObjectMask)
        {
            var obj = other.GetComponent<SelectableObject>();

            if (CurrentTask.TypeOfTask == TypesOfTasks.Harvesting && _haveCollectedResource == false && obj is IResource)
            {
                if ((CurrentTask.Target as IResource).GetId() == (obj as IResource).GetId())
                {
                    _mover.Stop();
                    CurrentAction = StartCoroutine(PerformingAnAction(Collecting));
                }
            }
            else if (_haveCollectedResource && obj is MainBaseAI && obj as MainBaseAI == _mainBase)
            {
                _mover.Stop();
                StoreResource(CurrentTask.Target as IResource);
                OnMoveCompleted();
            }
            else if (CurrentTask.TypeOfTask == TypesOfTasks.Constructing && obj is BuildingUnderConstruction)
            {
                var building = obj as BuildingUnderConstruction;
                _mover.Stop();
                CurrentAction = StartCoroutine(PerformingAnAction(Constructing));
                building.StartConstruction(this);
            }
        }
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

        CurrentTask = task;
        GoTo(task.Target.transform.position);
    }

    public void SetBaseAffiliation(MainBaseAI mainBase)
    {
        _mainBase = mainBase;
    }

    protected override void StopAction()
    {
        base.StopAction();

        if (CurrentTask.TypeOfTask == TypesOfTasks.Constructing)
            (CurrentTask.Target as BuildingUnderConstruction).StopConstruction(this);
    }

    protected void CompleteTask()
    {
        TaskCompleted?.Invoke(this);
        CurrentTask = null;
    }

    private void StoreResource(IResource resource)
    {
        _haveCollectedResource = false;
        _mainBase.Store.StoreResource(resource.GetResourceType());
        resource.Delete();
        CompleteTask();
    }

    private void OnMoveCompleted()
    {
        if (CurrentTask == null)
            TaskCompleted?.Invoke(this);
        else if (_haveCollectedResource == false && CurrentTask != null)
            GoTo(CurrentTask.Target.transform.position);
        else
            GoTo(_mainBase.transform.position);
    }

    private void Collecting()
    {
        var resource = CurrentTask.Target;
        _haveCollectedResource = true;
        resource.transform.SetParent(transform);
        resource.transform.localPosition = _resourceAttachmentPoint;
        GoTo(_mainBase.transform.position);
    }

    private void Constructing()
    {
        var building = CurrentTask.Target as BuildingUnderConstruction;
        building.CompleteConstruction(this);
        CompleteTask();
        OnMoveCompleted();
    }
}