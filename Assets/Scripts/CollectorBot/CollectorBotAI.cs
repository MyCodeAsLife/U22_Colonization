using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CollectorBotAI : SelectableObject
{
    private BotMover _mover;
    [SerializeField] private Resource _resource;                                         //++++++++++
    private MainBaseAI _mainBase;
    private SingleReactiveProperty<float> _collectionProgress = new();
    private Coroutine _collecting;

    private Vector3 _resourceAttachmentPoint;
    private bool _haveCollectedResource;
    private float _durationOfCollecting;
    private int _resourceMask;
    private int _interactableObjectMask;

    public event Action CollectingStarted;
    public event Action CollectingFinished;
    public event Action<CollectorBotAI> TaskCompleted;

    protected override void Awake()
    {
        base.Awake();
        _mover = transform.AddComponent<BotMover>();
        _resourceMask = LayerMask.NameToLayer("Resource");
        _interactableObjectMask = LayerMask.NameToLayer("Interactable");
    }

    private void OnEnable()
    {
        _resource = null;
        _collecting = null;
        _mover.MoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        if (_mover.IsMoving)
            _mover.Stop();

        _mover.MoveCompleted -= OnMoveCompleted;

        if (_collecting != null)
            StopCoroutine(_collecting);
    }

    private void Start()
    {
        _durationOfCollecting = 5f;
        _resourceAttachmentPoint = new Vector3(0, transform.localScale.y + 1, 0);
    }

    public void GoTo(Vector3 point)
    {
        if (_collecting != null)
        {
            StopCoroutine(_collecting);
            _collecting = null;
            CollectingFinished?.Invoke();
        }

        _mover.Move(point);
    }

    public void SetCollectionTask(Resource resource)
    {
        if (_haveCollectedResource)
            StoreResource();

        _resource = resource;
        _mover.Move(_resource.transform.position);
    }

    public void SetBaseAffiliation(MainBaseAI mainBase)
    {
        _mainBase = mainBase;
    }

    public void SubscribeToCollectionProgress(Action<float> func)
    {
        _collectionProgress.Change += func;
    }

    public void UnsubscribeToCollectionProgress(Action<float> func)
    {
        _collectionProgress.Change -= func;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_resource != null && other.gameObject.layer == _resourceMask)
        {
            if (other.GetComponent<Resource>() == _resource)
            {
                _mover.Stop();
                _collecting = StartCoroutine(Collecting());
            }
        }
        else if (other.gameObject.layer == _interactableObjectMask && _haveCollectedResource)
        {
            if (other.GetComponent<MainBaseAI>() == _mainBase)
            {
                _mover.Stop();
                StoreResource();
                OnMoveCompleted();
            }
        }
    }

    private void StoreResource()
    {
        _haveCollectedResource = false;
        _mainBase.StoreResource(_resource.ResourceType);
        _resource.Delete();
        _resource = null;
    }

    private void OnMoveCompleted()
    {
        if (_resource == null)
            TaskCompleted?.Invoke(this);
        else if (_haveCollectedResource == false && _resource != null)
            GoTo(_resource.transform.position);
        else
            GoTo(_mainBase.transform.position);
    }

    private IEnumerator Collecting()
    {
        CollectingStarted?.Invoke();
        float timer = 0;

        while (timer < _durationOfCollecting)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _collectionProgress.Value = timer / _durationOfCollecting;
        }

        CollectingFinished?.Invoke();
        _haveCollectedResource = true;
        _resource.transform.SetParent(transform);
        _resource.transform.localPosition = _resourceAttachmentPoint;
        _mover.Move(_mainBase.transform.position);
        _collecting = null;
    }
}