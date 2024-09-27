using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BotMover : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Coroutine _moving;
    private Vector3 _targetPoint;
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
        _moveSpeed = 7f;
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
        bool isWork = true;
        _targetPoint.y = 0;

        while (isWork)
        {
            yield return null;
            _agent.destination = _targetPoint;

            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
                isWork = false;
        }

        _agent.isStopped = true;
        MoveCompleted?.Invoke();
    }
}
