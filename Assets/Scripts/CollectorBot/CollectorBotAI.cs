using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    //public void SetTask(Resource resource)                                              // Здесь==================================
    //{
    //    if (_haveCollectedResource)
    //        StoreResource((Resource)_target);

    //    _target = resource;
    //    _mover.Move(_target.transform.position);
    //}

    public void SetTask(Task task)
    {
        if (_haveCollectedResource)
            StoreResource(task.Target as Resource);

        _task = task;
        _mover.Move(task.Target.transform.position);
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

    private void OnTriggerEnter(Collider other)                 // Упростить. Получать задание в  виде ссылки на объект и при пиближении к нему сравнивать соприкоснувшийся с тем что получен по заданию
    {
        if (other.gameObject.layer == _interactableObjectMask)                                  // Приделать состояние задач, и в зависимости от задачи к Action подключать нужную функцию
        {
            var obj = other.GetComponent<SelectableObject>();

            if (obj is Resource && _haveCollectedResource == false && /*_target != null*/_task != null)              // Если задание на сбор
            {
                if (Vector3.Distance(transform.position, _task.Target.transform.position) < 5f)
                {
                    _mover.Stop();
                    _action = StartCoroutine(Work(Collecting));
                }
            }
            else if (obj is MainBaseAI && _haveCollectedResource)                                   // Если задание на перенос
            {
                _mover.Stop();
                StoreResource(_task.Target as Resource);
                OnMoveCompleted();
            }
            else if (obj is BuildingUnderConstruction)                                             // Если задание на строительство
            {
                _mover.Stop();
                _action = StartCoroutine(Work(Constructing));
            }
        }
    }

    private void StoreResource(Resource resource)
    {
        _haveCollectedResource = false;
        _mainBase.StoreResource(resource.ResourceType);
        resource.Delete();
        //_target = null;
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
            GoTo(_task.Target.transform.position);
        else
            GoTo(_mainBase.transform.position);
    }

    private void Collecting()
    {
        //var resource = (Resource)_target;
        var resource = _task.Target as Resource;
        _haveCollectedResource = true;
        resource.transform.SetParent(transform);
        resource.transform.localPosition = _resourceAttachmentPoint;
        _mover.Move(_mainBase.transform.position);
    }

    private void Constructing()
    {
        //var building = (BuildingUnderConstruction)_target;
        var building = _task.Target as BuildingUnderConstruction;
        // По завершению строительства у построеного здания вызвать завершающий метод в  BuildingUnderConstruction
        building.CompleteConstruction();
    }

    private IEnumerator Work(Action action)             // Получать тип действия (в виде функции?)
    {
        ActionStarted?.Invoke();                        // Переименовать в Action
        float timer = 0;

        while (timer < _durationOfCollecting)           // Переименовать в Action
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _actionProgress.Value = timer / _durationOfCollecting;
        }

        action();
        ActionFinished?.Invoke();
        _action = null;
        // Запрос нового задания
    }
}