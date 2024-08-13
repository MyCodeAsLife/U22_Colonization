using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Сделать атодобавления NavMeshAgent компоненнта вместе с этим компонентом
public class BotMover : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Coroutine _moving;
    private Vector3 _targetPoint;
    private bool _isWork;
    private float _moveSpeed;

    public event Action MoveCompleted;

    public bool IsMoving => _moving != null;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnDisable()
    {
        Stop();
    }

    private void Start()
    {
        //_agent = GetComponent<NavMeshAgent>();
        _moveSpeed = 7f;                                            // Нужен ли тогда ???
        _agent.speed = _moveSpeed;
    }

    public void Move(Vector3 point)
    {
        _agent.isStopped = false;
        _targetPoint = point;
        _moving = StartCoroutine(Moving());
    }

    public void Stop()
    {
        if (IsMoving)
        {
            if (_agent.isActiveAndEnabled)
                _agent.isStopped = true;

            StopCoroutine(_moving);
            _moving = null;
        }
    }

    private IEnumerator Moving()
    {
        _isWork = true;
        _targetPoint.y = 0;

        while (_isWork)                     // Переделать на  _agent.isStopped ???
        {
            yield return null;
            //transform.LookAt(_targetPoint);
            //transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _moveSpeed * Time.deltaTime);

            _agent.destination = _targetPoint;
            //_agent.Move(_targetPoint);

            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
                _isWork = false;
        }

        _agent.isStopped = true;
        MoveCompleted?.Invoke();
    }
}
