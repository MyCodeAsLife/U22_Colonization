using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CollectorBotAI : SelectableObject
{
    private BotMover _mover;
    //[SerializeField] private SelectableObject _target;                                         //++++++++++
    private Task _task;
    private MainBaseAI _mainBase;
    private Coroutine _action;

    private Vector3 _resourceAttachmentPoint;
    private bool _haveCollectedResource;
    private float _durationOfCollecting;
    //private int _resourceMask;
    private int _interactableObjectMask;

    public event Action ActionStarted;
    public event Action ActionFinished;
    public event Action<CollectorBotAI> TaskCompleted;

    protected override void Awake()
    {
        base.Awake();
        _mover = transform.AddComponent<BotMover>();
        //_resourceMask = LayerMask.NameToLayer("Resource");
        _interactableObjectMask = LayerMask.NameToLayer("Interactable");
    }

    private void OnEnable()
    {
        //_target = null;
        _action = null;
        _mover.MoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        if (_mover.IsMoving)
            _mover.Stop();

        _mover.MoveCompleted -= OnMoveCompleted;

        if (_action != null)
            StopCoroutine(_action);
    }

    private void Start()
    {
        _durationOfCollecting = 5f;
        _resourceAttachmentPoint = new Vector3(0, transform.localScale.y + 1, 0);
    }

    public void GoTo(Vector3 point)
    {
        if (_action != null)
        {
            StopCoroutine(_action);
            _action = null;
            ActionFinished?.Invoke();
        }
        _mover.Move(point);
    }

    public void SetTask(Task task)
    {
        if (_haveCollectedResource)
            StoreResource(task.Target as IResource);

        _task = task;
        //_mover.Move(task.Target.transform.position);        // �������� �������� ����� GoTo ???
        GoTo(task.Target.transform.position);               // ����� �� SetTask
    }

    public void SetBaseAffiliation(MainBaseAI mainBase)
    {
        _mainBase = mainBase;
    }

    public void SubscribeToActionProgress(Action<float> func)
    {
        _actionProgress.Change += func;
    }

    public void UnsubscribeToActionProgress(Action<float> func)
    {
        _actionProgress.Change -= func;
    }

    private void OnTriggerEnter(Collider other)                 // ���������. �������� ������� �  ���� ������ �� ������ � ��� ���������� � ���� ���������� ���������������� � ��� ��� ������� �� �������
    {
        if (other.gameObject.layer == _interactableObjectMask)                                  // ��������� ��������� �����, � � ����������� �� ������ � Action ���������� ������ �������
        {
            var obj = other.GetComponent<SelectableObject>();

            if (obj is IResource && _haveCollectedResource == false && /*_target != null*/_task != null)              // ���� ������� �� ����
            {
                if ((_task.Target as IResource).GetId() == (obj as IResource).GetId())
                {
                    _mover.Stop();
                    _action = StartCoroutine(Work(Collecting));
                }
            }
            else if (obj is MainBaseAI && _haveCollectedResource)                                   // ���� ������� �� �������
            {
                _mover.Stop();
                StoreResource(_task.Target as IResource);
                OnMoveCompleted();
            }
            else if (obj is BuildingUnderConstruction)                                             // ���� ������� �� �������������
            {
                _mover.Stop();
                _action = StartCoroutine(Work(Constructing));
            }
        }
    }

    private void StoreResource(IResource resource)
    {
        _haveCollectedResource = false;
        _mainBase.StoreResource(resource.GetResourceType());
        resource.Delete();
        //_target = null;
        TaskComplete();
    }

    private void TaskComplete()
    {
        _task.Complete();
        _task = null;
    }

    //private void OnMoveCompleted()
    //{
    //    if (_target == null)
    //        TaskCompleted?.Invoke(this);
    //    else if (_haveCollectedResource == false && _target != null)
    //        GoTo(_target.transform.position);
    //    else
    //        GoTo(_mainBase.transform.position);
    //}

    private void OnMoveCompleted()
    {
        if (_task == null)
            TaskCompleted?.Invoke(this);
        else if (_haveCollectedResource == false && _task != null)
            GoTo(_task.Target.transform.position);          // ����� �� OnMoveCompleted
        else
            GoTo(_mainBase.transform.position);          // ����� �� OnMoveCompleted
    }

    private void Collecting()
    {
        //var resource = (Resource)_target;
        //var resource = _task.Target as Resource;                        // ������ ��������� ������ �� ������(��� ������ ����������� ������� � ������)
        var resource = _task.Target;                        // ������ ��������� ������ �� ������(��� ������ ����������� ������� � ������)
        _haveCollectedResource = true;
        resource.transform.SetParent(transform);
        resource.transform.localPosition = _resourceAttachmentPoint;
        //_mover.Move(_mainBase.transform.position);             // �������� �������� ����� GoTo ???
        GoTo(_mainBase.transform.position);          // ����� �� Collecting
    }

    private void Constructing()                         // ������������� ��� � taskPool, ��������. �������� ������� ������ � ���� ��� ������ ������ ������
    {
        //var building = (BuildingUnderConstruction)_target;
        var building = _task.Target as BuildingUnderConstruction;
        // �� ���������� ������������� � ����������� ������ ������� ����������� ����� �  BuildingUnderConstruction
        building.CompleteConstruction();
    }

    private IEnumerator Work(Action action)             // �������� ��� �������� (� ���� �������?)
    {
        ActionStarted?.Invoke();                        // ������������� � Action
        float timer = 0;

        while (timer < _durationOfCollecting)           // ������������� � Action
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _actionProgress.Value = timer / _durationOfCollecting;
        }

        action();
        ActionFinished?.Invoke();
        _action = null;
        // ������ ������ �������
    }
}